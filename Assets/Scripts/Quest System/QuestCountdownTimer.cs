using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestCountdownTimer : MonoBehaviour
{
    [HideInInspector] public float rawTimer = 0f; //TODO: Assign this value from quest manager only if the quest event has a timer enabled
    public TextMeshProUGUI shownTimer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FormatTimer();
    }

    private void FormatTimer()
    {
        int days = (int)(rawTimer / 86400) % 365;
        int hours = (int)(rawTimer / 3600) % 24;
        int minutes = (int)(rawTimer / 60) % 60;
        int seconds = (int)(rawTimer % 60);

        shownTimer.text = ""; 
        if (days > 0) { shownTimer.text += days + "d "; }
        if (hours > 0) { shownTimer.text += hours + "h "; }
        if (minutes > 0) { shownTimer.text += minutes + "m "; }
        if (seconds > 0) { shownTimer.text += seconds + "s "; }
    }
}
