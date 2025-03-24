using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        [SerializeField] Dialogue selectedDialouge = null;
        [SerializeField] Vector2 scrollPosition;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;

        const float canvasSize = 4000f;
        const float backgroundSize = 50f;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
           
            if(dialogue != null) 
            {
                ShowEditorWindow();
                return true; 
            }

            return false;
        }

        void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        void OnSelectionChanged()
        {
            Dialogue newDialouge = Selection.activeObject as Dialogue;
            if(newDialouge != null)
            {
                selectedDialouge = newDialouge;
                Repaint();
            }

        }

        void OnGUI()
        {
            if(selectedDialouge == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCords);

                foreach(DialogueNode node in selectedDialouge.GetAllNodes())
                {
                    if(node.GetChildren() == null) { continue; }
                    DrawConnections(node);
                }
                foreach(DialogueNode node in selectedDialouge.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if(creatingNode != null)
                {
                    selectedDialouge.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if(deletingNode != null)
                {
                    selectedDialouge.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        void ProcessEvents()
        {
            if(Event.current.commandName == "UndoRedoPerformed") // force GUI to repaint when undo is used on nodes
            {
                GUI.changed = true;
            }
            if(Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);

                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialouge;
                }
            }
            else if(Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseDrag && draggingCanvas == true)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;

            if(node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }

            GUILayout.BeginArea(node.GetRect(), style);

            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-"))
            {
                deletingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if(linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y); // This is outside the foreach loop because the parent never changes

            foreach (DialogueNode childNode in selectedDialouge.GetAllChildren(node))
            {

                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0f;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.white, null, 5f);
            }
        }


        DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode lastFoundNode = null;

            foreach (DialogueNode node in selectedDialouge.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    lastFoundNode = node;
                }
            }
            return lastFoundNode;
        }
    }
}
