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
    public enum QuestState { WAITING, CURRENT, COMPLETED, FAILED};
    public QuestState questState;
    //Enum that defines quest state

    public List<QuestEvent> questEvents = new List<QuestEvent>();
    [SerializeField]  public List<QuestPath> questPaths = new List<QuestPath>();
    [HideInInspector] public List<QuestEventScript> questEventScripts = new List<QuestEventScript>();
    //[HideInInspector] public List<GameObject> questObjects = new List<GameObject>();


    public Quest() 
    {
        //Default values
        name = "Default Quest Name";
        desc = "Go forth! Test this quest and be the best!";
        isActive = false;
    }

    public QuestEvent AddQuestEvent(string n, string d) //Events are goals that need to be completed to progress the quest
    {
        QuestEvent questEvent = new QuestEvent(n, d);
        questEvents.Add(questEvent);
        return questEvent;
    }

    public void AddPath(string fromQuestEvent, string toQuestEvent) //Paths move between Events and link them
    {
        QuestEvent from = FindQuestEvent(fromQuestEvent);
        QuestEvent to = FindQuestEvent(toQuestEvent);

        if (from != null && to != null)
        {
            QuestPath p = new QuestPath(from, to);
            from.pathlist.Add(p);
            questPaths.Add(p);
        }
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

    public void BFS(string id, int orderNumber = 1) //Breadth first search, gives all paths an order so that we can follow them. Order dictates the order in which they can be done. Two quests with the same order means either can be done
    {
        // No longer necessary since order is assigned in inspector now
        QuestEvent thisEvent = FindQuestEvent(id);
        thisEvent.order = orderNumber;

        foreach (QuestPath e in questPaths)
        {
            if (e.endEvent.order == -1)
            {
                BFS(e.endEvent.GetId(), orderNumber + 1);
            }
        }
    }

    public void PrintPath()
    {
        foreach (QuestEvent n in questEvents)
        {
            Debug.Log(n.name + " " + n.order);
        }
    }
}
