using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public TextMeshProUGUI currentQuestText;
    public GameObject questEventPrefab;
    public GameObject questHolder;

    public Quest currentQuest; //The current quest that is being completed.

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Charquests>().questmanager = this;
    }

    public void SetupNewQuest(Quest quest)
    {
        //if (currentQuest != null) { ClearOldQuest(); }
        //else
        {
            currentQuest = quest;
            currentQuest.name = quest.name;
            currentQuest.desc = quest.desc;
            currentQuest.isActive = true;

            //Make loop creating each quest event script and assigning an ID for each quest event.
            foreach (QuestEvent qe in currentQuest.questEvents)
            {
                currentQuest.questEventScripts.Add(CreateQuestEventText(qe).GetComponent<QuestEventScript>());
                //quest.BFS(qe.GetId()); // No longer necessary since order is assigned in inspector now
            }


            int n = 0;
            //Loop setting up each quest object
            foreach (QuestEvent qe in currentQuest.questEvents)
            {
                foreach (GameObject qo in qe.questObjects)
                {
                    qo.GetComponent<QuestObject>().Setup(this, currentQuest.questEvents[n], currentQuest.questEventScripts[n]);
                    n++;
                }
            }
        }
    }

    public void ClearOldQuest()
    {
        //Loop through each quest event and removes them from the list in questEvents
        foreach (QuestEvent qe in currentQuest.questEvents)
        {
            currentQuest.questEventScripts.Remove(CreateQuestEventText(qe).GetComponent<QuestEventScript>());
        }

        //Loop removing each quest object
        foreach (QuestEvent qe in currentQuest.questEvents)
        { 
            foreach (GameObject qo in qe.questObjects)
            {
                qe.questObjects.Remove(qo);
            }
        }

        currentQuest = null;
        currentQuest.name = null;
        currentQuest.desc = null;
        currentQuest.isActive = false;


    }

    GameObject CreateQuestEventText(QuestEvent e) //Creates a new text for each currentQuest event requested
    {
        GameObject b = Instantiate(questEventPrefab, questHolder.transform);
        b.GetComponent<QuestEventScript>().Setup(e, questHolder);
        if (e.order == 1)
        {
            //Assigns the first currentQuest event as current
            e.status = QuestEvent.EventStatus.CURRENT;
        }
        return b;
    }

    private void Update()
    {
        currentQuestText.text = currentQuest.name;
    }

    public void UpdateQuestsOnCompletion(QuestEvent e)
    {
        foreach (QuestEvent n in currentQuest.questEvents)
        {
            //If this event is the next in order
            if (n.order == (e.order + 1))
            {
                //Make the next in line available for completion
                n.UpdateQuestEvent(QuestEvent.EventStatus.CURRENT);
            }
        }
    }
}
