using RPG.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] Trait trait; 
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] Button plusButton;
        [SerializeField] Button minusButton;
                
        TraitStore playerTraitStore = null;

        private void Start()
        {
            playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            plusButton.onClick.AddListener(() => Allocate(+1));
            minusButton.onClick.AddListener(() => Allocate(-1));            
        }

        private void Update()
        {
            minusButton.interactable = playerTraitStore.CanAssignPoints(trait, -1);
            plusButton.interactable = playerTraitStore.CanAssignPoints(trait, +1);
            valueText.text = playerTraitStore.GetProposedPoints(trait).ToString();
        }               

        public void Allocate(int points)
        {
            playerTraitStore.AssignPoints(trait, points);            
        }
    }
}
