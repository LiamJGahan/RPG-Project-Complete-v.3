using RPG.Control;
using RPG.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    class PauseMenuUI : MonoBehaviour
    {
        PlayerController playerController;

        void Awake()
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        void OnEnable()
        {
            if(playerController == null) { return; }

            Time.timeScale = 0;
            playerController.SetCursor(CursorType.UI);
            playerController.enabled = false;
        }

        void OnDisable()
        {
            if (playerController == null) { return; }

            Time.timeScale = 1;
            playerController.enabled = true;
        }

        public void Save()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
        }

        public void SaveAndQuit()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            savingWrapper.LoadMenu();
        }
    }
}
