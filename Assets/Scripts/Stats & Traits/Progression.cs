using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgessionStat[] stats;
        }

        [System.Serializable]
        class ProgessionStat
        {
            public Stat stat;
            public float[] levels;
        }

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookupTabel();

            if (!lookupTable[characterClass].ContainsKey(stat))
            {
                return 0;
            }

            float[] levels = lookupTable[characterClass][stat];
            
            if (levels.Length == 0)
            {
                return 0;
            }
            
            if (levels.Length < level)
            {
                return levels[levels.Length -1];
            }
            return levels[level - 1];
        }

        private void BuildLookupTabel()
        {
            if (lookupTable != null) { return; }

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgessionStat progressionStat in progressionCharacterClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionCharacterClass.characterClass] = statLookupTable;
            }
        }

        public int GetLevels (Stat stat, CharacterClass characterClass)
        {
            BuildLookupTabel();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }  

    }
}

    
