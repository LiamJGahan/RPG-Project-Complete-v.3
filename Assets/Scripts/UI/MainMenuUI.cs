using GameDevTV.Utils;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField newGameNameField;

        LazyValue<SavingWrapper> savingWrapper;

        void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }

        SavingWrapper GetSavingWrapper()
        {
            return savingWrapper.value = FindObjectOfType<SavingWrapper>();
        }

        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }

        public void NewGame()
        {
            if (!string.IsNullOrEmpty(newGameNameField.text))
            {
                savingWrapper.value.NewGame(newGameNameField.text);
            }
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

}
