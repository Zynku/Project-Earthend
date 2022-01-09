using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public bool showAllTimers = true;
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

    //In order to make an onscreen timer, call TimerManager.instance.CreateTimer(timerTime, timerTargetTime, "timerName", false) in Start() and assign the variables in brackets as arguments
    //You also must continually assign timerTime in the Update() function from wherever you called CreateTimer() as it needs a live reference to count down.
    //Non-auto timers need to do timerTime -= time.DeltaTime from wherever it is instantiated. In that regard, this script's only job is to display said timer, not to count.
    //Stop making that mistake lol.

    private void Update()
    {
        if (showAllTimers) { ShowTimers(); }
        if (!showAllTimers) { HideTimers(); }

    }

    private void ShowTimers()
    {
        foreach (GameObject timer in timers)
        {
            timer.SetActive(true);
        }
    }

    private void HideTimers()
    {
        foreach (GameObject timer in timers)
        {
            timer.SetActive(false);
        }
    }

    public GameObject CreateTimer(float timerTime, float timerTargetTime, string timerName, bool deleteOnEnd)    //TIMER IS NOT IN CHARGE OF CALCULATING ITS TIME. IF YOU WANT THAT USE AUTO TIMER
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
