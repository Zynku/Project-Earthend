using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public float healthVal;
    [ReadOnly] public float smoothTimer;  //Actual timer for smooth time         
    public float smoothTargetTime;        //How long the bar takes to smooth between the initial value and the intended value
    public float updateDelay;              //The delay between receiving a new value, and actually updating the bar. Used for health bar underlay

    public void SetMaxHealth (int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth (int health)
    {
        Debug.Log("Updating health vals");
        healthVal = health;
        StopAllCoroutines();
        StartCoroutine(SmoothValues(Mathf.FloorToInt(slider.value), health));
    }

    public IEnumerator SmoothValues(int startHealth, int endHealth)
    {
        yield return new WaitForSeconds(updateDelay);
        smoothTimer = 0;
        while (smoothTimer <= smoothTargetTime)
        {
            //Debug.Log("Smooth health values");
            smoothTimer += Time.deltaTime;
            float timer = smoothTimer / smoothTargetTime;
            float currentValue = Mathf.SmoothStep(startHealth, endHealth, timer);
            slider.value = currentValue;
            yield return null;
        }
    }
}
