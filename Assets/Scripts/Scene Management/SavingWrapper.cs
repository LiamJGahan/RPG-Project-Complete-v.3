using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string currentSaveKey = "currentSaveName";

        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] int firstLevelBuildIndex = 1;
        [SerializeField] int menuLevelBuildIndex = 0;

        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(currentSaveKey)) { return; }
            if (!GetComponent<SavingSystem>().SaveFileExists(GetCurrentSave())) { return; }

            StartCoroutine(LoadLastScene());
        }

        public void NewGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            StartCoroutine(LoadFirstSceneAndSave());
        }

        public void LoadGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuScene());
        }

        void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(currentSaveKey, saveFile);
        }

        string GetCurrentSave()
        {
            return PlayerPrefs.GetString(currentSaveKey);
        }

        IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();

            yield return fader.FadeOut(fadeOutTime);

            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());

            yield return fader.FadeIn(fadeInTime);
        }

        IEnumerator LoadFirstSceneAndSave()
        {
            Fader fader = FindObjectOfType<Fader>();

            yield return fader.FadeOut(fadeOutTime);

            yield return SceneManager.LoadSceneAsync(firstLevelBuildIndex);

            yield return fader.FadeIn(fadeInTime);

            Save();
        }

        IEnumerator LoadMenuScene()
        {
            Fader fader = FindObjectOfType<Fader>();

            yield return fader.FadeOut(fadeOutTime);

            yield return SceneManager.LoadSceneAsync(menuLevelBuildIndex);

            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(LoadLastScene()); //formally would have used Load(); but was causing duplication issues.
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if(Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSave();
            }
        }
        public void Load()
        {
            //Call to saving system
            GetComponent<SavingSystem>().Load(GetCurrentSave());
        }

        public void Save()
        {
            //Call to saving system
            GetComponent<SavingSystem>().Save(GetCurrentSave());
        }

        public void DeleteSave()
        {
            //Call to saving system
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
            print("SaveFile Deleted");
        }

        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }
    }
}
