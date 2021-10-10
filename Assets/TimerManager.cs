using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public GameObject timerPrefab;
    public List<GameObject> timers;
    public static TimerManager instance;

    #region Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Timer Manager found!");
            return;
        }
        instance = this;
    }
    #endregion

    //In order to make an onscreen timer, call TimerManager.instance.CreateTimer(timerTime, timerTargetTime, "timerName", false) and assign the variables in brackets as arguments
    //You also must continually assign timerTime in the Update() function from wherever you called CreateTimer() as it needs a live reference to count down.

    public GameObject CreateTimer(float timerTime, float timerTargetTime, string timerName, bool deleteOnEnd)    //TIMER IS NOT IN CHARGE OF CALCULATING ITS TIME. IF YOU WANT THAT USE THE METHOD BELOW
    {
        GameObject timer = Instantiate(timerPrefab, gameObject.transform);
        SimpleTimerScript timerScript = timer.GetComponent<SimpleTimerScript>();
        timerScript.timerTime = timerTime;
        timerScript.timerTargetTime = timerTargetTime;
        timerScript.timerName.text = timerName;
        timerScript.deleteOnEnd = deleteOnEnd;
        timers.Add(timer);

        return timer;
    }

    public GameObject CreateAutoTimer(float timerTime, float timerTargetTime, string timerName, bool deleteOnEnd, bool startTimer)    //This timer is in charge of its own time
    { 
        GameObject timer = Instantiate(timerPrefab, gameObject.transform);
        SimpleTimerScript timerScript = timer.GetComponent<SimpleTimerScript>();
        timerScript.timerTime = timerTime;
        timerScript.timerTargetTime = timerTargetTime;
        timerScript.timerName.text = timerName;
        timerScript.deleteOnEnd = deleteOnEnd;
        timerScript.calculateOwnTime = true;
        timerScript.startTimer = startTimer;
        timers.Add(timer);

        return timer;
    }
}
