using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charquests : MonoBehaviour
{
    public List<Quest> currentQuests;

    public QuestManager questmanager;

    public void AcceptQuest(Quest quest)
    {
        currentQuests.Add(quest);
        questmanager.SetupNewQuest(quest);
    }

    public void Test()
    {
        Debug.Log("IT WORKS");
    }

}
