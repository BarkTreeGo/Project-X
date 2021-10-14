using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "Abilities/Effects/Delay Composite", order = 0)]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] float delay = 0f;
        [SerializeField] EffectStrategy[] delayedEffects;
        [SerializeField] bool abortIfCancelled = false;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffects(data, finished));
            finished();
        }

        private IEnumerator DelayedEffects(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);
            if (abortIfCancelled && data.IsCancelled()) { yield break; } 
            foreach (EffectStrategy effect in delayedEffects)
            {
                effect.StartEffect(data, finished);                
            }
        }
    }
}
