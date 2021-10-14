using GameDevTV.Inventories;
using GameDevTV.Saving;
using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new List<QuestStatus>(); //always initialise lists!

        //Events
        public event Action onUpdate;

        private void Update()
        {
            CompleteObjectivesByPredicates();
        }

        private void CompleteObjectivesByPredicates()
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.IsComplete()) { continue; }

                Quest quest = status.GetQuest();
                foreach (var objective in quest.GetObjectives())
                {
                    if (status.IsObjectiveComplete(objective.reference)) { continue; }
                    if (!objective.usesCondition) { continue; }
                    if (objective.completionCondition.Check(GetComponents<IPredicateEvaluator>()))
                    {
                        CompleteObjective(quest, objective.reference);
                    }
                } 
            }            
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) { return; }
            
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            if (onUpdate != null)
            {
                onUpdate();
            }
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                GiveReward(quest);
            }

            if (onUpdate != null)
            {
                onUpdate();
            }
        }       

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }
            return null;
        }

        private void GiveReward(Quest quest)
        {
            Inventory inventory = GetComponent<Inventory>();
            ItemDropper itemDropper = GetComponent<ItemDropper>();

            foreach (var reward in quest.GetRewards())
            {
                bool success = inventory.AddToFirstEmptySlot(reward.item, reward.number);
                if (!success)
                {
                    itemDropper.DropItem(reward.item, reward.number);
                }
            }
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach (QuestStatus status in statuses)
            {
                state.Add(status.CaptureState()); //adds it as QuestStatusRecord
            }
            return state;
        }       

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) return;

            statuses.Clear();
            foreach (object objectState in stateList)
            {                
                statuses.Add(new QuestStatus(objectState));
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {      
            switch (predicate)
            {
                case "HasQuest":
                    return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();
            }
            return null;
        }
    }
}
