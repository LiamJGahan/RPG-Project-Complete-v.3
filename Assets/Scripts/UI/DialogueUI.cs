using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI aiText;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] TextMeshProUGUI currentSpeaker;
        [SerializeField] GameObject aiResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;

        PlayerConversant playerConversant; 

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(() => playerConversant.Next());
            quitButton.onClick.AddListener(() => playerConversant.Quit());

            UpdateUI();
        }

        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) { return; }

            currentSpeaker.text = playerConversant.GetCurrentConversantName();

            aiResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if(playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                aiText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }
            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
