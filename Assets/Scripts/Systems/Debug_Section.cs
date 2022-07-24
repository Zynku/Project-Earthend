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
    public TextMeshProUGUI titleText;
    private Color thisColor;    //Color this gameObject's light changes to.

    Charcontrol charcontrol;
    Charanimation charanimation;
    Charattacks charattacks;

    public DebugType debugType;

    public enum DebugType
    {
        DeltaTime,
        ComboBuffer,
        ComboAttacks,
        AttackInputs,
        PlayerState
    }

    private void Start()
    {
        debugscript = GetComponentInParent<Debugscript>();
        charcontrol = GameManager.instance.Player.GetComponent<Charcontrol>();
        charanimation = GameManager.instance.Player.GetComponent<Charanimation>();
        charattacks = GameManager.instance.Player.GetComponent<Charattacks>();
        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    private void Update()
    {
        titleText.text = debugType.ToString();
        switch (debugType)
        {
            case DebugType.DeltaTime:
                debugText.text = $"Delta time is {Time.deltaTime}";
                break;
            case DebugType.ComboBuffer:
                thisColor = Color.red;
                charanimation.comboBufferCleared += DoLightBlink;
                //debugText.text = string.Join(" , ", debugscript.Player.GetComponent<Charanimation>().comboBuffer);
                string comboBufferString = string.Join(" , ", debugscript.comboBufferNames);
                if (comboBufferString.Length > 0)
                {
                    debugText.text = comboBufferString;
                }
                else
                {
                    debugText.text = $" -~- ";
                }
                break;
            case DebugType.ComboAttacks:
                thisColor = Color.red;
                charattacks.attacksCleared += DoLightBlink;
                string comboAttackString = string.Join(" , ", debugscript.comboAttackNames);
                if (comboAttackString.Length > 0)
                {
                    debugText.text = comboAttackString;
                }
                else
                {
                    debugText.text = $" -~- ";
                }
                break;
            case DebugType.AttackInputs:
                thisColor = Color.yellow;
                charattacks.attackInputRegistered += DoLightBlink;
                debugText.text = $" -~- ";
                break;
            case DebugType.PlayerState:
                debugText.text = $"Player state is {charcontrol.currentState}";
                if (charcontrol.stateChanged)
                {
                    StopAllCoroutines();
                    StartCoroutine(Lightblink(0.12f, Color.green));
                }
                break;
            default:
                break;
        }
    }

    public void DoLightBlink() //This method essentially calls the LightBlink coroutine, but can subscribe to events since it doesn't have arguments
    {
        StartCoroutine(Lightblink(0.15f, thisColor));
    }

    public IEnumerator Lightblink(float duration, Color blinkColor)
    {
        debugLight.color = blinkColor;
        yield return new WaitForSeconds(duration);
        debugLight.color = Color.white;
    }
}
