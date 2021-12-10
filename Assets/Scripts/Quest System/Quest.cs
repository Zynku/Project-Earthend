using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;


//[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/ New Quest")]
[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
public class Quest : ScriptableObject
{ 
    public string questName;
    [TextArea(5, 10)]
    public string questDesc;
    public bool isActive;
    public bool hasTimerForQuest;
    [ConditionalField(nameof(hasTimerForQuest))] public float questTimerTargetTime;       //Time that timer starts at before counting down
    public List<GameObject> associatedQuestGivers;                                        //Do any NPCs give you this quest via dialogue?
    
    public enum QuestState { WAITING, CURRENT, COMPLETED, FAILED};
    public QuestState questState;
    
    [SerializeField] public List<QuestEvent> questEvents = new List<QuestEvent>();

    public Quest() 
    {
        //Default values
        questName = "No Quest";
        questDesc = "No Quest Description";
        isActive = false;
    }

    [ButtonMethod]
    [SerializeField]
    public void ResetQuest()
    {
        isActive = false;
        questState = Quest.QuestState.WAITING;
        foreach (var questEvent in questEvents)
        {
            questEvent.status = QuestEvent.EventStatus.WAITING;
            foreach (var questLogic in questEvent.questLogic)
            {
                questLogic.status = QuestEvent.EventStatus.WAITING;
                questLogic.moneyCounter = 0;
                questLogic.healthCounter = 0;
                questLogic.itemCounter = 0;
                questLogic.levelCounter = 0;
            }
        }
    }

    [ButtonMethod]
    [SerializeField]
    public void AutoGenerateOrders()
    {
        for (int i = 0; i < questEvents.Count; i++)
        {
            questEvents[i].order = i;
        }
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
    public int order = -1;
    public bool failWholeQuest; //If this quest event is failed, then the whole quest fails. If not, it can be failed, and the quest can still be passed with it's other events
    public EventStatus status;
    [HideInInspector] public QuestEventPrefabScript questEventPrefabScript; //Holds the script associated with this quest event's quest event prefab (try saying that x10 fast)
    public List<QuestLogic> questLogic;

    public QuestEvent(string n, string d)
    {
        questEventName = n;
        description = d;
        status = EventStatus.WAITING;
    }
}



