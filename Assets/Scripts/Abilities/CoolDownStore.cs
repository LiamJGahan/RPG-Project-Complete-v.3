using GameDevTV.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class CoolDownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> coolDownTimers = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> initialCoolDownTimes = new Dictionary<InventoryItem, float>();

        void Update()
        {
            var keys = new List<InventoryItem>(coolDownTimers.Keys);
            foreach(InventoryItem ability in keys)
            {
                coolDownTimers[ability] -= Time.deltaTime;
                if(coolDownTimers[ability] < 0)
                {
                    coolDownTimers.Remove(ability);
                    initialCoolDownTimes.Remove(ability);
                }
            }
        }

        public void StartCooldown(InventoryItem ability, float cooldownTime)
        {
            coolDownTimers[ability] = cooldownTime;
            initialCoolDownTimes[ability] = cooldownTime;
        }

        public float GetTimeRemaining(InventoryItem ability)
        {
            if(!coolDownTimers.ContainsKey(ability))
            {
                return 0;
            }

            return coolDownTimers[ability];
        }

        public float GetFractionRemaining(InventoryItem ability)
        {
            if(ability == null) { return 0; }

            if(!coolDownTimers.ContainsKey(ability))
            {
                return 0;
            }

            return coolDownTimers[ability] / initialCoolDownTimes[ability];
        }
    }
}
