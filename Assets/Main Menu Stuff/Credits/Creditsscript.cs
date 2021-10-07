using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creditsscript : MonoBehaviour
{
    public GameObject credits;
    private RectTransform creditsRect;
    public GameObject creditsBg;
    public GameObject startingMessage;
    public GameObject endingMessage;
    public Vector3 startingPos;
    public Vector3 endingPos;
    public bool startScrolling;
    public bool finishedScrolling;
    private bool musicRestarted = false;
    private float creditsHeight;
    public float lerpFraction = 0;
    public float timeBeforeStartScroll;
    public float scrollSpeed;
    private float scaleMultiplier = 100;
    
    
    // Start is called before the first frame update
    void Start()
    {
        creditsRect = credits.GetComponent<RectTransform>();
        creditsRect.anchoredPosition = startingPos;
        creditsHeight = credits.GetComponent<RectTransform>().sizeDelta.y;
        endingPos = new Vector3(startingPos.x,Mathf.Abs(creditsRect.anchoredPosition.y)+ creditsHeight);
        lerpFraction -= timeBeforeStartScroll / scaleMultiplier * 10;        //Yes this is a magic number, fight me. 10 just works okay?

        creditsBg.SetActive(false);
        startingMessage.SetActive(false);
        endingMessage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    { 
        if (startingMessage.GetComponent<Nonscrollingcreditsscript>().finishedShowing) { startScrolling = true; }
        if (finishedScrolling)
        {
            endingMessage.SetActive(true);
            //startingMessage.GetComponent<Animator>().Play("End");
        }
        if (endingMessage.activeSelf == true && endingMessage.GetComponent<Nonscrollingcreditsscript>().finishedShowing)    //When the ending message is done showing...
        {
            creditsBg.SetActive(false);
            //startScrolling = false;
            if (!musicRestarted) 
            { 
                Mainaudioscript.Instance.PlayAudioClip(0);
                musicRestarted = true;
            }  //Restarts playing the main menu theme when we're done  
        }

        if (startScrolling)
        {
            Debug.Log("Starting to scroll credits...");
            Scroll();
            if (lerpFraction < 1)
            {
                lerpFraction += (Time.deltaTime * (scrollSpeed / scaleMultiplier)); //Moves lerpFraction between <0 and 1
            }
            else
            {
                finishedScrolling = true;
                startScrolling = false;
            }
        }
    }

    public void Scroll()    //Moves the creds between startingPos.y and endingPos.y by lerpFraction, which is expressed between 0 and 1, 0 being start and 1 being end
    {
        creditsRect.anchoredPosition = new Vector2(startingPos.x, Mathf.Lerp(startingPos.y, endingPos.y, lerpFraction));    
    }

    public void StartCredits()  //Resets everything. I really coded this badly didn't I...look at this :(
    {
        finishedScrolling = false;
        startingMessage.GetComponent<Nonscrollingcreditsscript>().finishedShowing = false;
        endingMessage.GetComponent<Nonscrollingcreditsscript>().finishedShowing = false;
        creditsBg.SetActive(true);
        startingMessage.SetActive(true);
        //startingMessage.GetComponent<Animator>().Play("Start");
        endingMessage.SetActive(false);
        Mainaudioscript.Instance.StopAudio();
        musicRestarted = false;
        lerpFraction = 0;
        lerpFraction -= timeBeforeStartScroll / scaleMultiplier * 10;
        creditsRect.anchoredPosition = startingPos;
    }
}
