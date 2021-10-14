using GameDevTV.Utils;
using RPG.Control;
using RPG.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class PauseMenuUI : MonoBehaviour
    {       
        LazyValue<PlayerController> playerController;
        LazyValue<SavingWrapper> savingWrapper;

        private void Awake()
        {
            playerController = new LazyValue<PlayerController>(GetPlayerController);
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }

        private PlayerController GetPlayerController()
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        private void OnEnable()
        {
            if (playerController.value == null) { return; }
            Time.timeScale = 0; //sets how events are happening relative to realworld time.
            playerController.value.enabled = false;
        }

        private void OnDisable()
        {
            if (playerController.value == null) { return; }
            Time.timeScale = 1; //can be used to change the gamespeed
            playerController.value.enabled = true;
        }

        public void Save()
        {
            savingWrapper.value.Save();
        }

        public void SaveAndQuit()
        {            
            savingWrapper.value.Save();
            savingWrapper.value.LoadMenu();
        }
    }
}
