using RPG.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI rewardsText;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();

            foreach(Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            foreach(Quest.Objective objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;
                if(status.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                }
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }

            rewardsText.text = GetRewardText(quest);
        }

        string GetRewardText(Quest quest)
        {
            string rewardText = "";
            foreach(var reward in quest.GetRewards())
            {
                if(rewardText != "")
                {
                    rewardText += ", ";
                }
                rewardText += reward.item.GetDisplayName();
                if(reward.number > 1)
                {
                    rewardText += " (" + reward.number + ")" + " ";
                }
            }
            if(rewardText == "")
            {
                rewardText = "No reward";
            }
            rewardText += ".";
            return rewardText;
        }
    }
}
