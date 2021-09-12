using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestEventScript : MonoBehaviour //Really should be called QuestEventText
{
    public TextMeshProUGUI currentEventText;
    public int order;
    public Image Square;
    public Image Tick;
    public Image X;
    public Image Question;
    public Image Exclamation;
    public QuestEvent thisEvent;
    public GameObject questHolder;

    public QuestEvent.EventStatus status;

    public void Setup(QuestEvent e, GameObject questHolder)
    {
        thisEvent = e;
        status = thisEvent.status;
        currentEventText.text = thisEvent.description;
        order = thisEvent.order;
        //gameObject.transform.SetParent
        //Come back to this if, when the text is instantiated, it isnt set as child of questHolder object
    }

    private void Update()
    {
        status = thisEvent.status;
        if (status == QuestEvent.EventStatus.DONE)
        {
            Tick.gameObject.SetActive(true);
            X.enabled = false;
            Question.enabled = false;
            Exclamation.enabled = false;

            currentEventText.color = new Color32(255, 255, 255, 105);
        }

        if (status == QuestEvent.EventStatus.FAILED)
        {
            Tick.enabled = false;
            X.gameObject.SetActive(true);
            Question.enabled = false;
            Exclamation.enabled = false;

            currentEventText.color = new Color32(255, 255, 255, 105);
        }
    }

    /*public void UpdateElement(QuestEvent.EventStatus s)
    {
        //status = s;
        if (status == QuestEvent.EventStatus.DONE)
        {
            Tick.enabled = true;
            X.enabled = false;
            Question.enabled = false;
            Exclamation.enabled = false;

            currentEventText.color = new Color32(255, 255, 255, 105);
        }

        if (status == QuestEvent.EventStatus.FAILED)
        {
            Tick.enabled = false;
            X.enabled = true;
            Question.enabled = false;
            Exclamation.enabled = false;

            currentEventText.color = new Color32(255, 255, 255, 105);
        }
    }
    */
}
