using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;
        
        public void StartAction(IAction action)
        {
            if (currentAction == action) { return; }
            if (currentAction != null)
            {
                currentAction.Cancel(); //if there is a switch between Fighter and Mover the other's cancel is called
            }
            currentAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null); //this stops the current action
        }
    }
}

