using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestEvent
{
    public enum EventStatus { WAITING, CURRENT, DONE, FAILED };
    //WAITING - not yet completed but can't be worked on cause there's a prerequisite event
    //CURRENT - the one the player should be trying to achieve
    //DONE - has been achieved

    public string name; //NOT USED;
    public string description; //The actual text used to display onscreen indicating what needs to be done
    public string id;
    public int order = -1;
    public EventStatus status;

    [NonSerialized]public List<QuestPath> pathlist = new List<QuestPath>();
    //public GameObject[] questObjects;
    public List<GameObject> questObjects;

    public QuestEvent(string n, string d)
    {
        id = Guid.NewGuid().ToString();
        name = n;
        description = d;
        status = EventStatus.WAITING;
    }

    public void UpdateQuestEvent(EventStatus es)
    {
        status = es;
    }

    public string GetId()
    {
        return id;
    }
}
