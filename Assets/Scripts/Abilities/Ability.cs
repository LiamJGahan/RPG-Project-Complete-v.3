using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Abilities/Ability")]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float coolDownTime = 2;
        [SerializeField] float manaCost = 5;

        public override bool Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if(mana.GetMana() < manaCost) { return false; }

            CoolDownStore coolDownStore  = user.GetComponent<CoolDownStore>();
            if(coolDownStore.GetTimeRemaining(this) > 0) { return false; }

            AbilityData data = new AbilityData(user);

            ActionScheduler actionScheduler = user.GetComponent<ActionScheduler>();
            actionScheduler.StartAction(data);

            targetingStrategy.StartTargeting(data, () => TargetAquired(data));

            return true;
        }
        
        void TargetAquired(AbilityData data)
        {
            if(data.IsCancelled()) { return; }

            Mana mana = data.GetUser().GetComponent<Mana>();
            if(!mana.UseMana(manaCost)) { return; }

            CoolDownStore coolDownStore = data.GetUser().GetComponent<CoolDownStore>();
            coolDownStore.StartCooldown(this, coolDownTime);

            foreach(FilterStrategy filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }

            foreach(EffectStrategy effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
        }

        void EffectFinished()
        {

        }
    }
}
