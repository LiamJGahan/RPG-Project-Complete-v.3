using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] bool isPlayerSpeaking = false; // for multiple people speaking change to enum
        [SerializeField] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] string onEnterAction;
        [SerializeField] string onExitAction;
        [SerializeField] Condition condition;

        public string GetText() { return text; }

        public List<string> GetChildren() { return children; }

        public Rect GetRect() { return rect; }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }

        public string GetOnExitAction()
        {
            return onExitAction;
        }

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }

#if UNITY_EDITOR
        public void AddChild(string childID) 
        {
            Undo.RecordObject(this, "Node Has Been Linked."); // undo will not set dirty on sub assets
            children.Add(childID);
            EditorUtility.SetDirty(this); // must set dirty on sub scriptable object/ sub asset files or will not save to disk
        }

        public void RemoveChild(string childID) 
        {
            Undo.RecordObject(this, "Node Has Been Unlinked.");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText) 
        {
            if(newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text.");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetPosition(Vector2 newPos) 
        {
            Undo.RecordObject(this, "Move Dialogue Node.");
            rect.position = newPos;
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker.");
            isPlayerSpeaking = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
