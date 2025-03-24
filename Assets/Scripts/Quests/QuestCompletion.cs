using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] string objective;

        QuestList questList;

        public void CompleteObjective()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            if (!questList.HasQuest(quest)) { return; }
            questList.CompleteObjective(quest, objective);
        }
    }
}
