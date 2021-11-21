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
    private float acceptTimer;
    public bool canAcceptQuest;
    public GameObject questDivider;
    public QuestCountdownTimer countDownTimer;
    public float timerTime;             //Actual timer time, changes with time.deltatime and counts down to 0
    public float timerTargetTime;       //Time that timer starts at before counting down
    private bool liveTimer;             //Is true when a quest has a timer and lets timer run
    public GameObject questTimerPrefab;
    public List<GameObject> questTimers;    //Keeps a list of quest timers that are instantiated and deleted as necessary
    public GameObject questEventPrefab;                                             //Assigned in Inspector
    public List<GameObject> questEventPrefabs;
    public GameObject questTimerHolder;                                             //Holds newly created quest Timers
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
        ManageOffScreenQATPs(); //This is called before ManageOffScreenQFTPs and QCTPs so that it prioritizes showing qatps over qctps and qftps over qctps
        ManageOffScreenQFTPs();
        ManageOffScreenQCTPs();

        //Cooldown for accepting quests
        if (!canAcceptQuest) { acceptTimer -= Time.deltaTime; }
        if (acceptTimer < 0) { acceptTimer = 3f; canAcceptQuest = true; }
    }

    void ManageOffScreenQCTPs()  //QCTP = Quest Completed Text Prefabs generated when a quest is completed and stored in questCompletedTextsOffscreen
    {
        //If there is nothing onscreen, remove oldest item from the offscreen list and add it to the onscreen list, and enable it so it shows
        if (questFailedTexts.Count == 0 && questCompletedTexts.Count == 0 && questAcceptedTexts.Count == 0 && questCompletedTextsOffscreen.Count > 0)    
        {
            //Debug.Log("Nothing onscreen, showing oldest QCTP");
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
            //Debug.Log("Nothing onscreen, showing oldest QATP");
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
            //Debug.Log("Nothing onscreen, showing oldest QFTP");
            //If there is nothing onscreen, remove oldest item from the offscreen list and add it to the onscreen list, and enable it so it shows
            GameObject oldestqftp = questFailedTextsOffscreen[0];
            questAcceptedTextsOffscreen.Remove(oldestqftp);
            questAcceptedTexts.Add(oldestqftp);
            oldestqftp.SetActive(true);
        }
    }

    public void SetupNewQuest(Quest quest)
    {
        if (CheckforSameQuest(quest))
        {
            Debug.LogWarning("Quest already accepted!");
            return;
        }
        else if (canAcceptQuest)
        {
            if (currentQuest != null) { ClearOldQuest(); }  //Check for a quest before you setup a new one.
            {
                if (quest.questEvents.Count == 0)
                {
                    Debug.LogWarning("Quest has no quest events! Quest needs atleast one to function properly");
                    return;
                }
                else
                {
                    player.GetComponent<Charquests>().currentQuests.Add(quest);
                    canAcceptQuest = false;
                    questDivider.SetActive(true);                            //Activates UI Elements associated with a quest
                    currentQuestText.gameObject.SetActive(true);
                    currentQuest = quest;                                     //Assigns incoming quest as current and lets the quest manager track it
                    currentQuest.questState = Quest.QuestState.CURRENT;       //Sets current quest as current
                                                                              //Debug.Log("Calling CheckforQuestTimer");
                    CheckforQuestTimer();                                     //Checks to see if the current quest has any sort of timer attached to it
                    SetupQuestEvents();
                    GameObject qatp = Instantiate(questAcceptedTextPrefab, questCanvas.transform);  //Creates a Quest Accepted Text Prefab, or qatp

                    bool questAssigned = false;
                    if (!questAssigned) { qatp.GetComponent<QuestAcceptedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = currentQuest.questName; }    //Assigns the quest name ONCE.

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
                }
            }
        }
    }

    public bool CheckforSameQuest(Quest myQuest)
    {
        foreach (Quest quest in player.GetComponent<Charquests>().currentQuests)
        {
            if (quest.questName == myQuest.questName)
            {
                return true;
            }
        }
        return false;
    }

    public void SetupQuestEvents()  //Also sets up quest objects and quest object game object (should they have one)
    {
        //Sets up all quest event prefabs and sets quest events as waiting
        foreach (QuestEvent qe in currentQuest.questEvents)       
        {
            var qep = Instantiate(questEventPrefab, questHolder.transform);     //QEP is a gameObject
            var qeps = qep.GetComponent<QuestEventPrefabScript>();              //QEPS is the script attached to the QEP
            qeps.Setup(qe, questHolder, qep);
            qe.status = QuestEvent.EventStatus.WAITING;

            //Loop setting up each quest object
            foreach (QuestObject qo in qe.questObjects)          
            {
                if (qo == null)
                {
                    Debug.LogWarning("There are no quest events associated with this quest event! Please Check the quest to make sure each quest event has a quest object!");
                }
                else
                {
                    qo.Setup(this, qe, qeps, currentQuest);
                    qo.associatedGameObject.GetComponent<QuestObjectGameObject>().Setup(qo);
                }
            }
        }

        QuestEvent firstQE = currentQuest.questEvents.First();      //Sets the first quest event as current
        firstQE.status = QuestEvent.EventStatus.CURRENT;

        currentQuestEvent = firstQE;
        currentQuest.isActive = true;
        questName.GetComponent<TextMeshProUGUI>().text = currentQuest.questName;
        questDesc.GetComponent<TextMeshProUGUI>().text = currentQuest.desc;

        /*
        //Make loop creating each quest event script and assigning an ID for each quest event.
        foreach (QuestEvent qe in currentQuest.questEvents)
        {
            currentQuest.questEventScripts.Add(CreateQuestEventText(qe).GetComponent<QuestEventScript>());
            //quest.BFS(qe.GetId()); // No longer necessary since order is assigned in inspector now
        }*/
    }

    public void UpdateQuestEvent(Quest quest) //Update quests everytime something is done
    {
        
    }

    private void CheckforQuestTimer()  //Checks to see if the current quest has a timer. If it does it activates the quest manager timer and sends it to be formatted
    {
        if (currentQuest.hasTimerForQuest)
        {
            countDownTimer.gameObject.SetActive(true);
            countDownTimer.shownTimer.text = "";
            liveTimer = true;
            //Debug.Log("Quest demands a timer be made, invoking ManageTimer() to make one");
            ManageTimer();
        }
        else if (currentQuest.hasTimerForEvent)
        {
            //TODO: Manage the code here for quest timers that refer to events, and not full quests
        }
        else
        {
            countDownTimer.shownTimer.text = "";
            countDownTimer.gameObject.SetActive(false);
            liveTimer = false;
            //Debug.Log("Quest does not have a timer attached");
        }
    }

    public void ManageTimer()
    {
        //Debug.Log("Starting ManageTimer()");
        if (questTimers.Count > 0)  //If quest timers exist
        {
            foreach (GameObject timer in questTimers)   //Check them all to make sure none of them are associated with the quest current quest
            {
                if (timer.GetComponent<QuestTimer>().myQuest == currentQuest)
                {
                    //Debug.Log("A quest timer for this quest already exists!");
                    return;
                }
                else
                {
                    GameObject newTimer = Instantiate(questTimerPrefab, questTimerHolder.transform);    //Creates a new quest Timer that is in charge of counting down and keeping track of time and when its completed
                    questTimers.Add(newTimer);
                    QuestTimer timerScript = newTimer.GetComponent<QuestTimer>();
                    timerScript.timerTargetTime = currentQuest.timerTargetTime;            //Sets the starting time for the timer script to the quest's amount of time
                    liveTimer = true;
                    StartCoroutine(AssignTimer(timerScript));

                    if (currentQuest.hasTimerForQuest)
                    {
                        timerScript.TypeofTimer = QuestTimer.Timertype.QUEST;
                        timerScript.myQuest = currentQuest;
                    }
                    else if (currentQuest.hasTimerForEvent)
                    {
                        timerScript.TypeofTimer = QuestTimer.Timertype.EVENT;
                        timerScript.myQuestEvent = currentQuestEvent;
                    }
                    return;
                }
            }
        }
        else
        {
            //Debug.Log("No quest timers exist at all, instantiating one...");
            GameObject newTimer = Instantiate(questTimerPrefab, questTimerHolder.transform);    //Creates a new quest Timer that is in charge of counting down and keeping track of time and when its completed
            questTimers.Add(newTimer);
            QuestTimer timerScript = newTimer.GetComponent<QuestTimer>();
            timerScript.timerTargetTime = currentQuest.timerTargetTime;            //Sets the starting time for the timer script to the quest's amount of time
            liveTimer = true;
            StartCoroutine(AssignTimer(timerScript));
            
            if (currentQuest.hasTimerForQuest)
            {
                timerScript.TypeofTimer = QuestTimer.Timertype.QUEST;
                timerScript.myQuest = currentQuest;
            }
            else if (currentQuest.hasTimerForEvent)
            {
                timerScript.TypeofTimer = QuestTimer.Timertype.EVENT;
                timerScript.myQuestEvent = currentQuestEvent;
            }
            StartCoroutine(CheckforTimerDone());
        }        
    }

    private IEnumerator AssignTimer(QuestTimer timerScript)
    {
        //Debug.Log("Updating formatted timer");
        yield return new WaitForSeconds(0.1f);
        if (timerScript.myQuest == currentQuest)
        {
            countDownTimer.shownTimer.text = timerScript.formattedTime;             //Sets the timer text onscreen to whatever the timerscript says as
        }
        if (liveTimer) { StartCoroutine(AssignTimer(timerScript)); }            //Loops this coroutine to continue assigning the text if there is a text available
    }

    private IEnumerator CheckforTimerDone() //Constantly checks every timer in the questTimers list to see if its done. If it is, fail that quest or event
    {
        yield return new WaitForSeconds(0.5f);
        if (questTimers.Count > 0)
        {
            foreach (GameObject timer in questTimers)
            {
                if (timer.GetComponent<QuestTimer>().timerDone == true)
                {
                    //Debug.Log("Timer done, quest failed...");
                    //Fail current quest if it is a quest timer
                    StartCoroutine(FailSpecificQuest(timer.GetComponent<QuestTimer>().myQuest));
                    questTimers.Remove(timer);
                    Destroy(timer);
                    break;

                    //Fail current event if it is an event timer
                }
            }
            StartCoroutine(CheckforTimerDone());
        }
        else
        {
            yield break;
        }
    }

    GameObject CreateQuestEventText(QuestEvent e) //Creates a new text for each currentQuest event requested
    {
        GameObject b = Instantiate(questEventPrefab, questHolder.transform);
        b.GetComponent<QuestEventPrefabScript>().currentEventText.text = "";
        //b.GetComponent<QuestEventScript>().Setup(e, questHolder);
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

        if (currentQuest.hasTimerForQuest) { countDownTimer.gameObject.SetActive(true); }
        foreach (GameObject timer in questTimers)   //Check them all to which one is associated with the quest you're readding
        {
            if (timer.GetComponent<QuestTimer>().myQuest == quest)
            {
                //Debug.Log("A timer for this quest exists!");
                StartCoroutine(AssignTimer(timer.GetComponent<QuestTimer>()));
                liveTimer = true;
            }
        }

        foreach (QuestEvent qe in currentQuest.questEvents)
        {
            qe.status = QuestEvent.EventStatus.WAITING;
        }
        QuestEvent firstQE = currentQuest.questEvents.First();
        firstQE.status = QuestEvent.EventStatus.CURRENT;
        currentQuestEvent = firstQE;
        currentQuest.isActive = true;
        questName.GetComponent<TextMeshProUGUI>().text = currentQuest.questName;
        questDesc.GetComponent<TextMeshProUGUI>().text = currentQuest.desc;

        SetupQuestEvents();
    }

    public void UpdateQuestsOnCompletion(QuestEvent e)
    {
        e.status = QuestEvent.EventStatus.DONE;
        if (currentQuest.questEvents.Count > 0)     //If there are quest events...
        {
            foreach (QuestEvent n in currentQuest.questEvents)
            {
                //If this event is the next in order
                if (n.order == (e.order + 1))
                {
                    //Make the next in line available for completion
                    n.status = QuestEvent.EventStatus.CURRENT;
                    currentQuestEvent = n;
                    return;
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


        if (questTimers.Count > 0)  //If quest timers exist
        {
            //Debug.Log("Timers exist...");
            foreach (GameObject timer in questTimers)   //Check them all for a timer associated with the current quest
            {
                //Debug.Log("Checking timer for " + timer.GetComponent<QuestTimer>().myQuest);
                if (timer.GetComponent<QuestTimer>().myQuest == currentQuest)
                {
                    //Debug.Log("Found a timer for " + currentQuest.name + " .Destroying it");
                    questTimers.Remove(timer);          //Remove from list
                    Destroy(timer);                     //Destroy it
                    break;
                }
            }
        }

        GameObject qctp = Instantiate(questCompletedTextPrefab, questCanvas.transform);     //Creates a Quest Completed Text Prefab, or qctp
        bool questAssigned = false;
        if (!questAssigned) { qctp.GetComponent<QuestCompletedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = currentQuest.questName; }   //Assigns the quest name ONCE.
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

        if (questTimers.Count > 0)  //If quest timers exist
        {
            foreach (GameObject timer in questTimers)   //Check them all for a timer associated with the current quest
            {
                if (timer.GetComponent<QuestTimer>().myQuest == currentQuest)
                {
                    questTimers.Remove(timer);          //Remove from list
                    Destroy(timer);                     //Destroy it
                    break;
                }
            }
        }

        GameObject qftp = Instantiate(questFailedTextPrefab, questCanvas.transform);     //Creates a Quest Failed Text Prefab, or qftp
        bool questAssigned = false;
        if (!questAssigned) { qftp.GetComponent<QuestFailedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = currentQuest.questName; }   //Assigns the quest name ONCE.
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
           // ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }

    public IEnumerator FailSpecificQuest(Quest quest)
    {
        questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
        liveTimer = false;

        if (questTimers.Count > 0)  //If quest timers exist
        {
            foreach (GameObject timer in questTimers)   //Check them all for a timer associated with the current quest
            {
                if (timer.GetComponent<QuestTimer>().myQuest == quest)
                {
                    questTimers.Remove(timer);          //Remove from list
                    Destroy(timer);                     //Destroy it
                    break;
                }
            }
        }

        GameObject qftp = Instantiate(questFailedTextPrefab, questCanvas.transform);     //Creates a Quest Failed Text Prefab, or qftp
        bool questAssigned = false;
        if (!questAssigned) { qftp.GetComponent<QuestFailedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = quest.questName; }   //Assigns the quest name ONCE.
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
        quest.isActive = false;
        player.GetComponent<Charquests>().currentQuests.Remove(quest);
        quest.questState = Quest.QuestState.FAILED;

        //currentQuestEvent = null;
        //currentQuest = null;    //Sets the current quest to nothing

        if (player.GetComponent<Charquests>().currentQuests.Count != 0)     //If there are more quests yet to be completed, add the next one back
        {
           ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }
}
