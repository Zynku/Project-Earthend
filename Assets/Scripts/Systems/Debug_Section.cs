using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[System.Serializable]
public class Debug_Section : MonoBehaviour
{
    private Debugscript debugscript;
    public Image debugLight;
    public TextMeshProUGUI debugText;


    Charcontrol charcontrol;
    Charanimation charanimation;
    Charattacks charattacks;

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
        charcontrol = GameManager.instance.Player.GetComponent<Charcontrol>();
        charanimation = GameManager.instance.Player.GetComponent<Charanimation>();
        charattacks = GameManager.instance.Player.GetComponent<Charattacks>();
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
                    debugText.text = $"Player state is {charcontrol.currentState}";

                    if (charcontrol.stateChanged)
                    {
                        StopAllCoroutines();
                        StartCoroutine(Lightblink(0.12f));
                    }
                }
                break;
            default:
                break;
        }
    }

    public IEnumerator Lightblink(float duration)
    {
        debugLight.color = Color.green;
        yield return new WaitForSeconds(duration);
        debugLight.color = Color.white;
    }
}
