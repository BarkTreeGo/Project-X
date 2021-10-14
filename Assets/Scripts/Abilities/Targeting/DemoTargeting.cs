using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "DemoTargeting", menuName = "Abilities/Targeting/Demo", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            Debug.Log("Demotargeting Started");
            finished();
        }
    }
}
