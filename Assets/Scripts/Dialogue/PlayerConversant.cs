using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName = "Player";

        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        AIConversant currentAIConversant = null;

        bool isChoosing = false;

        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant newAIConversant, Dialogue newDialogue)
        {
            currentAIConversant = newAIConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

        public void Quit()
        {
            TriggerExitAction();
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            currentAIConversant = null;
            onConversationUpdated();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetCurrentConversantName()
        {
            if(isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentAIConversant.GetNPCName();
            }
        }

        public string GetText()
        {
            if(currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            if(HasNext())
            {
                Next();
            }
            else
            {
                Quit();
            }
        }

        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if(numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, children.Count());

            TriggerExitAction();
            currentNode = children[randomIndex];
            TriggerEnterAction();

            onConversationUpdated();
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNodes)
        {
            foreach(DialogueNode node in inputNodes)
            {
                if(node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        void TriggerEnterAction()
        {
            if(currentDialogue != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        void TriggerExitAction()
        {
            if(currentDialogue != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        void TriggerAction(string action)
        {
            if(action == "") { return; }

            foreach(DialogueTrigger trigger in currentAIConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}
