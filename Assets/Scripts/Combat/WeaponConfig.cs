using GameDevTV.Inventories;
using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] AnimatorOverrideController AnimatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController; // to check if the runtimeAnimatorController is empty (basically AnimatorOverrideController inherits from runtimeAnimatorController and has a property smartly named runtimeAnimatorController for when its not connected to the runtime, so if you cast runtimeAnimatorController using "as" (as returns null if it can't cast) to AnimatorOverrideController you can check if anything is overriding. If the animator override is hooked up it will be won't be null.... I know)

            Weapon weapon = null;

            if(equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);

                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }


            if (AnimatorOverride != null) // is something in the AnimatorOverride [SerializeField] if yes set runtimeAnimatorController to it 
            {
                animator.runtimeAnimatorController = AnimatorOverride;
            }
            else if (overrideController != null) // is an AnimatorOverride already in the runtimeAnimatorController if yes take it out
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {          
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetPecentageBonus()
        {
            return percentageBonus;
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }

        Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform handTransform;
            if (isRightHanded) { handTransform = rightHandTransform; }
            else { handTransform = leftHandTransform; }

            return handTransform;
        }

        void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform oldWeapon = rightHandTransform.Find(weaponName);

            if(oldWeapon == null)
            {
                oldWeapon = leftHandTransform.Find(weaponName);
            }
            if(oldWeapon == null) { return; }

            oldWeapon.name = "DESTROYING"; //To ensure that a new "Weapon" picked up does not conflict if delayed
            Destroy(oldWeapon.gameObject);
        }
    }

}