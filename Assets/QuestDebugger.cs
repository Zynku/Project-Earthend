using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class QuestDebugger : MonoBehaviour
{
    GameObject player;
    QuestManager questManager;
    QuestEvent currentEvent;
    Quest currentQuest;
    public GameObject shownDebugType;
    public DebugType DebuggingType;

    public enum DebugType { completeAllQuests, completeCurrentQuest, completeCurrentQuestEvent, failCurrentQuestEvent, failCurrentQuest, clearAllQuestEventPrefabs}
    bool playerInRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        shownDebugType.GetComponent<TextMeshPro>().text = DebuggingType.ToString();
    }

    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            switch (DebuggingType)
            {
                case DebugType.completeAllQuests:
                    StartCoroutine(CompleteAllQuests());
                    break;
                case DebugType.completeCurrentQuest:
                    CompleteCurrentQuest();
                    break;
                case DebugType.completeCurrentQuestEvent:
                    CompleteCurrentQuestEvent();
                    break;
                case DebugType.clearAllQuestEventPrefabs:
                    ClearAllQuestEventPrefabs();
                    break;
                case DebugType.failCurrentQuestEvent:
                    FailCurrentQuestEvent();
                    break;
                case DebugType.failCurrentQuest:
                    FailCurrentQuest();
                    break;
                default:
                    break;
            }
        }
    }

    void CompleteCurrentQuestEvent()
    {
        //Looks in the current quest from quest manager for its quest events and returns the first one that is marked as current
        if (questManager.currentQuest.questEvents.Count != 0)
        {
            //TODO: FIX THIS
            //currentEvent = questManager.currentQuest.questEvents.Where(currentEvent => currentEvent.status == QuestEvent.EventStatus.CURRENT).FirstOrDefault();
        }
        else
        {
            Debug.Log("No quest events chief...");
        }

        if (currentEvent != null)
        {
            //currentEvent.UpdateQuestEvent(QuestEvent.EventStatus.DONE);
            //questManager.UpdateQuestsOnCompletion(currentEvent); 
        }
    }

    void CompleteCurrentQuest()
    {
        if (player.GetComponent<Charquests>().currentQuests.Count > 0)
        {
            StartCoroutine(questManager.CompleteCurrentQuest());
        }
        else
        {
            Debug.Log("No quests chief...");
        }
    }

    IEnumerator CompleteAllQuests()
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

    void ClearAllQuestEventPrefabs()
    {
/*        QuestEventScript[] prefabsToBeDestroyed = FindObjectsOfType<QuestEventScript>();
        foreach (QuestEventScript qes in prefabsToBeDestroyed)
        {
            Destroy(qes.gameObject);
        }*/
    }

    void FailCurrentQuestEvent()
    {
        //Looks in the current quest from quest manager for its quest events and returns the first one that is marked as current
        if (questManager.currentQuest.questEvents.Count != 0)
        {
            //TODO: FIX THIS
            //currentEvent = questManager.currentQuest.questEvents.Where(currentEvent => currentEvent.status == QuestEvent.EventStatus.CURRENT).FirstOrDefault();
        }
        else
        {
            Debug.Log("No quest events chief...");
        }

        if (currentEvent != null)
        {
            //currentEvent.UpdateQuestEvent(QuestEvent.EventStatus.FAILED);
            //questManager.UpdateQuestsOnCompletion(currentEvent);
        }
    }

    void FailCurrentQuest()
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
}
