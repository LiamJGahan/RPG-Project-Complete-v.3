using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(menuName = ("Dialogue"))]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] Vector2 newNodeOffset = new Vector2(250f,0f);
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();


        void Awake()
        {
            OnValidate();
        }

        void OnValidate()
        {
            nodeLookUp.Clear();
            foreach(DialogueNode node in GetAllNodes())
            {
                if (node != null)
                {
                    nodeLookUp[node.name] = node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {           
            foreach(string childID in parentNode.GetChildren())
            {
                if(nodeLookUp.ContainsKey(childID))
                {
                    yield return nodeLookUp[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if(!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);

            Undo.RegisterCompleteObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialouge Node.");

            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialouge Node.");

            nodes.Remove(nodeToDelete);

            OnValidate();

            cleanUpParentedLinks(nodeToDelete);

            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();



            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }
        void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);

            OnValidate();
        }

        void cleanUpParentedLinks(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);

                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach(DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
