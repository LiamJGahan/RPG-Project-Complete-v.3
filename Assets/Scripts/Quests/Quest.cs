using GameDevTV.Inventories;
using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG Project/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();

        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
            public bool usesCondition = false;
            public Condition completionCondition;
        }

        [System.Serializable]
        public class Reward
        {
            [Min(1)]
            public int number;
            public InventoryItem item;
        }

        public string GetTitle()
        {
            return name;
        }

        public int GetProgressCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveReference)
        {
            foreach(Objective objective in objectives)
            {
                if(objective.reference == objectiveReference)
                {
                    return true;
                }
            }
            return false;
        }

        public static Quest GetByName(String questName)
        {
            foreach(Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }
}
