using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{   
    public class QuestStatus //not deriving from monobehavior, but also not a scriptable object
    {
        Quest quest;
        List<string> completedObjectives = new List<string>(); //always initialise lists!

        [System.Serializable]
        public class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public object CaptureState()
        {
            QuestStatusRecord state = new QuestStatusRecord();
            state.questName = quest.name;
            state.completedObjectives = completedObjectives;
            return state;
        }

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            quest =  Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }

        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if (!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective) )
            {
                completedObjectives.Add(objective);
            }
        }
    }
}
