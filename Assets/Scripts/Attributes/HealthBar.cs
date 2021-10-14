using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        float initialHealth;
        float currentHealth;
        
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            //disable healthbar if health is zero or if it is full
            if (Mathf.Approximately(healthComponent.GetFraction(), 0) || Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }
            //else
            rootCanvas.enabled = true;
            
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1f, 1f);
        }
    }
}
