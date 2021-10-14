using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "My Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targeting;
        [SerializeField] FilterStrategy[] filterStrategies; //allows multiple filters to be applied
        [SerializeField] EffectStrategy[] effectStrategies; //allows multiple effects to be applied
        [SerializeField] float cooldownTime = 0f;
        [SerializeField] float manaCost = 0f;

        public override bool Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if (mana.GetMana() < manaCost) 
            {
                return false;
            }

            CoolDownStore coolDownStore = user.GetComponent<CoolDownStore>();
            if (coolDownStore.GetTimeRemaining(this) > 0)
            {
                return false;
            }           

            AbilityData data = new AbilityData(user);

            ActionScheduler actionScheduler = user.GetComponent<ActionScheduler>();
            actionScheduler.StartAction(data);

            targeting.StartTargeting(data, () => 
                {
                    TargetAcquired(data);

                }); //passes in the function result as a parameter, which passes in the IEnumerable.
            return true;
        }

        private void TargetAcquired(AbilityData data)
        {
            if (data.IsCancelled()) { return; }
            
            Mana mana = data.GetUser().GetComponent<Mana>();
            if (!mana.UseMana(manaCost)) { return; }        
                        
            CoolDownStore coolDownStore = data.GetUser().GetComponent<CoolDownStore>();
            coolDownStore.StartCooldown(this, cooldownTime);            

            foreach (FilterStrategy filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }

            foreach (EffectStrategy effect in effectStrategies)
            {
                effect.StartEffect(data, Effectfinished);
            }
        }

        private void Effectfinished()
        {

        }
    }
}
