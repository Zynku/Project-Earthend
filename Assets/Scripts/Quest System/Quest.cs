using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string name;
    public string desc;
    public bool isActive;

    public List<QuestEvent> questEvents = new List<QuestEvent>();
    [SerializeField] 
    public List<QuestPath> questPaths = new List<QuestPath>();
    public List<QuestEventScript> questEventScripts = new List<QuestEventScript>();
    public List<QuestObject> questObjects = new List<QuestObject>();

    public Quest() 
    {
        //Default values
        name = "Default Quest Name";
        desc = "Go forth! Test this quest and be the best!";
        isActive = true;
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
        QuestEvent thisEvent = FindQuestEvent(id);
        thisEvent.order = orderNumber;

        foreach (QuestPath e in thisEvent.pathlist)
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
