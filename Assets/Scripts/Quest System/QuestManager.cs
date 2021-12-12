using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using MyBox;

public class QuestManager : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public TextMeshProUGUI currentQuestText;
    [HideInInspector] public Charpickup_inventory inventory;
    [HideInInspector] public IHUIQuestManager ihuiquestmanager;

    [Separator("General Quest Variables")]
    public bool canAcceptQuest;
    private float acceptTimer;
    public bool showLocationsDebug;
    public Quest[] allQuests;
    public Quest lastAcceptedQuest;              //The last quest that had been accepted.
    public QuestEvent lastAcceptedQuestEvent;    //The last quest event that had been accepted.
    //public GameObject questName;
    //public GameObject questDesc;
    private bool checkedForQuestLogicTypes = false; //Makes sure that this script isnt constantly checking the type, and only does it once

    [Separator("Quest & Event Timer Pieces")]
    public QuestCountdownTimer countDownTimer;
    public GameObject questTimerPrefab;
    public List<GameObject> questTimers;          //Keeps a list of quest timers that are instantiated and deleted as necessary
    public float questLogicTimerTime;             //Actual timer time, changes with time.deltatime and counts down to 0
    public float questLogicTimerTargetTime;       //Time that timer starts at before counting down
    private bool timerSet;              //Have the values for timerTime and timerTargetTime been assigned?
    private bool liveTimer;             //Is true when a quest has a timer and lets timer run

    [Separator("Canvas and UI Pieces")]
    //public GameObject questDivider;
    
    //public GameObject questEventPrefab;                                             //Assigned in Inspector
    //public List<GameObject> questEventPrefabs;
    public GameObject questTimerHolder;                                             //Holds newly created quest Timers
    public GameObject questHolder;                                                  //Holds newly created QCTPs, QATPs and QFTPs. See below for abbreviation meanings
    public GameObject questCanvas;                                                  //Assigned in inspector

    [Separator("QCTPs, QATPs and QFTPs")]
    public GameObject questCompletedTextPrefab;                                     //Assigned in inspector
    public List<GameObject> questCompletedTexts, questCompletedTextsOffscreen;
    public GameObject questAcceptedTextPrefab;                                      //Assigned in inspector
    public List<GameObject> questAcceptedTexts, questAcceptedTextsOffscreen;
    public GameObject questFailedTextPrefab;                                        //Assigned in inspector
    public List<GameObject> questFailedTexts, questFailedTextsOffscreen;
    public bool qBlankOnScreen, qctpOnScreen, qatpOnScreen, qftpOnScreen;           //Are any of the related qBlanktp's currently onscreen?

    private void Awake()
    {
        player.GetComponent<Charquests>().questmanager = this;
    }

    private void Start()
    {
        player = GameManager.instance.Player;
        inventory = player.GetComponent<Charpickup_inventory>();
        ihuiquestmanager = GameManager.instance.ihuiquestmanager;
        //questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
    }

    private void Update()
    {
        ManageOffScreenQATPs(); //This is called before ManageOffScreenQFTPs and QCTPs so that it prioritizes showing qatps over qctps and qftps over qctps
        ManageOffScreenQFTPs();
        ManageOffScreenQCTPs();
        ManageQBlankTPs();

        //Cooldown for accepting quests
        if (!canAcceptQuest) { acceptTimer -= Time.deltaTime; }
        if (acceptTimer < 0) { acceptTimer = 3f; canAcceptQuest = true; }

        if (lastAcceptedQuest != null && !checkedForQuestLogicTypes) 
        {
            foreach (QuestEvent qe in lastAcceptedQuest.questEvents)
            {
                foreach (QuestLogic ql in qe.questLogic)
                {
                    switch (ql.type)
                    {
                        case QuestLogic.EventType.none:
                            break;
                        case QuestLogic.EventType.location:
                            StartCoroutine(CheckForLocationCollisions(ql));
                            checkedForQuestLogicTypes = true;
                            break;
                        case QuestLogic.EventType.collectUniqueItem:
                            break;
                        case QuestLogic.EventType.playerAspectMonitor:
                            StartCoroutine(CheckForPlayerAspect(ql));
                            checkedForQuestLogicTypes = true;
                            break;
                        case QuestLogic.EventType.killEnemies:
                            break;
                        case QuestLogic.EventType.collectInventoryItems:
                            StartCoroutine(CollectInventoryItems(ql));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    [ButtonMethod]
    public void GetAllQuests()
    {
        allQuests = Resources.LoadAll("Quests", typeof(Quest)).Cast<Quest>().ToArray();
    }   //Finds all quests in the resources folder and adds them to allQuests

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

    void ManageQBlankTPs()       //Shows whether any qctps, qatps or qftps are currently onscreen
    {
        if (questCompletedTexts.Count == 0) { qctpOnScreen = false; }
        else { qctpOnScreen = true; }

        if (questAcceptedTexts.Count == 0) { qatpOnScreen = false; }
        else { qatpOnScreen = true; }

        if (questFailedTexts.Count == 0) { qftpOnScreen = false; }
        else { qftpOnScreen = true; }

        if (qatpOnScreen || qftpOnScreen || qctpOnScreen) { qBlankOnScreen = true; }
        else { qBlankOnScreen = false; }
    }

    public void SetupNewQuest(Quest quest) //Any quests that are accepted come here first, and are processed
    {
        checkedForQuestLogicTypes = false;
        if (CheckforSameQuest(quest))
        {
            Debug.LogWarning("Quest already accepted!");
        }
        else if (canAcceptQuest)
        {
            if (quest.questEvents.Count == 0)
            {
                Debug.LogWarning("Quest has no quest events! Quest needs atleast one to function properly");
                return;
            }

            else if (CheckforQuestLogic(quest)) //If this returns true, we're good to go! Let's star setting up that quest!
            {
                player.GetComponent<Charquests>().currentQuests.Add(quest);

                if (lastAcceptedQuest != null)   //If we already have a quest...
                {
                    //ClearOldQuest();
                    ihuiquestmanager.AddAnotherQuest(quest);    //We don't have to add a quest for the first time again, just add another one
                }
                else
                {
                    ihuiquestmanager.SetupNewQuest(quest);      //However if you don't already have a quest, add this one for the first time
                }

                canAcceptQuest = false;
                //questDivider.SetActive(true);                                //Activates UI Elements associated with a quest
                //currentQuestText.gameObject.SetActive(true);
                lastAcceptedQuest = quest;                                     //Assigns incoming quest as current and lets the quest manager track it
                lastAcceptedQuest.questState = Quest.QuestState.CURRENT;       //Sets current quest as current

                SetupQuestEvents();
                GameObject qatp = Instantiate(questAcceptedTextPrefab, questCanvas.transform);  //Creates a Quest Accepted Text Prefab, or qatp
                qatp.GetComponent<QuestAcceptedText>().myQuest = quest;

                bool questAssigned = false;
                if (!questAssigned) { qatp.GetComponent<QuestAcceptedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = lastAcceptedQuest.questName; }    //Assigns the quest name ONCE.

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

                foreach (QuestEvent qe in lastAcceptedQuest.questEvents) //Checks to see if the current quest has any sort of timer attached to it
                {
                    foreach (QuestLogic ql in qe.questLogic)
                    {
                        if (ql.hasEventTimeLimit)
                        {
                            //StartCoroutine(CreateTimeLimitForEvent(ql));
                            CheckforQuestTimer(ql);
                        }
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

    public bool CheckforQuestLogic(Quest myQuest) //Makes sure that all quest events in the quest have quest logic attached
    {
        foreach (var questEvent in myQuest.questEvents)
        {
            if (questEvent.questLogic.Count <= 0)
            {
                Debug.LogWarning(questEvent + " has no quest logic attached! Every quest event must have quest logic attached.");
                if (player.GetComponent<Charquests>().currentQuests.Count > 0)
                { 
                    ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]); 
                }
                return false;
            }
        }
        return true;
    }   

    public void SetupQuestEvents()  //Also sets up quest objects and quest object game object (should they have one)
    {
        //Sets up all quest event prefabs and sets quest events as waiting
        foreach (QuestEvent qe in lastAcceptedQuest.questEvents)       
        {
            //var qep = Instantiate(questEventPrefab, questHolder.transform);     //QEP is a gameObject
            //questEventPrefabs.Add(qep);
            //var qeps = qep.GetComponent<QuestEventPrefabScript>();              //QEPS is the script attached to the QEP
            //qeps.Setup(qe, questHolder, qep);
            qe.status = QuestEvent.EventStatus.WAITING;
            qe.myQuest = lastAcceptedQuest;
            
            //Loop setting up each quest object
            foreach (QuestLogic ql in qe.questLogic)          
            {
                if (ql == null)
                {
                    Debug.LogWarning("There are no quest events associated with this quest event! Please Check the quest to make sure each quest event has a quest object!");
                }
                else
                {
                    ql.Setup(this, qe, lastAcceptedQuest);
                    ql.status = QuestEvent.EventStatus.WAITING;
                }
            }
        }

        QuestEvent firstQE = lastAcceptedQuest.questEvents.First();      //Sets the first quest event as current
        firstQE.status = QuestEvent.EventStatus.CURRENT;

        lastAcceptedQuestEvent = firstQE;
        lastAcceptedQuest.isActive = true;
        //questName.GetComponent<TextMeshProUGUI>().text = currentQuest.questName;
        //questDesc.GetComponent<TextMeshProUGUI>().text = currentQuest.questDesc;

        /*
        //Make loop creating each quest event script and assigning an ID for each quest event.
        foreach (QuestEvent qe in currentQuest.questEvents)
        {
            currentQuest.questEventScripts.Add(CreateQuestEventText(qe).GetComponent<QuestEventScript>());
            //quest.BFS(qe.GetId()); // No longer necessary since order is assigned in inspector now
        }*/
    }

    private void CheckforQuestTimer(QuestLogic questLogic)  //Checks to see if the current quest has a timer. If it does it activates the quest manager timer and sends it to be formatted
    {
        if (questLogic.hasEventTimeLimit)
        {
            countDownTimer.gameObject.SetActive(true);
            countDownTimer.shownTimer.text = "";
            liveTimer = true;
            Debug.Log("Quest demands a timer be made, invoking ManageTimer() to make one");
            ManageTimer(questLogic);
        }
        else if (lastAcceptedQuest.hasTimerForQuest)
        {
            //TODO: Manage the code here for quest timers that refer to quests, and not events
        }
        else
        {
            countDownTimer.shownTimer.text = "";
            countDownTimer.gameObject.SetActive(false);
            liveTimer = false;
            Debug.LogWarning("Quest does not have a timer attached");
        }
    }

    #region Quest Logic Functionality
    private IEnumerator CheckForLocationCollisions(QuestLogic questLogic)   //If the current quest has a location quest logic component, this function will grab the locations and check if the player enters any
    {
        Debug.Log("Checking for location collisions for " + questLogic.qEvent.questEventName + " in " + questLogic.myQuest);
        if (lastAcceptedQuest != null)
        {
            //Check for player collision
            Collider2D[] hitColliders = Physics2D.OverlapAreaAll(questLogic.areaStart, questLogic.areaEnd);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    questLogic.status = QuestEvent.EventStatus.DONE;
                    UpdateQuestsOnCompletion(questLogic.qEvent, QuestEvent.EventStatus.DONE);
                    yield break;
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(CheckForLocationCollisions(questLogic));
    }

    public IEnumerator CheckForPlayerAspect(QuestLogic questLogic)
    {
        if (questLogic.playerAspectToMonitor == QuestLogic.PlayerAspectMonitorType.money)
        {
            Debug.Log("Checking money player aspect for " + questLogic.qEvent.questEventName);
            questLogic.moneyCounter = questLogic.moneyCounter = player.GetComponent<Charpickup_inventory>().money;  //Sets the counter to how much cash you're on right now
            if (!questLogic.amountsSet)
            {
                questLogic.moneyRequired = questLogic.moneyCounter + questLogic.moneyTarget;
                questLogic.amountsSet = true;
            }
            if (questLogic.moneyCounter >= questLogic.moneyRequired && !questLogic.eventCompleted)
            {
                questLogic.eventCompleted = true;
                questLogic.qEvent.status = QuestEvent.EventStatus.DONE;
                UpdateQuestsOnCompletion(questLogic.qEvent, QuestEvent.EventStatus.DONE);
                yield break;
            }
        }

        if (questLogic.playerAspectToMonitor == QuestLogic.PlayerAspectMonitorType.level)
        {
            //Debug.Log("Checking level player aspect for " + questLogic.qEvent.questEventName);
            questLogic.levelCounter = player.GetComponent<Charhealth>().level;  //Sets the counter to how what level you are currently
            if (!questLogic.amountsSet)
            {
                questLogic.amountsSet = true;
            }
            if (questLogic.levelCounter >= questLogic.levelTarget && !questLogic.eventCompleted)
            {
                questLogic.eventCompleted = true;
                questLogic.qEvent.status = QuestEvent.EventStatus.DONE;
                UpdateQuestsOnCompletion(questLogic.qEvent, QuestEvent.EventStatus.DONE);
                yield break;
            }
        }

        if (questLogic.playerAspectToMonitor == QuestLogic.PlayerAspectMonitorType.health)
        {
            //Debug.Log("Checking health player aspect for " + questLogic.qEvent.questEventName);
            questLogic.healthCounter = player.GetComponent<Charhealth>().currentHealth;  //Sets the counter to how what level you are currently
            if (!questLogic.amountsSet)
            {
                questLogic.amountsSet = true;
            }
            if (questLogic.healthCounter >= questLogic.healthTarget && !questLogic.eventCompleted)
            {
                questLogic.eventCompleted = true;
                questLogic.qEvent.status = QuestEvent.EventStatus.DONE;
                UpdateQuestsOnCompletion(questLogic.qEvent, QuestEvent.EventStatus.DONE);
                yield break;
            }
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(CheckForPlayerAspect(questLogic));
    }

    public IEnumerator CollectInventoryItems(QuestLogic questLogic)
    {
        //Check if you have the item. If you don't recheck if you have the item every 0.5 seconds.
        //If you have the item, reference the amountHas value. Set itemCounter equal to it
        //Compare it to itemAmountRequired for completion
        if (!inventory.items.Contains(questLogic.itemToCollect)) //If the specified item is not in the Inventory
        {
            StartCoroutine(CheckforInventoryItem(questLogic));
        }
        else
        {
            questLogic.itemCounter = questLogic.itemToCollect.amountHas;
            if (!questLogic.amountsSet)
            {
                questLogic.itemAmountRequired = questLogic.itemCounter + questLogic.itemAmountTarget;
                questLogic.amountsSet = true;
            }
            if (questLogic.itemCounter >= questLogic.itemAmountRequired && !questLogic.eventCompleted)
            {
                questLogic.eventCompleted = true;
                questLogic.status = QuestEvent.EventStatus.DONE;
                UpdateQuestsOnCompletion(questLogic.qEvent, QuestEvent.EventStatus.DONE);
                yield break;
            }
        }
    }

    public IEnumerator CheckforInventoryItem(QuestLogic questLogic)
    {
        if (!inventory.items.Contains(questLogic.itemToCollect))
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(CheckforInventoryItem(questLogic));
        }
        else
        {
            CollectInventoryItems(questLogic);
        }
    }
    #endregion

    public void ManageTimer(QuestLogic questLogic)
    {
        Debug.Log("Starting ManageTimer()");
        if (questTimers.Count > 0)  //If quest timers exist
        {
            foreach (GameObject timer in questTimers)   //Check them all to make sure none of them are associated with the quest current quest
            {
                GameObject newTimer = Instantiate(questTimerPrefab, questTimerHolder.transform);    //Creates a new quest Timer that is in charge of counting down and keeping track of time and when its completed
                questTimers.Add(newTimer);
                QuestTimer timerScript = newTimer.GetComponent<QuestTimer>();
                timerScript.myQuest = lastAcceptedQuest;
                timerScript.timerTargetTime = questLogic.timerTargetTime;            //Sets the starting time for the timer script to the quest's amount of time
                liveTimer = true;
                StartCoroutine(AssignTimer(timerScript));

                if (lastAcceptedQuest.hasTimerForQuest)
                {
                    timerScript.TypeofTimer = QuestTimer.Timertype.QUEST;
                    timerScript.myQuest = lastAcceptedQuest;
                    return;
                }
                else if (questLogic.hasEventTimeLimit)
                {
                    timerScript.TypeofTimer = QuestTimer.Timertype.EVENT;
                    timerScript.myQuestEvent = lastAcceptedQuestEvent;

                    if(questLogic.qEvent.order == 0)
                    {
                        questLogic.status = QuestEvent.EventStatus.CURRENT;
                    }
                    return;
                }
            }
        }
        else
        {
            Debug.Log("No quest timers exist at all, instantiating one...");
            GameObject newTimer = Instantiate(questTimerPrefab, questTimerHolder.transform);    //Creates a new quest Timer that is in charge of counting down and keeping track of time and when its completed
            questTimers.Add(newTimer);
            QuestTimer timerScript = newTimer.GetComponent<QuestTimer>();
            timerScript.myQuest = lastAcceptedQuest;
            timerScript.timerTargetTime = questLogic.timerTargetTime;                           //Sets the starting time for the timer script to the quest's amount of time
            liveTimer = true;
            StartCoroutine(AssignTimer(timerScript));
            
            if (lastAcceptedQuest.hasTimerForQuest)
            {
                timerScript.TypeofTimer = QuestTimer.Timertype.QUEST;
                timerScript.myQuest = lastAcceptedQuest;
            }
            else if (questLogic.hasEventTimeLimit)
            {
                timerScript.TypeofTimer = QuestTimer.Timertype.EVENT;
                timerScript.myQuestEvent = lastAcceptedQuestEvent;
                if (questLogic.qEvent.order == 0)
                {
                    questLogic.status = QuestEvent.EventStatus.CURRENT;
                }
            }
            StartCoroutine(CheckforTimerDone(questLogic));
        }        
    }

    private IEnumerator AssignTimer(QuestTimer timerScript)
    {
        //Debug.Log("Updating formatted timer");
        yield return new WaitForSeconds(0.1f);
        if (timerScript.myQuest == lastAcceptedQuest)
        {
            countDownTimer.shownTimer.text = timerScript.formattedTime;             //Sets the timer text onscreen to whatever the timerscript says as
        }
        if (liveTimer) { StartCoroutine(AssignTimer(timerScript)); }            //Loops this coroutine to continue assigning the text if there is a text available
    }

    private IEnumerator CheckforTimerDone(QuestLogic questLogic) //Constantly checks every timer in the questTimers list to see if its done. If it is, fail that quest or event
    {
        yield return new WaitForSeconds(0.5f);
        if (questTimers.Count > 0)
        {
            foreach (GameObject timer in questTimers)
            {
                if (timer.GetComponent<QuestTimer>().timerDone == true && lastAcceptedQuest != null) //If the timer hits 0
                {
                    //ADD QUEST TIMER STUFF HERE
                    if (questLogic.hasEventTimeLimit && lastAcceptedQuest != null)
                    {
                        UpdateQuestsOnCompletion(questLogic.qEvent, QuestEvent.EventStatus.FAILED);
                        countDownTimer.gameObject.SetActive(false);
                        liveTimer = false;
                        Destroy(timer);
                    }

                    questTimers.Remove(timer);
                    Destroy(timer);

                    foreach (QuestEvent n in lastAcceptedQuest.questEvents)
                    {
                        //If this event is the next in order
                        if (n.order == (questLogic.qEvent.order + 1))
                        {
                            //Make the next in line available for completion
                            n.status = QuestEvent.EventStatus.CURRENT;
                            n.questLogic[0].status = QuestEvent.EventStatus.CURRENT;
                            lastAcceptedQuestEvent = n;
                            yield break;
                        }
                    }
                    break;
                }
            }
            StartCoroutine(CheckforTimerDone(questLogic));
        }
        else
        {
            yield break;
        }
    }

/*    GameObject CreateQuestEventText(QuestEvent e) //Creates a new text for each currentQuest event requested
    {
        GameObject b = Instantiate(questEventPrefab, questHolder.transform);
        b.GetComponent<QuestEventPrefabScript>().currentEventText.text = "";
        //b.GetComponent<QuestEventScript>().Setup(e, questHolder);
        questEventPrefabs.Add(b);
        return b;
    }*/

/*    public void ClearOldQuest()
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
    }*/

    private void ReAddQuest(Quest quest)
    {
        //questDivider.SetActive(true);
        //currentQuestText.gameObject.SetActive(true);
        lastAcceptedQuest = quest;
        lastAcceptedQuest.questState = Quest.QuestState.CURRENT;

        if (lastAcceptedQuest.hasTimerForQuest) { countDownTimer.gameObject.SetActive(true); }
        foreach (GameObject timer in questTimers)   //Check them all to which one is associated with the quest you're readding
        {
            if (timer.GetComponent<QuestTimer>().myQuest == quest)
            {
                //Debug.Log("A timer for this quest exists!");
                StartCoroutine(AssignTimer(timer.GetComponent<QuestTimer>()));
                liveTimer = true;
            }
        }

        foreach (QuestEvent qe in lastAcceptedQuest.questEvents)
        {
            qe.status = QuestEvent.EventStatus.WAITING;
        }
        QuestEvent firstQE = lastAcceptedQuest.questEvents.First();
        firstQE.status = QuestEvent.EventStatus.CURRENT;
        lastAcceptedQuestEvent = firstQE;
        lastAcceptedQuest.isActive = true;
        //questName.GetComponent<TextMeshProUGUI>().text = currentQuest.questName;
        //questDesc.GetComponent<TextMeshProUGUI>().text = currentQuest.questDesc;

        SetupQuestEvents();
    }

    public void UpdateQuestsOnCompletion(QuestEvent qe, QuestEvent.EventStatus status) //Update quests everytime something is done and makes sure the next event is ready. If it's the last it will finish the quest
    {
        if (qe != null)
        {
            qe.status = status;
            qe.questLogic[0].status = status;
            ihuiquestmanager.UpdateQuestSteppie(qe, status);
        }

        if (qe.failWholeQuest && qe.status == QuestEvent.EventStatus.FAILED)
        {
            StopCoroutine(CheckforTimerDone(qe.questLogic[0]));
            StartCoroutine(FailCurrentQuest());
            //questDivider.SetActive(false);
            countDownTimer.gameObject.SetActive(false);
            liveTimer = false;
            currentQuestText.gameObject.SetActive(false);
            return;
        }

        if (lastAcceptedQuest.questEvents.Count > 0)     //If there are quest events...
        {
            foreach (QuestEvent n in lastAcceptedQuest.questEvents)
            {
                
                if (n.order == (qe.order + 1))      //If this event is the next in order
                {
                    //Make the next in line available for completion
                    n.status = QuestEvent.EventStatus.CURRENT;
                    n.questLogic[0].status = QuestEvent.EventStatus.CURRENT;
                    lastAcceptedQuestEvent = n;
                    return;
                }
            }
        }

        Quest thisQeQuest = qe.myQuest;

        //If the last quest event gets completed, that means the quest is done! Call that function!
        if (thisQeQuest.questEvents.Last().status == QuestEvent.EventStatus.DONE)
        {
            StopCoroutine(CheckforTimerDone(qe.questLogic[0]));
            StartCoroutine(CompleteSpecificQuest(thisQeQuest));
            //questDivider.SetActive(false);
            currentQuestText.gameObject.SetActive(false);
            countDownTimer.gameObject.SetActive(false);
            liveTimer = false;
        }
        else if (thisQeQuest.questEvents.Last().status == QuestEvent.EventStatus.FAILED)    //Right now it fails if only the LAST quest event is failed. Add functionality for flexibility
        {
            StopCoroutine(CheckforTimerDone(qe.questLogic[0]));
            StartCoroutine(FailSpecificQuest(thisQeQuest));
            //questDivider.SetActive(false);
            countDownTimer.gameObject.SetActive(false);
            liveTimer = false;
            currentQuestText.gameObject.SetActive(false);
        }
    }

    public IEnumerator CompleteCurrentQuest()
    {
        //questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
        liveTimer = false;
        
        CleanUpTimers();

        GameObject qctp = Instantiate(questCompletedTextPrefab, questCanvas.transform);     //Creates a Quest Completed Text Prefab, or qctp
        qctp.GetComponent<QuestCompletedText>().myQuest = lastAcceptedQuest;
        bool questAssigned = false;
        if (!questAssigned) { qctp.GetComponent<QuestCompletedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = lastAcceptedQuest.questName; }   //Assigns the quest name ONCE.
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
        //Checks for duplicate qctps
        foreach (var qcto in questCompletedTextsOffscreen)
        {
            if (qcto.GetComponent<QuestCompletedText>().myQuest == qctp.GetComponent<QuestCompletedText>().myQuest && canAcceptQuest)
            {
                questCompletedTextsOffscreen.Remove(qctp);
                Destroy(qctp);
                Debug.LogWarning("Tried to make more than one instance of a Quest Completed Text Prefab for a Quest where one was already created! Stopped spawning 'em for ya");
                break;
            }
        }
/*
        //Gets rid of all quest event prefabs on screen and clears the list
        foreach (GameObject qep in questEventPrefabs)
        {
            Destroy(qep);
        }
        questEventPrefabs.Clear();*/

        yield return new WaitForSeconds(0.1f);
        //questName.GetComponent<TextMeshProUGUI>().text = null;
        //questDesc.GetComponent<TextMeshProUGUI>().text = null;
        lastAcceptedQuest.isActive = false;
        player.GetComponent<Charquests>().currentQuests.Remove(lastAcceptedQuest);
        player.GetComponent<Charquests>().completedQuests.Add(lastAcceptedQuest);
        ihuiquestmanager.RemoveQuestOnCompletion(lastAcceptedQuest);
        lastAcceptedQuest.questState = Quest.QuestState.COMPLETED;

        lastAcceptedQuestEvent = null;
        lastAcceptedQuest = null;    //Sets the current quest to nothing

        if (player.GetComponent<Charquests>().currentQuests.Count != 0)     //If there are more quests yet to be completed, add the next one back
        {
            ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }

    public IEnumerator CompleteSpecificQuest(Quest quest)
    {
        //questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
        liveTimer = false;

        CleanUpTimers();

        GameObject qctp = Instantiate(questCompletedTextPrefab, questCanvas.transform);     //Creates a Quest Completed Text Prefab, or qctp
        qctp.GetComponent<QuestCompletedText>().myQuest = quest;

        //Checks for duplicate qctps
        foreach (var qcto in questCompletedTextsOffscreen)
        {
            if (qcto.GetComponent<QuestCompletedText>().myQuest == qctp.GetComponent<QuestCompletedText>().myQuest && canAcceptQuest)
            {
                //questCompletedTextsOffscreen.Remove(qctp);
                Destroy(qctp);
                Debug.LogWarning(qcto.GetComponent<QuestCompletedText>().myQuest + " and " + qctp.GetComponent<QuestCompletedText>().myQuest + " are the same quest. Tried to make more than one instance of a Quest Completed Text Prefab for a Quest where one was already created! Stopped spawning 'em for ya");
                break;
            }
        }

        bool questAssigned = false;
        if (!questAssigned) { qctp.GetComponent<QuestCompletedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = quest.questName; }   //Assigns the quest name ONCE.
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
        
        /*
                //Gets rid of all quest event prefabs on screen and clears the list
                foreach (GameObject qep in questEventPrefabs)
                {
                    Destroy(qep);
                }
                questEventPrefabs.Clear();*/

        yield return new WaitForSeconds(0.1f);
        //questName.GetComponent<TextMeshProUGUI>().text = null;
        //questDesc.GetComponent<TextMeshProUGUI>().text = null;
        quest.isActive = false;
        player.GetComponent<Charquests>().currentQuests.Remove(quest);
        player.GetComponent<Charquests>().completedQuests.Add(quest);
        ihuiquestmanager.RemoveQuestOnCompletion(quest);
        quest.questState = Quest.QuestState.COMPLETED;

        lastAcceptedQuestEvent = null;
        lastAcceptedQuest = null;    //Sets the current quest to nothing

        if (player.GetComponent<Charquests>().currentQuests.Count != 0)     //If there are more quests yet to be completed, add the next one back
        {
            ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }

    public IEnumerator FailCurrentQuest()       //TODO: Expand this, make sure quest is added to appropriate list in Charquests both in this function and the one above.
    {
        //questDivider.SetActive(false);
        currentQuestText.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
        liveTimer = false;

        CleanUpTimers();

        GameObject qftp = Instantiate(questFailedTextPrefab, questCanvas.transform);     //Creates a Quest Failed Text Prefab, or qftp
        qftp.GetComponent<QuestCompletedText>().myQuest = lastAcceptedQuest;
        bool questAssigned = false;
        if (!questAssigned) { qftp.GetComponent<QuestFailedText>().myQuestName.GetComponent<TextMeshProUGUI>().text = lastAcceptedQuest.questName; }   //Assigns the quest name ONCE.
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
/*        foreach (GameObject qep in questEventPrefabs)
        {
            Destroy(qep);
        }
        questEventPrefabs.Clear();
*/
        yield return new WaitForSeconds(0.1f);
        //questName.GetComponent<TextMeshProUGUI>().text = null;
        //questDesc.GetComponent<TextMeshProUGUI>().text = null;
        lastAcceptedQuest.isActive = false;
        player.GetComponent<Charquests>().currentQuests.Remove(lastAcceptedQuest);
        player.GetComponent<Charquests>().failedQuests.Add(lastAcceptedQuest);
        lastAcceptedQuest.questState = Quest.QuestState.FAILED;

        lastAcceptedQuestEvent = null;
        lastAcceptedQuest = null;    //Sets the current quest to nothing

        if (player.GetComponent<Charquests>().currentQuests.Count != 0)     //If there are more quests yet to be completed, add the next one back
        {
           ReAddQuest(player.GetComponent<Charquests>().currentQuests[0]);
        }
    }

    public IEnumerator FailSpecificQuest(Quest quest)
    {
        //questDivider.SetActive(false);
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
        qftp.GetComponent<QuestCompletedText>().myQuest = quest;
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
/*        foreach (GameObject qep in questEventPrefabs)
        {
            Destroy(qep);
        }
        questEventPrefabs.Clear();*/

        yield return new WaitForSeconds(0.1f);
        //questName.GetComponent<TextMeshProUGUI>().text = null;
        //questDesc.GetComponent<TextMeshProUGUI>().text = null;
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

    public void CleanUpTimers()     //Cleans up timers when quests finish
    {
        if (questTimers.Count > 0)  //If quest timers exist
        {
            foreach (GameObject timer in questTimers)   //Check them all for a timer associated with the current quest
             {
                 if (timer.GetComponent<QuestTimer>().myQuest == lastAcceptedQuest)
                 {
                     questTimers.Remove(timer);          //Remove from list
                     Destroy(timer);
                 }
             }

            
        }
        questTimers.RemoveAll(i => i.GetComponent<QuestTimer>().myQuest == lastAcceptedQuest);
    }

    private void OnDrawGizmos()
    {
        //Shows areas for quest location quest events
        if (lastAcceptedQuest != null && showLocationsDebug)
        {
            foreach (QuestEvent qe in lastAcceptedQuest.questEvents)
            {
                foreach (QuestLogic ql in qe.questLogic)
                {
                    if (ql.type == QuestLogic.EventType.location)
                    {
                        Vector3 bottomLeft = new Vector3(ql.areaStart.x, ql.areaStart.y, ql.areaStart.z);
                        Vector3 topRight = new Vector3(ql.areaEnd.x, ql.areaEnd.y, ql.areaEnd.z);

                        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
                        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);

                        Gizmos.color = Color.magenta;
                        Gizmos.DrawWireSphere(bottomLeft, 0.05f);
                        Gizmos.DrawWireSphere(topRight, 0.05f);

                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(bottomRight, 0.05f);
                        Gizmos.DrawWireSphere(topLeft, 0.05f);

                        Gizmos.color = Color.magenta;
                        Gizmos.DrawLine(bottomLeft, topRight);
                        Gizmos.DrawLine(bottomRight, topLeft);

                        Gizmos.DrawLine(topLeft, topRight);
                        Gizmos.DrawLine(topRight, bottomRight);
                        Gizmos.DrawLine(bottomRight, bottomLeft);
                        Gizmos.DrawLine(bottomLeft, topLeft);
                    }
                }
            }
        }
    }
}
