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

    private void Update()
    {
/*        if (Input.GetKeyDown(KeyCode.B))
        {
            CreateAutoTimer(69, "Testname", true, true);
        }*/
    }

    public GameObject CreateTimer(float timerTime, string timerName, bool deleteOnEnd)    //TIMER IS NOT IN CHARGE OF CALCULATING ITS TIME. IF YOU WANT THAT USE THE METHOD BELOW
    {
        GameObject timer = Instantiate(timerPrefab, gameObject.transform);
        SimpleTimerScript timerScript = timer.GetComponent<SimpleTimerScript>();
        timerScript.timerTime = timerTime;
        timerScript.timerName.text = timerName;
        timerScript.deleteOnEnd = deleteOnEnd;
        timers.Add(timer);

/*        foreach (GameObject timtam in timers)
        {
            if (timerScript.timerName == timtam.GetComponent<SimpleTimerScript>().timerName)
            {
                Debug.Log("Timer already exists");
                timers.Remove(timer);
                Destroy(timer);
            }
        }*/

        return timer;
    }

    public GameObject CreateAutoTimer(float timerTime, string timerName, bool deleteOnEnd, bool startTimer)    //This timer is in charge of its own time
    { 
        GameObject timer = Instantiate(timerPrefab, gameObject.transform);
        SimpleTimerScript timerScript = timer.GetComponent<SimpleTimerScript>();
        timerScript.timerTime = timerTime;
        timerScript.timerName.text = timerName;
        timerScript.deleteOnEnd = deleteOnEnd;
        timerScript.calculateOwnTime = true;
        timerScript.startTimer = startTimer;
        timers.Add(timer);

        return timer;
    }
}
