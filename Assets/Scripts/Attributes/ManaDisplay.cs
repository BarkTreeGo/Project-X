using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {       
        // cached References
        Mana mana;

        private void Awake()
        {
            mana = GameObject.FindGameObjectWithTag("Player").GetComponent<Mana>();
        }

        private void Update()
        {
            //using a String formater which indicates by the :0.0 to use 0 decimal places
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", mana.GetMana(), mana.GetMaxMana());
        }
    }
}

