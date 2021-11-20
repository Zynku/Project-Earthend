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
    public bool hasTimerForQuest;
    public bool hasTimerForEvent;
    public int whichEventOrderNumber;
    public float timerTargetTime;       //Time that timer starts at before counting down
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
}
