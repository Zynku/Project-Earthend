using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Slider underlaySlider;
    public float healthVal;
    public int sliderValInt, underlayValInt;
    public float underlayDelayTime;
    public float smoothTime, smoothTimeUnderlay;
    public float smoothTargetTime;        //How long the bar takes to smooth between the initial value and the intended value
    public float smoothVel;
    public float smoothLerpSpeed;
    public bool needToUpdateValsOverlay;
    public bool needToUpdateValsUnderlay;

    private void Start()
    {
        //healthVal = GameManager.instance.Player.GetComponent<Charhealth>().currentHealth;
    }

    public void Update()
    {
        smoothLerpSpeed = 3f * Time.deltaTime;
        //SmoothOverlayVals();
        //SmoothUnderlayVals();
    }

    public void SetMaxHealth (int health)
    {
        slider.maxValue = health;
        slider.value = health;

        underlaySlider.maxValue = health;
        underlaySlider.value = health;
    }

    public void SetHealth (int health)  //This whole shit barely works and I barely understand it, I'm angry and upset and I dont give two fucks about this right now. Fuck off
    {
        healthVal = health;
        //smoothTime = smoothTargetTime;
       // smoothTimeUnderlay = smoothTargetTime;
        needToUpdateValsOverlay = true;
        needToUpdateValsUnderlay = true;

        float currentHealthVal = slider.value;
        slider.value = Mathf.Lerp(currentHealthVal, health, smoothLerpSpeed);
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

    public IEnumerator SmoothUnderlayVals()
    {
        yield return new WaitForSeconds(underlayDelayTime);
        if (needToUpdateValsUnderlay)
        {
            underlayValInt = Mathf.FloorToInt(underlaySlider.value);
            //slider.value = Mathf.SmoothDamp(sliderValInt, healthVal, ref smoothVel, smoothTargetTime);

            if (smoothTimeUnderlay > 0)
            {
                smoothTimeUnderlay -= Time.deltaTime;
            }
            underlaySlider.value = Mathf.SmoothStep(underlayValInt, healthVal, smoothTime);

            if (underlayValInt == healthVal) { needToUpdateValsUnderlay = false; }
        }
    }

    public IEnumerator UnderlayDelaySet(int health)
    {
        yield return new WaitForSeconds(underlayDelayTime);
        underlaySlider.value = health;
    }
}
