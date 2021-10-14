using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)] [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect;
        [SerializeField] bool shouldUseModifiers = false;
        
        LazyValue<int> currentLevel;

        public event Action onLevelUp; //Action is a delegate with no return value, which forms list of methods to run

        // cached References
        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;                
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;               
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp(); //Action is a delegate with no return value, which forms list of methods to run
    }            
        }

        private void LevelUpEffect()
        {
            if (levelUpParticleEffect != null)
            {
                Instantiate(levelUpParticleEffect, transform);
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifiers(stat)/100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            if (!shouldUseModifiers) { return 0f; }
            
            float total = 0;
            
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifiers(Stat stat)
        {
            if (!shouldUseModifiers) { return 0f; }
            
            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public int GetLevel()
        {            
            return currentLevel.value;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null)
            {
                return startingLevel;
            }
            
            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float xPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (xPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel +1;
        }
    }
}


