using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class QuestGiver : MonoBehaviour
{
    public Quest myQuest;
    GameObject player;
    GameObject questManager;
    bool playerInRange;
    public GameObject shownQuestNameText;
   
    //QuestGiver script is not responsible for dictating the type of quest this is, only the questObject that is referenced in this Quest.

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        questManager = GameObject.FindGameObjectWithTag("QuestManager");
        shownQuestNameText.GetComponent<TextMeshPro>().text = myQuest.name;
    }


    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            //If the quest you're trying to add is not the same as the quest that is already active, or if there are no quests
            //if(questManager.GetComponent<QuestManager>().currentQuest.name == myQuest.name)
            if (CheckforSameQuest(myQuest))
            {
                Debug.Log("Quest already accepted!");
            }
            else
            {
                player.GetComponent<Charquests>().currentQuests.Add(myQuest);
                questManager.GetComponent<QuestManager>().SetupNewQuest(myQuest);
            }
        }
    }

    bool CheckforSameQuest(Quest myQuest)
    {
        foreach (Quest quest in player.GetComponent<Charquests>().currentQuests)
        {
            if (quest.name == myQuest.name)
            {
                return true;
            }
        }
        return false;
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
