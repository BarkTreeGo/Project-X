using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Orient to Target Effect", menuName = "Abilities/Effects/Orient to Target", order = 0)]
    public class OrientToTargetEffect : EffectStrategy
    {
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().transform.LookAt(data.GetTargetedPoint());
            finished();
        }
    }
}
