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
    public GameObject questDivider;
    public GameObject questEventPrefab;
    public List<GameObject> questEventPrefabs;
    public GameObject questHolder;
    public GameObject questCanvas;
    public GameObject questCompletedTextPrefab;
    public List<GameObject> questCompletedTexts, questCompletedTextsOffscreen;
    public GameObject questAcceptedTextPrefab;
    public List<GameObject> questAcceptedTexts, questAcceptedTextsOffscreen;

    public Quest currentQuest; //The current quest that is being completed.
    public GameObject questName;
    public GameObject questDesc;

    public int randomint = 0;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Charquests>().questmanager = this;
    }

    private void Start()
    {
        questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
    }

    private void Update()
    {
        ManageOffScreenQATPs(); //This is called before ManageOffScreenQCTPs so that it prioritizes showing qatps over qctps
        ManageOffScreenQCTPs();
    }

    void ManageOffScreenQCTPs()  //QCTP = Quest Completed Text Prefabs generated when a quest is completed and stored in questCompletedTextsOffscreen
    {
        //If there is nothing onscreen, remove oldest item from the offscreen list and add it to the onscreen list, and enable it so it shows
        if (questCompletedTexts.Count == 0 && questAcceptedTexts.Count == 0 && questCompletedTextsOffscreen.Count > 0)    
        {
            GameObject oldestqctp = questCompletedTextsOffscreen[0];
            questCompletedTextsOffscreen.Remove(oldestqctp);
            questCompletedTexts.Add(oldestqctp);
            oldestqctp.SetActive(true);
        }
    }

    void ManageOffScreenQATPs()  //QATP = Quest Accepted Text Prefabs generated when a quest is accepted and stored in questAcceptedTextsOffScreen
    {
        if (questCompletedTexts.Count == 0 && questAcceptedTexts.Count == 0 && questAcceptedTextsOffscreen.Count > 0)
        {
            //If there is nothing onscreen, remove oldest item from the offscreen list and add it to the onscreen list, and enable it so it shows
            GameObject oldestqatp = questAcceptedTextsOffscreen[0];
            questAcceptedTextsOffscreen.Remove(oldestqatp);
            questAcceptedTexts.Add(oldestqatp);
            oldestqatp.SetActive(true);
        }
    }

    public void SetupNewQuest(Quest quest)
    {
        if (currentQuest != null) { ClearOldQuest(); }
        {
            questDivider.SetActive(true);
            currentQuestText.gameObject.SetActive(true);
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

            GameObject qatp = Instantiate(questAcceptedTextPrefab, questCanvas.transform);  //Creates a Quest Accepted Text Prefab, or qatp

            bool questAssigned = false;
            if (!questAssigned) { qatp.GetComponent<QuestAcceptedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = currentQuest.name; }    //Assigns the quest name ONCE.
            //Can't use a quest reference because referencing a type (such as Quest) is LIVE, as opposed to referencing a variable (such as Quest.name)
            //that makes a copy. Because of this, if we reference Quest, it will always assign the current quest, even if it has changed since we instantiated this object, 
            //meaning it has the potential to show the wrong quest because it could have changed since this object was instantiated.
            //May need to assign more variables here if more information is needed in qatp script in the future
            if (questAcceptedTexts.Count > 0) //If there are accepted quests on screen, yeet it to the offscreen bin. This is to avoid two accepted texts overlapping
            {
                questAcceptedTextsOffscreen.Add(qatp);
                qatp.SetActive(false);
            }
            else if (questCompletedTexts.Count > 0) //If there are completed quests on screen, yeet it to the offscreen bin. This is to avoid a quest accepted and quest completed text overlapping
            {
                questAcceptedTextsOffscreen.Add(qatp);
                qatp.SetActive(false);
            }
            else
            {
                questAcceptedTexts.Add(qatp);
            }

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
        questDivider.SetActive(true);
        currentQuestText.gameObject.SetActive(true);
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
            else
            {
                Debug.Log("No more quest events chief");
            }
        }
        
        //If the last quest event gets completed, that means the quest is done! Call that function!
        if (currentQuest.questEvents.Last().status == QuestEvent.EventStatus.DONE)
        {
            StartCoroutine(CompleteCurrentQuest(currentQuest));
            questDivider.SetActive(false);
            currentQuestText.gameObject.SetActive(false);
        }
    }

    public IEnumerator CompleteCurrentQuest(Quest quest)
    {
        questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        GameObject qctp = Instantiate(questCompletedTextPrefab, questCanvas.transform);     //Creates a Quest Completed Text Prefab, or qctp
        bool questAssigned = false;
        if (!questAssigned) { qctp.GetComponent<QuestCompletedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = currentQuest.name; }   //Assigns the quest name ONCE.
        if (questCompletedTexts.Count > 0)  //If there are completed quests on screen, yeet it to the offscreen bin
        {
            questCompletedTextsOffscreen.Add(qctp);
            qctp.SetActive(false);
        }
        else if (questAcceptedTexts.Count > 0)     //If there are accepted quests on screen, yeet it to the offscreen bin
        {
            questCompletedTextsOffscreen.Add(qctp);
            qctp.SetActive(false);
        }
        else
        {
            questCompletedTexts.Add(qctp);     //If there is nothing onscreen, just put it onscreen
        }

        //Gets rid of all quest event prefabs on screen and clears the list
        foreach (GameObject qep in questEventPrefabs)
        {
            Destroy(qep);
        }
        questEventPrefabs.Clear();

        yield return new WaitForSeconds(0.1f);
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
