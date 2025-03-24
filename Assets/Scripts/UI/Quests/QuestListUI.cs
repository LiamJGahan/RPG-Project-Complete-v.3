using RPG.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestItemUI questPrefab;

        QuestList questList;

        void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onQuestListUpdate += RedrawQuestListUI;
            RedrawQuestListUI();
        }

        void RedrawQuestListUI()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach(QuestStatus status in questList.GetStatuses())
            {
                QuestItemUI uiInstance = Instantiate(questPrefab, transform);
                uiInstance.Setup(status);
            }
        }
    }
}
