using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEvent
{
    public enum EventStatus { WAITING, CURRENT, DONE, FAILED };
    //WAITING - not yet completed but can't be worked on cause there's a prerequisite event
    //CURRENT - the one the player should be trying to achieve
    //DONE - has been achieved

    public string name;
    public string description; //The actual text used to display onscreen indicating what needs to be done
    public string id;
    public int order = -1;
    public EventStatus status;

    public List<QuestPath> pathlist = new List<QuestPath>();

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
