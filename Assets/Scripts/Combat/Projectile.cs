using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{

    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed = 0.2f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 0.2f;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        Vector3 targetPoint;
        GameObject instigator = null;
        float damage = 0;

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (target)
            {
                if (isHoming && !target.IsDead())
                {
                    transform.LookAt(GetAimLocation());
                }
                if (target.IsDead())
                {
                    target = null;
                    Destroy(gameObject);
                }
            }
            transform.Translate(Vector3.forward * (projectileSpeed * Time.deltaTime));
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, target);
        }

        public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }

        public void SetTarget(GameObject instigator, float damage, Health target=null, Vector3 targetPoint=default)
        {
            this.target = target;
            this.targetPoint = targetPoint;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }

        Vector3 GetAimLocation()
        {
            if(target == null)
            {
                return targetPoint;
            }

            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null)
            {
                return target.transform.position;
            }

            return target.transform.position + Vector3.up * targetCapsule.height / 2;  
        }

        void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();

            if (target != null && health != target) return;
            if (health == null || health.IsDead()) return;
            if (other.gameObject == instigator) return;

            health.TakeDamage(instigator, damage);

            projectileSpeed = 0f;

            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);

        }
    }
}
