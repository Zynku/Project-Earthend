using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System.Linq;
using TMPro;

public class QuestSystemCleanerUpper : MonoBehaviour
{
    GameObject player;
    QuestManager questManager;
    QuestEvent currentEvent;
    Quest currentQuest;

    public void Start()
    {
        player = gamemanager.instance.Player;
        questManager = gamemanager.instance.questManager;
    }

    [ButtonMethod]
    public void CompleteCurrentQuestEvent()
    {
        //Looks in the current quest from quest manager for its quest events and returns the first one that is marked as current
        if (questManager.currentQuest.questEvents.Count != 0)
        {
            currentEvent = questManager.currentQuest.questEvents.Where(currentEvent => currentEvent.status == QuestEvent.EventStatus.CURRENT).FirstOrDefault();
        }
        else
        {
            Debug.LogWarning("No quest events chief...");
        }

        if (currentEvent != null)
        {
            currentEvent.status = QuestEvent.EventStatus.DONE;
            questManager.UpdateQuestsOnCompletion(currentEvent);
        }
    }

    [ButtonMethod]
    public void CompleteCurrentQuest()
    {
        if (questManager.questAcceptedTexts.Count == 0)
        {
            if (player.GetComponent<Charquests>().currentQuests.Count > 0)
            {
                StartCoroutine(questManager.CompleteCurrentQuest());
            }
            else
            {
                Debug.LogWarning("No quests chief...");
            }
        }
        else
        {
            Debug.LogWarning("Wait for the Quest Accepted Text to finish showing before doing this. Try again in a second");
        }
    }

    [ButtonMethod]
    public IEnumerator CompleteAllQuests()
    {
        if (questManager.currentQuest.questName != "No Quest")
        {
            StartCoroutine(questManager.CompleteCurrentQuest());
            //Complete a quest, wait 0.1 seconds, check if there's another quest. If there is complete it
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject qep in questManager.questEventPrefabs)
            {
                Destroy(qep);
            }
            questManager.questEventPrefabs.Clear();
            if (player.GetComponent<Charquests>().currentQuests.Count > 0)
            {
                StartCoroutine(CompleteAllQuests());
            }
        }
    }

    [ButtonMethod]
    public void ClearAllQuestEventPrefabs()
    {
        QuestEventPrefabScript[] prefabsToBeDestroyed = FindObjectsOfType<QuestEventPrefabScript>();
        foreach (QuestEventPrefabScript qes in prefabsToBeDestroyed)
        {
            Destroy(qes.gameObject);
        }
    }

    [ButtonMethod]
    public void FailCurrentQuestEvent()
    {
        //Looks in the current quest from quest manager for its quest events and returns the first one that is marked as current
        if (questManager.currentQuest.questEvents.Count != 0)
        {
            currentEvent = questManager.currentQuest.questEvents.Where(currentEvent => currentEvent.status == QuestEvent.EventStatus.CURRENT).FirstOrDefault();
        }
        else
        {
            Debug.Log("No quest events chief...");
        }

        if (currentEvent != null)
        {
            currentEvent.status = QuestEvent.EventStatus.FAILED;
            questManager.UpdateQuestsOnCompletion(currentEvent);
        }
    }

    [ButtonMethod]
    public void FailCurrentQuest()
    {
        if (player.GetComponent<Charquests>().currentQuests.Count > 0)
        {
            StartCoroutine(questManager.FailCurrentQuest());
        }
        else
        {
            Debug.Log("No quests chief...");
        }
    }

    [ButtonMethod]
    public void ClearQctpsQatpsAndQftps()
    {
        questManager.questAcceptedTextsOffscreen.Clear();
        questManager.questAcceptedTexts.Clear();
        questManager.questCompletedTextsOffscreen.Clear();
        questManager.questCompletedTexts.Clear();
        questManager.questFailedTextsOffscreen.Clear();
        questManager.questFailedTexts.Clear();
    }

    [ButtonMethod]
    public void ResetAllQuestStatuses()
    {
        Quest[] allQuests = Resources.LoadAll("Quests", typeof(Quest)).Cast<Quest>().ToArray();

        foreach (var quest in allQuests)
        {
            quest.questState = Quest.QuestState.WAITING;
            foreach (var questEvent in quest.questEvents)
            {
                questEvent.status = QuestEvent.EventStatus.WAITING;
                foreach (var questLogic in questEvent.questLogic)
                {
                    questLogic.status = QuestEvent.EventStatus.WAITING;
                }
            }
        }

        GameObject[] allNPCS = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npc in allNPCS)
        {
/*            foreach (var dialogueSwitcher in npc.GetComponent<QuestGiver>().dialogueSwitchInstances)
            {
                dialogueSwitcher.dialogueTreeSwitched = false;
            }*/
        }
    }

    [ButtonMethod]
    public void ResetAllDialogueTrees()
    {
        Dialogue[] allDialogues = Resources.LoadAll("Dialogue", typeof(Dialogue)).Cast<Dialogue>().ToArray();
        foreach (var dialogue in allDialogues)
        {
            dialogue.defaultTreeId = 1.ToString();
        }

        GameObject[] allNPCS = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npc in allNPCS)
        {
/*            foreach (var dialogueSwitcher in npc.GetComponent<QuestGiver>().dialogueSwitchInstances)
            {
                dialogueSwitcher.dialogueTreeSwitched = false;
            }*/
            
        }
    }
}
