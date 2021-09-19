using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestCountdownTimer : MonoBehaviour    //This script and its gameObject are only responsible for SHOWING the timer onscreen, not calculating it
{
    [HideInInspector] public float rawTimer = 0f; //TODO: Assign this value from quest manager only if the quest event has a timer enabled
    public TextMeshProUGUI shownTimer;
}
