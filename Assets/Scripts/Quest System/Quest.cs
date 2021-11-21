using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


//[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/ New Quest")]
[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea(5, 10)]
    public string desc;
    public bool isActive;
    public bool hasTimerForQuest;
    public bool hasTimerForEvent;
    public int whichEventOrderNumber;
    public float timerTargetTime;       //Time that timer starts at before counting down
    public enum QuestState { WAITING, CURRENT, COMPLETED, FAILED};
    public QuestState questState;
    //Enum that defines quest state

    //public List<ScriptableObject> questEvents = new List<ScriptableObject>();
    [SerializeField] public List<QuestEvent> questEvents = new List<QuestEvent>();


    public Quest() 
    {
        //Default values
        questName = "No Quest";
        desc = "No Quest";
        isActive = false;
    }
}

//This script is part of every Quest and holds information about each quest step. It doesn't do anything, only holds information
//and is changed and read by other classes.
[System.Serializable]
public class QuestEvent
{
    public enum EventStatus { WAITING, CURRENT, DONE, FAILED };
    //WAITING - not yet completed but can't be worked on cause there's a prerequisite event
    //CURRENT - the one the player should be trying to achieve
    //DONE - has been achieved

    public string questEventName;
    public string description; //The actual text used to display onscreen indicating what needs to be done
    public string id;
    public int order = -1;
    public EventStatus status;
    [HideInInspector] public QuestEventPrefabScript questEventPrefabScript; //Holds the script associated with this quest event's quest event prefab (try saying that x10 fast)
    public List<QuestObject> questObjects;

    public QuestEvent(string n, string d)
    {
        id = Guid.NewGuid().ToString();
        questEventName = n;
        description = d;
        status = EventStatus.WAITING;
    }
}



