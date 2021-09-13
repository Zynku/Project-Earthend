using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class QuestManager : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public TextMeshProUGUI currentQuestText;
    public GameObject questEventPrefab;
    public List<GameObject> questEventPrefabs;
    public GameObject questHolder;
    public GameObject questCompletedText;
    public GameObject questCompletedName;
    public GameObject questAcceptedText;
    public GameObject questAcceptedName;

    public Quest currentQuest; //The current quest that is being completed.
    public GameObject questName;
    public GameObject questDesc;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Charquests>().questmanager = this;
    }

    private void Start()
    {
        questCompletedText.SetActive(false);
        questAcceptedText.SetActive(false);
    }

    public void SetupNewQuest(Quest quest)
    {
        if (currentQuest != null) { ClearOldQuest(); }
        {
            currentQuest = quest;
            currentQuest.questState = Quest.QuestState.CURRENT;
            foreach (QuestEvent qe in currentQuest.questEvents)
            {
                qe.status = QuestEvent.EventStatus.WAITING;
            }
            currentQuest.questEvents.First().status = QuestEvent.EventStatus.CURRENT;
            currentQuest.isActive = true;
            questName.GetComponent<TextMeshProUGUI>().text = currentQuest.name;
            questDesc.GetComponent<TextMeshProUGUI>().text = currentQuest.desc;

            questAcceptedText.SetActive(true);
            questAcceptedName.GetComponent<TextMeshProUGUI>().text = currentQuest.name;

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

    GameObject CreateQuestEventText(QuestEvent e) //Creates a new text for each currentQuest event requested
    {
        GameObject b = Instantiate(questEventPrefab, questHolder.transform);
        b.GetComponent<QuestEventScript>().Setup(e, questHolder);
        questEventPrefabs.Add(b);
        /*if (e.order == 1)
        {
            //Assigns the first currentQuest event as current
            e.status = QuestEvent.EventStatus.CURRENT;
        }
        */
        return b;
    }


    public void ClearOldQuest()
    {
        foreach (GameObject qep in questEventPrefabs)
        {
            Destroy(qep);
        }
        questEventPrefabs.Clear();

        questName.GetComponent<TextMeshProUGUI>().text = null;
        questDesc.GetComponent<TextMeshProUGUI>().text = null;
        currentQuest.isActive = false;
        currentQuest = null;
    }

    private void ReAddQuest(Quest quest)
    {
        currentQuest = quest;
        currentQuest.questState = Quest.QuestState.CURRENT;
        foreach (QuestEvent qe in currentQuest.questEvents)
        {
            qe.status = QuestEvent.EventStatus.WAITING;
        }
        currentQuest.questEvents.First().status = QuestEvent.EventStatus.CURRENT;
        currentQuest.isActive = true;
        questName.GetComponent<TextMeshProUGUI>().text = currentQuest.name;
        questDesc.GetComponent<TextMeshProUGUI>().text = currentQuest.desc;

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

    private void Update()
    {
        
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

        if (currentQuest.questEvents.Last().status == QuestEvent.EventStatus.DONE)
        {
            CompleteCurrentQuest();
        }
    }

    public void CompleteCurrentQuest()
    {
        questCompletedText.SetActive(true);
        questCompletedName.GetComponent<TextMeshProUGUI>().text = currentQuest.name;

        foreach (GameObject qep in questEventPrefabs)
        {
            Destroy(qep);
        }
        questEventPrefabs.Clear();

        questName.GetComponent<TextMeshProUGUI>().text = null;
        questDesc.GetComponent<TextMeshProUGUI>().text = null;
        currentQuest.isActive = false;
        player.GetComponent<Charquests>().currentQuests.Remove(currentQuest);
        currentQuest.questState = Quest.QuestState.COMPLETED;

        currentQuest = null;

        if (player.GetComponent<Charquests>().currentQuests.Count != 0)
        {
            ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }

}
