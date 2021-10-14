using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Abilities
{
    public abstract class TargetingStrategy : ScriptableObject
    {
          //Action is passed as parameter, which returns the IEnumerable (i.e. the targets) when the finished function is called.  
          //this is handy when a function cannot immediatly return and output, but takes time
          //hence this method is called a callback using the Action-trigger          
          public abstract void StartTargeting(AbilityData data, Action finished);
       
    }
}
