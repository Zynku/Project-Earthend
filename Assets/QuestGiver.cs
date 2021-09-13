using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public Quest myQuest;
    GameObject player;
    GameObject questManager;
    bool playerInRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        questManager = GameObject.FindGameObjectWithTag("QuestManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            player.GetComponent<Charquests>().currentQuests.Add(myQuest);
            questManager.GetComponent<QuestManager>().SetupNewQuest(myQuest);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GenerateQuestEventID()
    {
        foreach (QuestEvent questevent in myQuest.questEvents)
        {
            questevent.id = Guid.NewGuid().ToString();
        }
    }
}
