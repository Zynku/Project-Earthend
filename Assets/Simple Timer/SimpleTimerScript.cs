using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleTimerScript : MonoBehaviour
{
    [SerializeField] private Image uiFill;
    [SerializeField] private TextMeshProUGUI timerText;
    public float timerTime;
    public float timerTargetTime;
    public bool calculateOwnTime;
    public TextMeshProUGUI timerName;
    public bool deleteOnEnd = false;
    public bool startTimer = true;
    private TimerManager timerManager;

    // Start is called before the first frame update
    void Start()
    {
        timerTargetTime = timerTime;
        uiFill.color = Random.ColorHSV(0f,1f,1f,1f,0.5f,1f,1f,1f);
        timerManager = TimerManager.instance;
    }


    // Update is called once per frame
    void Update()                   //TIMER SHOULD NOT BE IN CHARGE OF CALCULATING TIME. THAT SHOULD BE DONE FROM WHEREVER IT IS INSTANTIATED
    {
        timerText.text = Mathf.Round(timerTime).ToString();
        if (startTimer && calculateOwnTime) { timerTime -= Time.deltaTime; }
        if(timerTime < 0)
        { 
            timerTime = 0f; 
            if (deleteOnEnd)
            {
                timerManager.timers.Remove(gameObject);
                Destroy(gameObject);
            }
        }
        uiFill.fillAmount = timerTime / timerTargetTime;
    }
}
