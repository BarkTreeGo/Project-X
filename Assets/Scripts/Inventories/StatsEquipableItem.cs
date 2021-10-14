using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using RPG.Stats;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider //inherits from the normal Equipable item script on the player
    {
        [SerializeField] Modifier[] additiveModifiers;
        [SerializeField] Modifier[] percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var additiveModifier in additiveModifiers)
            {
                if (stat == additiveModifier.stat)
                {                    
                    yield return additiveModifier.value;                    
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var percentageModifier in percentageModifiers)
            {
                if (stat == percentageModifier.stat)
                {
                    yield return percentageModifier.value;
                }
            }
        }
    }
}   
