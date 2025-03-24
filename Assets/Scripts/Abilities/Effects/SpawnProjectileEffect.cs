using RPG.Attributes;
using RPG.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = "Abilities/Effects/Spawn Projectile")]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] Projectile projectileToSpawn;
        [SerializeField] float damage = 10;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] bool useTargetPoint = true;

        public override void StartEffect(AbilityData data, Action finished)
        {

            Fighter fighter = data.GetUser().GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHandTransform(isRightHanded).position;

            if (useTargetPoint)
            {
                SpawnProjectilesForTargetPoint(data, spawnPosition);
            }
            else
            {
                SpawnProjectilesForTargets(data, spawnPosition);
            }

            finished();
        }

        void SpawnProjectilesForTargetPoint(AbilityData data, Vector3 spawnPosition)
        {
            Projectile projectile = Instantiate(projectileToSpawn);

            projectile.transform.position = spawnPosition;
            projectile.SetTarget(data.GetTargetedPoint(), data.GetUser(), damage);
        }

        void SpawnProjectilesForTargets(AbilityData data, Vector3 spawnPosition)
        {
            foreach (GameObject target in data.GetTargets())
            {
                Health targetHealth = target.GetComponent<Health>();

                if (targetHealth)
                {
                    Projectile projectile = Instantiate(projectileToSpawn);

                    projectile.transform.position = spawnPosition;
                    projectile.SetTarget(targetHealth, data.GetUser(), damage);
                }
            }
        }
    }
}
