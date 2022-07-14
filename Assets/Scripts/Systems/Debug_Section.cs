using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[System.Serializable]
public class Debug_Section : MonoBehaviour
{
    private Debugscript debugscript;
    public TextMeshProUGUI debugText;

    public DebugType debugType;

    public enum DebugType
    {
        DeltaTime,
        ComboBuffer,
        PlayerState
    }

    private void Start()
    {
        debugscript = GetComponentInParent<Debugscript>();

        switch (debugType)
        {
            case DebugType.DeltaTime:
                break;
            case DebugType.ComboBuffer:
                break;
            case DebugType.PlayerState:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        switch (debugType)
        {
            case DebugType.DeltaTime:
                if (debugscript.showDeltaTime)
                {
                    debugText.text = $"Delta time is {Time.deltaTime}";
                }
                break;
            case DebugType.ComboBuffer:
                if (debugscript.showComboBuffer)
                {
                    //debugText.text = string.Join(" , ", debugscript.Player.GetComponent<Charanimation>().comboBuffer);
                    debugText.text = string.Join(" , ", debugscript.comboBufferNames);
                }
                break;
            case DebugType.PlayerState:
                if (debugscript.showPlayerState)
                {
                    debugText.text = $"Player state is {debugscript.Player.GetComponent<Charcontrol>().currentState}";
                }
                break;
            default:
                break;
        }
    }
}
