using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {       
        // cached References
        Health health;

        private void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            
        }

        private void Update()
        {
            //using a String formater which indicates by the :0.0 to use 0 decimal places
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GethealthPoints(), health.GetMaxhealthPoints());            
        }
    }
}

