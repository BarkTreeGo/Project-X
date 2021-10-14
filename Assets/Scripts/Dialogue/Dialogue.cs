using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {

        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] Vector2 newNodeOffset = new Vector2(250, 0);

        Dictionary<string, DialogueNode> nodeLookUp = new Dictionary<string, DialogueNode>();

        private void OnValidate() //called when a value in inspector is changed or when script is loaded
        {
            nodeLookUp.Clear();

            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookUp[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes() 
        //IEmumberables are always foreach-able, even if type is not a lost or an array
        //lists and arrays are in fact IEnumerables
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {           
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookUp.ContainsKey(childID))
                {
                    yield return nodeLookUp[childID];
                }
            }            
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }
#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Creating new Dialogue");
            Undo.RecordObject(this, "Added Dialog Node");
            AddNode(newNode);
        }        

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialog Node");
            nodes.Remove(nodeToDelete);            
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();           

            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetRectPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
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
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
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
