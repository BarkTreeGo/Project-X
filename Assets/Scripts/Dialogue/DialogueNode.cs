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
        [SerializeField] private bool isPlayer = false; //if you want more then two speaker considder using an Enum
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] string onEnterAction;
        [SerializeField] string onExitAction;
        [SerializeField] Condition condition;
        
        
        public string GetNodeText()
        {
            return text;
        }        

        public List<string> GetChildren()
        {
            return children;
        }       

        public Rect GetRect()
        {
            return rect;
        }        

        public bool IsPlayerSpeaking()
        {
            return isPlayer;
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
        public void SetRectPosition(Vector2 newPosition)
        {            
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this); //without this the Undo won't work for making changes to disk for sub-assets
        }

        public void SetText(string newText)
        {            
            if (text != newText)
            {
                Undo.RecordObject(this, "Update Dialog text"); //state to revert back to.
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }
        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add dialogue link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }
        public void RemoveChild(string childID)
        {            
            Undo.RecordObject(this, "Remove dialogue link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayer = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }

        

#endif
    }
}
