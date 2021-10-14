using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        // cached References
        Experience experience;

        private void Awake()
        {
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            //using a String formater which indicates by the :0.0 to use 1 decimal places
            GetComponent<Text>().text = String.Format("{0:0}", experience.GetPoints());
        }
    }
}
