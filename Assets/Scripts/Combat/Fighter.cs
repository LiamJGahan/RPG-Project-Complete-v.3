using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using GameDevTV.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;
using GameDevTV.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] bool autoAttack = true;
        [SerializeField] float autoAttackRange = 4f;


        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        Health target;
        Equipment equipment; 

        void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        Weapon SetupDefaultWeapon()
        {           
            return AttachWeapon(defaultWeapon);
        }

        void Start()
        {
            currentWeapon.ForceInit();
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; }
            if (target.IsDead()) 
            {
                if (autoAttack)
                {
                    target = FindNewTargetInRange();

                    if (target == null) { return; }
                }
                else
                {
                    return;
                }
            }

            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }

        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform))
            { 
                return false;
            }

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        void UpdateWeapon()
        {
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public Transform GetHandTransform(bool isRightHanded)
        {
            if (isRightHanded)
            {
                return rightHandTransform;
            }
            else
            {
                return leftHandTransform;
            }
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPecentageBonus();
            }
        }

        void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            //This will trigger the Hit() Event
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        Health FindNewTargetInRange()
        {
            Health bestCandidate = null;
            float bestDistance = Mathf.Infinity;

            foreach(Health candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);
                if (candidateDistance < bestDistance)
                {
                    bestCandidate = candidate;
                    bestDistance = candidateDistance;
                }
            }

            return bestCandidate;
        }

        IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up);

            foreach (RaycastHit hit in hits)
            {
                Health health = hit.transform.GetComponent<Health>();

                if (health == null) { continue; }
                if (health.IsDead()) { continue; }
                if (health.gameObject == gameObject) { continue; }

                yield return health;
            }
        }

        void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("Cancel Attack");
            GetComponent<Animator>().SetTrigger("Attack");
        }

        //Animation Event
        void Hit()
        {
            if (target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            BaseStats targetBaseStats = target.GetComponent<BaseStats>();

            if(targetBaseStats != null)
            {
                float defence = targetBaseStats.GetStat(Stat.Defence);

                damage /= 1 + defence / damage;
            }

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if(currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        //Animation Event
        void Shoot()
        {
            Hit();
        }

        bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponRange();
        }

        void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("Cancel Attack");
        }
    }
}
