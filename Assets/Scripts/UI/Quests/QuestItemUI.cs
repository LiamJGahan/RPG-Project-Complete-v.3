using RPG.Quests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI Progress;

        QuestStatus status;

        public void Setup(QuestStatus status)
        {
            this.status = status;
            title.text = status.GetQuest().GetTitle();
            Progress.text = status.GetCompletedCount() + "/" + status.GetQuest().GetProgressCount().ToString();
        }

        public QuestStatus GetQuestStatus()
        {
            return status;
        }
    }
}
