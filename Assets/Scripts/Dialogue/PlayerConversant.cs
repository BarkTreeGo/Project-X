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
        [SerializeField] string playerName;
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        AIConversant currenConversant = null;
        bool isChoosing = false;

        //Events
        public event Action onConversationUpdated;
        private void Awake()
        {
            //currentNode = testDialogue.GetRootNode();
        }

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currenConversant = newConversant;
            currentDialogue = newDialogue;            
            currentNode = currentDialogue.GetRootNode();            
            TriggerEnterAction();
            onConversationUpdated();            
        }

        public void Quit()
        {            
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currenConversant = null;
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

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }
            return currentNode.GetNodeText();            
        }       
        
        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));            
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currenConversant.GetName();
            }
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponse = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count(); //number of player response options
            if (numPlayerResponse > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }
            
            //if no player response options, continue with dialogie of the AI.
            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, children.Count());
            TriggerExitAction();
            currentNode = children[randomIndex];
            TriggerEnterAction();
            onConversationUpdated();           
        }

        public bool hasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNodes)
        {
            foreach (var node in inputNodes)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());               
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());                
            }
        }
        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach (DialogueTrigger trigger in currenConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}
