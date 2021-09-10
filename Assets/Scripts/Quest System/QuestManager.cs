using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public Quest quest = new Quest();
    public TextMeshProUGUI currentQuestText;
    public GameObject questEvent;
    public GameObject questHolder;

    public void Start()
    {
        //Create each event
        QuestEvent a = quest.AddQuestEvent("test1", "description 1");
        QuestEvent b = quest.AddQuestEvent("test2", "description 2");
        QuestEvent c = quest.AddQuestEvent("test3", "description 3");
        QuestEvent d = quest.AddQuestEvent("test4", "description 4");
        QuestEvent e = quest.AddQuestEvent("test5", "description 5");

        //define the paths between the events - e.g. the order they must be completed
        quest.AddPath(a.GetId(), b.GetId());
        quest.AddPath(b.GetId(), c.GetId());
        quest.AddPath(b.GetId(), d.GetId());
        quest.AddPath(c.GetId(), e.GetId());
        quest.AddPath(d.GetId(), e.GetId());

        quest.BFS(a.GetId());

        QuestEventScript eventScript = CreateQuestEventText(a).GetComponent<QuestEventScript>();

        quest.PrintPath();
    }

    GameObject CreateQuestEventText(QuestEvent e)
    {
        GameObject b = Instantiate(questEvent, transform.position, transform.rotation, gameObject.transform);
        b.GetComponent<QuestEventScript>().Setup(e, questHolder);
        if (e.order == 1)
        {
            b.GetComponent<QuestEventScript>().UpdateElement(QuestEvent.EventStatus.CURRENT);
            e.status = QuestEvent.EventStatus.CURRENT;
        }
        return b;
    }
}
