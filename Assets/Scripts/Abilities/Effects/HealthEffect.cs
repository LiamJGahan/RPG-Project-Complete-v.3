using RPG.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "Abilities/Effects/Health")]
    class HealthEffect : EffectStrategy
    {
        [SerializeField] float healthChange = 1f;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach(GameObject target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();

                if(health)
                {
                    if(healthChange < 0)
                    {
                        health.TakeDamage(data.GetUser(), -healthChange);
                    }
                    else
                    {
                        health.Heal(healthChange);
                    }
                }
            }

            finished();
        }
    }
}
