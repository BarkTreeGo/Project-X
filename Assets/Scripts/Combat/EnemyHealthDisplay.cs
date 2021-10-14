using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {       
        // cached References        
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
                  
            if (fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            else
            {
                Health health = fighter.GetTarget();
                //using a String formater which indicates by the :0.0 to use 0 decimal places
                GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GethealthPoints(), health.GetMaxhealthPoints());
            }
        }
    }
}

