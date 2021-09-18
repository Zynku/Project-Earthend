using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/ New Quest")]
[System.Serializable]
public class Quest
{
    public string name;
    [TextArea(5, 10)]
    public string desc;
    public bool isActive;
    public bool hasTimer;
    public enum QuestState { WAITING, CURRENT, COMPLETED, FAILED};
    public QuestState questState;
    //Enum that defines quest state

    public List<QuestEvent> questEvents = new List<QuestEvent>();
    [HideInInspector] public List<QuestEventScript> questEventScripts = new List<QuestEventScript>();


    public Quest() 
    {
        //Default values
        name = "No Quest";
        desc = "No Quest";
        isActive = false;
    }

    public QuestEvent AddQuestEvent(string n, string d) //Events are goals that need to be completed to progress the quest
    {
        QuestEvent questEvent = new QuestEvent(n, d);
        questEvents.Add(questEvent);
        return questEvent;
    }

    QuestEvent FindQuestEvent(string id)
    {
        foreach (QuestEvent n in questEvents)
        {
            if (n.GetId() == id)
            {
                return n;
            }
        }
        return null;
    }

    public void PrintPath()
    {
        foreach (QuestEvent n in questEvents)
        {
            Debug.Log(n.name + " " + n.order);
        }
    }
}
