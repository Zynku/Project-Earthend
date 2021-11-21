using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//This is the script that holds references and information about the Quest Event Prefab that is instantiated from the Quest Manager
//This is attached to each Quest Event Prefab
[System.Serializable]
public class QuestEventPrefabScript : MonoBehaviour
{
    public TextMeshProUGUI currentEventText;
    public Vector3 eventTextTransform;
    public Vector3 squareOffset;
    public int order;
    public Image Square;
    public Image Tick;
    public Image X;
    public Image Question;
    public Image Exclamation;
    public QuestEvent thisEvent;
    public GameObject questHolder;
    [HideInInspector] public GameObject questEventPrefab;
    public QuestEvent.EventStatus status;

    private void Start()
    {
        eventTextTransform = gameObject.GetComponent<RectTransform>().position;
    }

    public void Setup(QuestEvent e, GameObject questHolder, GameObject questEventPrefab)
    {
        questEventPrefab = this.gameObject;
        thisEvent = e;
        status = thisEvent.status;
        currentEventText.text = thisEvent.description;
        order = thisEvent.order;
        //gameObject.transform.SetParent
        //Come back to this if, when the text is instantiated, it isnt set as child of questHolder object
    }

    private void Update()
    {
        Square.GetComponent<RectTransform>().localPosition = squareOffset;

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
}
