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

    Charcontrol charcontrol;
    Charanimation charanimation;
    Charattacks charattacks;

    public DebugType debugType;

    public enum DebugType
    {
        DeltaTime,
        ComboBuffer,
        ComboAttacks,
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
                if (debugscript.showDeltaTime)
                {
                    debugText.text = $"Delta time is {Time.deltaTime}";
                }
                break;
            case DebugType.ComboBuffer:
                if (debugscript.showComboBuffer)
                {
                    charanimation.comboBufferCleared += LightBlinkEvents;
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
                        StartCoroutine(Lightblink(0.12f, Color.green));
                    }
                }
                break;
            case DebugType.ComboAttacks:
                charattacks.attacksCleared += LightBlinkEvents;
                debugText.text = string.Join(" , ", debugscript.comboAttackNames);
                break;
            default:
                break;
        }
    }

    public void LightBlinkEvents() //This method essentially calls the LightBlink coroutine, but can subscribe to events since it doesn't have arguments
    {
        StartCoroutine(Lightblink(0.15f, Color.red));
    }

    public IEnumerator Lightblink(float duration, Color blinkColor)
    {
        debugLight.color = blinkColor;
        yield return new WaitForSeconds(duration);
        debugLight.color = Color.white;
    }
}
