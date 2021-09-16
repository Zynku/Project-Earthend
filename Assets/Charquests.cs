using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charquests : MonoBehaviour
{
    public List<Quest> currentQuests;
    public List<Quest> completedQuests;
    public List<Quest> failedQuests;

    [HideInInspector] public QuestManager questmanager;

    public void AcceptQuest(Quest quest)
    {
        currentQuests.Add(quest);
        questmanager.SetupNewQuest(quest);
    }
}
