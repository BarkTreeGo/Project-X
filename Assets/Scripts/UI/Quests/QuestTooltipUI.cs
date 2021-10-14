using RPG.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI rewardsText;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;               

        public void Setup(QuestStatus status)
       {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();        
            rewardsText.text = GetRewardText(quest);

            foreach (Transform childTransform in objectiveContainer)
            {
                Destroy(childTransform.gameObject);
            }

            foreach (var objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;
                
                if (status.IsObjectiveComplete(objective.reference))
                {
                    prefab = objectivePrefab;
                }
                
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            }
       }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "";

            foreach (var reward in quest.GetRewards())
            {
                if (rewardText != "")
                {
                    rewardText += ", ";
                }
                if (reward.number > 1)
                {
                    rewardText += reward.number + " ";
                }
                rewardText += reward.item.GetDisplayName();
            }
            if(rewardText == "")
            {
                rewardText = "No reward";
            }
            rewardText += ".";
            return rewardText;
        }
    }
}
