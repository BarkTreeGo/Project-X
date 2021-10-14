using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        // cached References
        BaseStats baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            //using a String formater which indicates by the :0.0 to use 1 decimal places
            GetComponent<Text>().text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}
