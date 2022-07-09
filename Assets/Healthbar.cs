using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public float healthVal;
    public int sliderValInt;
    public float smoothTime, smoothTimeUnderlay;
    public float smoothTargetTime;        //How long the bar takes to smooth between the initial value and the intended value
    public float smoothVel;
    public bool needToUpdateValsOverlay;

    private void Start()
    {
        //healthVal = GameManager.instance.Player.GetComponent<Charhealth>().currentHealth;
    }

    public void Update()
    {
        SmoothOverlayVals();
    }

    public void SetMaxHealth (int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth (int health)  //This whole shit barely works and I barely understand it, I'm angry and upset and I dont give two fucks about this right now. Fuck off
    {
        healthVal = health;
        smoothTime = smoothTargetTime;
        smoothTimeUnderlay = smoothTargetTime;
        needToUpdateValsOverlay = true;
    }

    public void SmoothOverlayVals()
    {
        if (needToUpdateValsOverlay)
        {
            sliderValInt = Mathf.FloorToInt(slider.value);
            //slider.value = Mathf.SmoothDamp(sliderValInt, healthVal, ref smoothVel, smoothTargetTime);

            if (smoothTime > 0)
            {
                smoothTime -= Time.deltaTime;
            }
            slider.value = Mathf.SmoothStep(sliderValInt, healthVal, smoothTime);

            if (sliderValInt == healthVal) { needToUpdateValsOverlay = false; }
        }
    }
}
