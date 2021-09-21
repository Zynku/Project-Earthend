using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTimer : MonoBehaviour
{
    private float rawTimer = 99999999f; //Actual timer time, changes with time.deltatime and counts down to 0. Is automatically set in Start(), dont manually set
    public float timerTargetTime;       //Time that timer starts at before counting down
    public string formattedTime;
    public bool timerDone = false;
    public Timertype TypeofTimer;
    public Quest myQuest;
    public QuestEvent myQuestEvent;

    public enum Timertype
    {
        QUEST,
        EVENT
    }

    private void Start()
    {
        rawTimer = timerTargetTime;
    }

    void Update()
    {
        rawTimer -= Time.deltaTime;
        if(rawTimer <= 1) 
        { 
            rawTimer = 1;
            timerDone = true;
        }
        FormatTimer();
    }

    private void FormatTimer()
    {
        int days = (int)(rawTimer / 86400) % 365;
        int hours = (int)(rawTimer / 3600) % 24;
        int minutes = (int)(rawTimer / 60) % 60;
        int seconds = (int)(rawTimer % 60);

        formattedTime = "";
        if (days > 0) { formattedTime += days + "d "; }
        if (hours > 0) { formattedTime += hours + "h "; }
        if (minutes > 0) { formattedTime += minutes + "m "; }
        if (seconds > 0) { formattedTime += seconds + "s "; }
    }
}
