using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charquests : MonoBehaviour
{
    public List<Quest> currentQuests;

    [HideInInspector] public QuestManager questmanager;

    public void AcceptQuest(Quest quest)
    {
        currentQuests.Add(quest);
        questmanager.SetupNewQuest(quest);
    }
}
