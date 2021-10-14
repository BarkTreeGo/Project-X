using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameDevTV.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0f;

        //public delegate void ExperienceGainedDelegate();
        //public event ExperienceGainedDelegate OnExperienceGained;
        public event Action onExperienceGained; //Action is a delegate with no return value, which forms list of methods to run

        private void Update() //for debugging only
        {
            if (Input.GetKey(KeyCode.E))
            {
                GainExperience(Time.deltaTime * 1000);
            }
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();            
        }
        
        public float GetPoints()
        {
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}

