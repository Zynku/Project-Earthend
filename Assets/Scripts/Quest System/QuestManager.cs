using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI currentQuestText;
    public GameObject questEventPrefab;
    public GameObject questHolder;

    public Quest currentQuest; //The current quest that is being completed.

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Charquests>().questmanager = this;
    }

    public void Start()
    {
        
    }

    public void SetupNewQuest(Quest quest)
    {
        currentQuest = quest;
        currentQuest.name = quest.name;
        currentQuest.desc = quest.desc;
        currentQuest.isActive = true;
        //player.GetComponent<Charquests>().currentQuests.Add(currentQuest);

        //Create each event
        /*QuestEvent a = currentQuest.AddQuestEvent("test1", "description 1. This is where the first description will appear. I'm making this extra long to check for errors in my placements but I think it's working properly");
        QuestEvent b = currentQuest.AddQuestEvent("test2", "description 2");
        QuestEvent c = currentQuest.AddQuestEvent("test3", "description 3");
        QuestEvent d = currentQuest.AddQuestEvent("test4", "description 4");
        QuestEvent e = currentQuest.AddQuestEvent("test5", "description 5");

        //define the paths between the events - e.g. the order they must be completed
        currentQuest.AddPath(a.GetId(), b.GetId());
        currentQuest.AddPath(b.GetId(), c.GetId());
        currentQuest.AddPath(b.GetId(), d.GetId());
        currentQuest.AddPath(c.GetId(), e.GetId());
        currentQuest.AddPath(d.GetId(), e.GetId());
        
        currentQuest.BFS(a.GetId());
        */

        //Make loop creating each quest event script and assigning an ID for each quest event.
        foreach (QuestEvent qe in currentQuest.questEvents)
        {
            //CreateQuestEventText(qe);
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

        //int n = 0;
        //Loop sets up each quest object in questObject list
        /*
        foreach (GameObject q in currentQuest.questEven)
        {
            q.GetComponent<QuestObject>().Setup(this, currentQuest.questEvents[n], currentQuest.questEventScripts[n]);
            n++;
        }
        */

        //TODO: Move all quest creation code to its own script instead of keeping it here.

        //QuestEventScript eventScript = CreateQuestEventText(a).GetComponent<QuestEventScript>();
        //A.GetComponent<QuestObject>().Setup(this, a, eventScript); //Assigns definitions for the questmanager, the currentQuest event and the event script to the currentQuest object

        //CreateQuestEventText(b).GetComponent<QuestEventScript>();
        //B.GetComponent<QuestObject>().Setup(this, b, eventScript);

        //currentQuest.PrintPath();
    }


    GameObject CreateQuestEventText(QuestEvent e) //Creates a new text for each currentQuest event requested
    {
        GameObject b = Instantiate(questEventPrefab, questHolder.transform);
        b.GetComponent<QuestEventScript>().Setup(e, questHolder);
        if (e.order == 1)
        {
            //Assigns the first currentQuest event as current
            //b.GetComponent<QuestEventScript>().UpdateElement(QuestEvent.EventStatus.CURRENT);
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
            Debug.Log("Quest event in order " + e.order + " is done! Checking to see if quest event " + n.order + " is equal to " + e.order + 1);
            Debug.Log(e.order + 1);
            //If this event is the next in order
            if (n.order == (e.order + 1))
            {
                //Make the next in line available for completion
                n.UpdateQuestEvent(QuestEvent.EventStatus.CURRENT);
            }
        }
    }
}
