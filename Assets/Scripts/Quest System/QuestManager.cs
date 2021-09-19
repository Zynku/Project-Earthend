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
    public QuestCountdownTimer countDownTimer;
    public float timerTime;             //Actual timer time, changes with time.deltatime and counts down to 0
    public float timerTargetTime;       //Time that timer starts at before counting down
    private bool liveTimer;             //Is true when a quest has a timer and lets timer run
    public GameObject questTimerPrefab;
    public List<GameObject> questTimers;    //Keeps a list of quest timers that are instantiated and deleted as necessary
    public GameObject questEventPrefab;                                             //Assigned in Inspector
    public List<GameObject> questEventPrefabs;
    public GameObject questHolder;                                                  //Holds newly created QCTPs, QATPs and QFTPs. See below for abbreviation meanings
    public GameObject questCanvas;                                                  //Assigned in inspector
    public GameObject questCompletedTextPrefab;                                     //Assigned in inspector
    public List<GameObject> questCompletedTexts, questCompletedTextsOffscreen;
    public GameObject questAcceptedTextPrefab;                                      //Assigned in inspector
    public List<GameObject> questAcceptedTexts, questAcceptedTextsOffscreen;
    public GameObject questFailedTextPrefab;                                        //Assigned in inspector
    public List<GameObject> questFailedTexts, questFailedTextsOffscreen;

    public Quest currentQuest; //The current quest that is being completed.
    public QuestEvent currentQuestEvent;    //The current quest event that is being completed.
    public GameObject questName;
    public GameObject questDesc;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Charquests>().questmanager = this;
    }

    private void Start()
    {
        questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
    }

    private void Update()
    {
        ManageOffScreenQATPs(); //This is called before ManageOffScreenQCTPs so that it prioritizes showing qatps over qctps
        ManageOffScreenQCTPs();
        ManageOffScreenQFTPs();
        if (liveTimer) { ManageTimer(); }
    }

    void ManageOffScreenQCTPs()  //QCTP = Quest Completed Text Prefabs generated when a quest is completed and stored in questCompletedTextsOffscreen
    {
        //If there is nothing onscreen, remove oldest item from the offscreen list and add it to the onscreen list, and enable it so it shows
        if (questFailedTexts.Count == 0 && questCompletedTexts.Count == 0 && questAcceptedTexts.Count == 0 && questCompletedTextsOffscreen.Count > 0)    
        {
            Debug.Log("Nothing onscreen, showing oldest QCTP");
            GameObject oldestqctp = questCompletedTextsOffscreen[0];
            questCompletedTextsOffscreen.Remove(oldestqctp);
            questCompletedTexts.Add(oldestqctp);
            oldestqctp.SetActive(true);
        }
    }

    void ManageOffScreenQATPs()  //QATP = Quest Accepted Text Prefabs generated when a quest is accepted and stored in questAcceptedTextsOffScreen
    {
        if (questFailedTexts.Count == 0 && questCompletedTexts.Count == 0 && questAcceptedTexts.Count == 0 && questAcceptedTextsOffscreen.Count > 0)
        {
            Debug.Log("Nothing onscreen, showing oldest QATP");
            //If there is nothing onscreen, remove oldest item from the offscreen list and add it to the onscreen list, and enable it so it shows
            GameObject oldestqatp = questAcceptedTextsOffscreen[0];
            questAcceptedTextsOffscreen.Remove(oldestqatp);
            questAcceptedTexts.Add(oldestqatp);
            oldestqatp.SetActive(true);
        }
    }

    void ManageOffScreenQFTPs()  //QFTP = Quest Failed Text Prefabs generated when a quest is failed and stored in questFailedTextsOffScreen
    {
        if (questFailedTexts.Count == 0 && questCompletedTexts.Count == 0 && questAcceptedTexts.Count == 0 && questFailedTextsOffscreen.Count > 0)
        {
            Debug.Log("Nothing onscreen, showing oldest QFTP");
            //If there is nothing onscreen, remove oldest item from the offscreen list and add it to the onscreen list, and enable it so it shows
            GameObject oldestqftp = questFailedTextsOffscreen[0];
            questAcceptedTextsOffscreen.Remove(oldestqftp);
            questAcceptedTexts.Add(oldestqftp);
            oldestqftp.SetActive(true);
        }
    }

    public void SetupNewQuest(Quest quest)
    {
        if (currentQuest != null) { ClearOldQuest(); }
        {
            questDivider.SetActive(true);                            //Activates UI Elements associated with a quest
            currentQuestText.gameObject.SetActive(true);
            currentQuest = quest;                                     //Assigns incoming quest as current and lets the quest manager track it
            currentQuest.questState = Quest.QuestState.CURRENT;       //Sets current quest as current
            CheckforQuestTimer();                                     //Checks to see if the current quest has any sort of timer attached to it

            foreach (QuestEvent qe in currentQuest.questEvents)       //Sets all quest events as waiting
            {
                qe.status = QuestEvent.EventStatus.WAITING;
            }

            QuestEvent firstQE = currentQuest.questEvents.First();      //Sets the first quest event as current
            firstQE.status = QuestEvent.EventStatus.CURRENT;

            currentQuestEvent = firstQE;
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
            else if (questFailedTexts.Count > 0)     //If there are failed quests on screen, yeet it to the offscreen bin
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
                    qo.GetComponent<QuestObject>().Setup(this, currentQuest.questEvents[n], currentQuest.questEventScripts[n], currentQuest);
                    n++;
                }
            }
        }
    }

    private void CheckforQuestTimer()                //Checks to see if the current quest has a timer every 0.5 seconds. If it does it activates the quest manager timer and sends it to be formatted
    {
        if (currentQuest.hasTimerForQuest) 
        {
            countDownTimer.rawTimer = timerTime;
            countDownTimer.gameObject.SetActive(true);
            liveTimer = true;
            Debug.Log("Quest has a quest timer");
            ManageTimer();
        }

        if (currentQuest.hasTimerForEvent)
        {

        }
    }

    public void ManageTimer()
    {
        //TODO: Fix this function to make sure it doesnt get called repeatedly, because it currently does...
        Debug.Log("Starting ManageTimer()");
        bool timerMade = false;
        if (questTimers.Count > 0 && !timerMade)
        {
            foreach (GameObject timer in questTimers)
            {
                if (timer.GetComponent<QuestTimer>().myQuest == currentQuest)
                {
                    Debug.Log("A quest timer for this quest already exists!");
                }
                else  //Otherwise...
                {
                    timerMade = true;
                    Debug.Log("No quest timers for this quest exist, instantiating one...");
                    GameObject newTimer = Instantiate(questTimerPrefab, questHolder.transform);    //Creates a new quest Timer that is in charge of counting down and keeping track of time and when its completed
                    questTimers.Add(newTimer);
                    QuestTimer timerScript = newTimer.GetComponent<QuestTimer>();
                    timerScript.timerTargetTime = currentQuest.timerTargetTime;
                    if (currentQuest.hasTimerForQuest)
                    {
                        timerScript.TypeofTimer = QuestTimer.Timertype.QUEST;
                    }
                    else if (currentQuest.hasTimerForEvent)
                    {
                        timerScript.TypeofTimer = QuestTimer.Timertype.EVENT;
                    }
                    StartCoroutine(CheckforTimerDone());
                }
            }
        }
        else
        {
            timerMade = true;
            Debug.Log("No quest timers exist at all, instantiating one...");
            GameObject newTimer = Instantiate(questTimerPrefab, questHolder.transform);    //Creates a new quest Timer that is in charge of counting down and keeping track of time and when its completed
            questTimers.Add(newTimer);
            QuestTimer timerScript = newTimer.GetComponent<QuestTimer>();
            timerScript.timerTargetTime = currentQuest.timerTargetTime;
            if (currentQuest.hasTimerForQuest)
            {
                timerScript.TypeofTimer = QuestTimer.Timertype.QUEST;
            }
            else if (currentQuest.hasTimerForEvent)
            {
                timerScript.TypeofTimer = QuestTimer.Timertype.EVENT;
            }
            StartCoroutine(CheckforTimerDone());
        }
        

        /*for (int i = 0; i < questTimers.Count; i++)
        {
            if (questTimers[i].GetComponent<QuestTimer>().myQuest == currentQuest)  //If there's a quest Timer already created for this quest, do nothing
            {
                Debug.Log("A quest timer for this quest already exists!");
            }
            else  //Otherwise...
            {
                Debug.Log("No quest timers for this quest exist, instantiating one...");
                GameObject newTimer = Instantiate(questTimerPrefab, questHolder.transform);    //Creates a new quest Timer that is in charge of counting down and keeping track of time and when its completed
                questTimers.Add(newTimer);
                QuestTimer timerScript = newTimer.GetComponent<QuestTimer>();
                timerScript.timerTargetTime = currentQuest.timerTargetTime;
                if (currentQuest.hasTimerForQuest)
                {
                    timerScript.TypeofTimer = QuestTimer.Timertype.QUEST;
                }
                else if (currentQuest.hasTimerForEvent)
                {
                    timerScript.TypeofTimer = QuestTimer.Timertype.EVENT;
                }
                StartCoroutine(CheckforTimerDone());
            }
        }*/
        

        if (timerTime <= 1)
        {
            timerTime = 1;
            //currentQuestEvent.UpdateQuestEvent(QuestEvent.EventStatus.FAILED);
            //UpdateQuestsOnCompletion(currentQuestEvent);
            //StartCoroutine(FailCurrentQuest());
            timerTime = timerTargetTime;
        }
    }

    private IEnumerator CheckforTimerDone() //Constantly checks every timer in the questTimers list to see if its done. If it is, fail that quest or event
    {
        yield return new WaitForSeconds(0.2f);
        if (questTimers.Count > 0)
        {
            foreach (GameObject timer in questTimers)
            {
                if (timer.GetComponent<QuestTimer>().timerDone == true)
                {
                    //Fail current quest if it is a quest timer
                    //Fail current event if it is an event timer
                }
            }
            StartCoroutine(CheckforTimerDone());
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
        //if (currentQuest.hasTimer) { countDownTimer.gameObject.SetActive(true); }
        foreach (QuestEvent qe in currentQuest.questEvents)
        {
            qe.status = QuestEvent.EventStatus.WAITING;
        }
        QuestEvent firstQE = currentQuest.questEvents.First();
        firstQE.status = QuestEvent.EventStatus.CURRENT;
        currentQuestEvent = firstQE;
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
                qo.GetComponent<QuestObject>().Setup(this, currentQuest.questEvents[n], currentQuest.questEventScripts[n], currentQuest);
                n++;
            }
        }
    }

    public void UpdateQuestsOnCompletion(QuestEvent e)
    {
        if (currentQuest.questEvents.Count > 0)     //If there are quest events...
        {
            foreach (QuestEvent n in currentQuest.questEvents)
            {
                //If this event is the next in order
                if (n.order == (e.order + 1))
                {
                    //Make the next in line available for completion
                    n.UpdateQuestEvent(QuestEvent.EventStatus.CURRENT);
                    currentQuestEvent = n;

                    //Check if the next quest event has a timer
                }
            }
        }

        bool qctpShown = false;
        bool qftpShown = false;
        //If the last quest event gets completed, that means the quest is done! Call that function!
        if (currentQuest.questEvents.Last().status == QuestEvent.EventStatus.DONE && !qctpShown)
        {
            StartCoroutine(CompleteCurrentQuest());
            questDivider.SetActive(false);
            currentQuestText.gameObject.SetActive(false);
            countDownTimer.gameObject.SetActive(false);
            liveTimer = false;
            qctpShown = true;
        }
        else if (currentQuest.questEvents.Last().status == QuestEvent.EventStatus.FAILED && !qftpShown)    //Right now it fails if only the LAST quest event is failed. Add functionality for flexibility
        {
            StartCoroutine(FailCurrentQuest());
            questDivider.SetActive(false);
            countDownTimer.gameObject.SetActive(false);
            liveTimer = false;
            currentQuestText.gameObject.SetActive(false);
            qftpShown = true;
        }
    }

    public IEnumerator CompleteCurrentQuest()
    {
        questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
        liveTimer = false;
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
        else if (questFailedTexts.Count > 0)     //If there are failed quests on screen, yeet it to the offscreen bin
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

        currentQuestEvent = null;
        currentQuest = null;    //Sets the current quest to nothing

        if (player.GetComponent<Charquests>().currentQuests.Count != 0)     //If there are more quests yet to be completed, add the next one back
        {
            ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }

    public IEnumerator FailCurrentQuest()       //TODO: Expand this, make sure quest is added to appropriate list in Charquests both in this function and the one above.
    {
        questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
        liveTimer = false;
        GameObject qftp = Instantiate(questFailedTextPrefab, questCanvas.transform);     //Creates a Quest Failed Text Prefab, or qftp
        bool questAssigned = false;
        if (!questAssigned) { qftp.GetComponent<QuestFailedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = currentQuest.name; }   //Assigns the quest name ONCE.
        if (questCompletedTexts.Count > 0)  //If there are completed quests on screen, yeet it to the offscreen bin
        {
            questFailedTextsOffscreen.Add(qftp);
            qftp.SetActive(false);
        }
        else if (questAcceptedTexts.Count > 0)     //If there are accepted quests on screen, yeet it to the offscreen bin
        {
            questFailedTextsOffscreen.Add(qftp);
            qftp.SetActive(false);
        }
        else if (questFailedTexts.Count > 0)
        {
            questFailedTextsOffscreen.Add(qftp);
            qftp.SetActive(false);
        }
        else
        {
            questFailedTexts.Add(qftp);     //If there is nothing onscreen, just put it onscreen
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
        currentQuest.questState = Quest.QuestState.FAILED;

        currentQuestEvent = null;
        currentQuest = null;    //Sets the current quest to nothing

        if (player.GetComponent<Charquests>().currentQuests.Count != 0)     //If there are more quests yet to be completed, add the next one back
        {
            ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }
}
