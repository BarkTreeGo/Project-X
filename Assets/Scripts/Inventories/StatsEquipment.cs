using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Inventories;
using RPG.Stats;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider //inherits from equipmemt...note you have equipment scrObject and Statsequipement scrObject
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots()) //goes over each slot 
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) { continue; }
                
                foreach (float modifier in item.GetAdditiveModifiers(stat)) //goed over each found IModifier proider (i.e. Statsequipment scrObject)
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) { continue; }

                foreach (float modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}
