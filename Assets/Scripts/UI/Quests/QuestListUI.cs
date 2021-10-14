using RPG.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{    
    [SerializeField] QuestItemUI questPrefab;
    [SerializeField] Transform choiceRoot;

    QuestList questList;

    void Start()
    {
        questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();

        //subscribing to events
        questList.onUpdate += Redraw;
        Redraw();
    }

    public void Redraw()
    {        
        foreach (Transform childTransform in choiceRoot)
        {
            Destroy(childTransform.gameObject);
        }
        foreach (QuestStatus status in questList.GetStatuses())
        {
            QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            uiInstance.Setup(status);
        }
    }

}
