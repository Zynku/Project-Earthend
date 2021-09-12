using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public Quest myQuest;
    GameObject player;
    GameObject questManager;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        questManager = GameObject.FindGameObjectWithTag("QuestManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            player.GetComponent<Charquests>().currentQuests.Add(myQuest);
            questManager.GetComponent<QuestManager>().SetupNewQuest(myQuest);
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
