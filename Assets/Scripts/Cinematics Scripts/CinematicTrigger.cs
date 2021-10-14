using GameDevTV.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool alreadyTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && !alreadyTriggered)
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }
        }

        public object CaptureState()
        {
            return alreadyTriggered;            
        }

        public void RestoreState(object state)
        {
            alreadyTriggered = (bool)state; //casting to convert object to bool            
        }
    }

}
