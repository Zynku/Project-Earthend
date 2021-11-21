using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestGiver : MonoBehaviour
{
    public bool acceptQuestByProximity = false;         //Can you just walk up to this NPC and get a quest?
    public Quest myQuest;

    GameObject player;
    gamemanager gamemanager;
    QuestManager questManager;
    QuestManager questManagerScript;
    bool playerInRange;
    public GameObject shownQuestNameText;

    //QuestGiver script is not responsible for dictating the type of quest this is, only the questObject that is referenced in this Quest.

    private void Start()
    {
        gamemanager = gamemanager.instance;
        player = gamemanager.Player;
        questManager = gamemanager.questManager;
        shownQuestNameText.GetComponent<TextMeshPro>().text = myQuest.questName;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (playerInRange && Input.GetButtonDown("Interact") && acceptQuestByProximity)
        {
            if (myQuest == null)
            {
                Debug.LogWarning("Quest Giver Script on " + gameObject + " has no associated quest!");
            }
            else
            {
                StartCoroutine(AcceptQuest(myQuest));
            }
        }
    }

    public IEnumerator AcceptQuest(Quest quest)
    {
        //If the quest you're trying to add is not the same as the quest that is already active, or if there are no quests
        //if(questManager.GetComponent<QuestManager>().currentQuest.name == myQuest.name)
        if (CheckforSameQuest(quest))
        {
            Debug.LogWarning("Quest already accepted!");
        }
        else if (questManager.canAcceptQuest)
        {
            player.GetComponent<Charquests>().currentQuests.Add(quest);
            questManager.GetComponent<QuestManager>().SetupNewQuest(quest);
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    bool CheckforSameQuest(Quest myQuest)
    {
        if (player.GetComponent<Charquests>().currentQuests.Count <= 0)
        {
            return false;
        }
        else
        {
            foreach (Quest quest in player.GetComponent<Charquests>().currentQuests)
            {
                if (quest.questName == myQuest.questName)
                {
                    return true;
                }
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

