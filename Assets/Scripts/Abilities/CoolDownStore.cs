using GameDevTV.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class CoolDownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> cooldownTimers = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> initialCooldownTimes = new Dictionary<InventoryItem, float>();

        private void Update()
        {
            List<InventoryItem> keys = new List<InventoryItem>(cooldownTimers.Keys);
            foreach (InventoryItem ability in keys)
            {
                cooldownTimers[ability] -= Time.deltaTime;
                if (cooldownTimers[ability] < 0)
                {
                    cooldownTimers.Remove(ability);
                    initialCooldownTimes.Remove(ability);
                }
            }
        }

        public void StartCooldown(InventoryItem ability, float cooldownTime)
        {
            cooldownTimers[ability] = cooldownTime;
            initialCooldownTimes[ability] = cooldownTime;
        }

        public float GetTimeRemaining(InventoryItem ability)
        {
            if (!cooldownTimers.ContainsKey(ability))
            {
                return 0f;
            }

            return cooldownTimers[ability];
        }

        public float GetFractionRemaining(InventoryItem ability)
        {
            if (ability == null)
            {
                return 0f;
            }

            if (!cooldownTimers.ContainsKey(ability))
            {
                return 0f;
            }

            return cooldownTimers[ability] / initialCooldownTimes[ability];
        }
    }
}
