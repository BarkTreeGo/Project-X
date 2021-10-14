using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] Transform contentRoot;
        [SerializeField] GameObject buttonPrefab;
        

        private void OnEnable()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper == null) return;

            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }
            
            foreach (string save in savingWrapper.ListSaves())
            {
                GameObject buttonInstance =  Instantiate(buttonPrefab, contentRoot);
                TMP_Text textComp = buttonInstance.GetComponentInChildren<TMPro.TMP_Text>();
                textComp.text = save;
                Button button = buttonInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    savingWrapper.LoadGame(save);
                });
            }            
        }
    }
}
