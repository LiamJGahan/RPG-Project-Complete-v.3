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
        [SerializeField] GameObject contentRoot;
        [SerializeField] GameObject buttonPrefab;       

        void OnEnable()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if( savingWrapper == null) { return; }

            print("Loading Saves...");

            foreach(Transform child in contentRoot.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (string saveFile in savingWrapper.ListSaves())
            {
                GameObject buttonInstance = Instantiate(buttonPrefab, contentRoot.transform);
                var textComp = buttonInstance.GetComponentInChildren<TMP_Text>();
                textComp.text = saveFile;

                Button button = buttonInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    savingWrapper.LoadGame(saveFile);
                });
            }
        }      
    }    
}
