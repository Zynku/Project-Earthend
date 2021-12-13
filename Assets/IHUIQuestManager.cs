using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;
using System.Linq;
using UnityEngine.UI;

public class IHUIQuestManager : MonoBehaviour
{
    [Separator("Variables to Assign")]
    GameManager gamemanager;
    Charquests charquests;
    Charcontrol charcontrol;
    QuestManager QuestManager;

    public TextMeshProUGUI onScreenQuestName;
    public TextMeshProUGUI onScreenQuestDesc;
    public TextMeshProUGUI ihuiQuestsTotal;
    public TextMeshProUGUI ihuiQuestsCurrent;
    public Image questKindColorImage;
    public TextMeshProUGUI questKindText;

    public GameObject questSteppiePrefab;
    public GameObject contentHolder;

    [Separator("Live variables")]
    public Quest currentQuest;
    private int currentQuestArrayNumber;
    public List<Quest> currentQuests;
    public List<Quest> failedQuests;
    public List<Quest> completedQuests;
    public List<GameObject> questSteppies;
    public int ihuiQuestsTotalNumber;       //How many quests are in the current list?
    public int ihuiQuestsCurrentNumber;     //Which quest in the list is currently active?

    public enum questPageType
    {
        currentQuests,
        completedQuests,
        failedQuests
    };
    public questPageType qPType;

    private bool beenSetup = false;

    // Start is called before the first frame update
    void Start()
    {
        gamemanager = GameManager.instance;
        QuestManager = gamemanager.questManager;
        charquests = gamemanager.Player.GetComponent<Charquests>();
        currentQuests = charquests.currentQuests;
        failedQuests = charquests.failedQuests;
        completedQuests = charquests.completedQuests;
        qPType = questPageType.currentQuests;

        //onScreenQuestName.text = "No Quests!";
        //onScreenQuestDesc.text = "No Quests!";

        beenSetup = true;
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.UpArrow))
        {
            QuestCanvasShowPreviousQuest();
        }

        if (this.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.DownArrow))
        {
            QuestCanvasShowNextQuest();
        }

        switch (qPType)
        {
            case questPageType.currentQuests:
                questKindColorImage.color = new Color(1, 1, 1, 0.2f);
                questKindText.text = "Current Quests";
                break;
            case questPageType.completedQuests:
                questKindColorImage.color = new Color(0.4f, 1, 0.2f, 0.2f);
                questKindText.text = "Completed Quests";
                break;
            case questPageType.failedQuests:
                questKindColorImage.color = new Color(1, 0.2f, 0.2f, 0.2f);
                questKindText.text = "Failed Quests";
                break;
            default:
                break;
        }
    }

    public void SwitchToCurrentQuestsPage()
    {
        if (qPType == questPageType.currentQuests)
        {
            return;
        }
        else
        {
            qPType = questPageType.currentQuests;
            ShowNewListOfQuests();
        }
        
        if (currentQuests.Count == 0)
        {
            ClearCurrentQuest();
        }
    }

    public void SwitchToCompletedQuestsPage()
    {
        if (qPType == questPageType.completedQuests)
        {
            return;
        }
        else
        {
            qPType = questPageType.completedQuests;
            ShowNewListOfQuests();
        }
        
        if (completedQuests.Count == 0)
        {
            ClearCurrentQuest();
        }
    }

    public void SwitchToFailedQuestsPage()
    {
        if (qPType == questPageType.failedQuests)
        {
            return;
        }
        else
        {
            qPType = questPageType.failedQuests;
            ShowNewListOfQuests();
        }

        if (failedQuests.Count == 0)
        {
            ClearCurrentQuest();
        }
    }

    public void SetupNewQuest(Quest quest)  //Called for the first quest accepted
    {
        questPageType previousqpType = qPType;
        qPType = questPageType.currentQuests;       //Switch to currentQuests

        if (!beenSetup) { Start(); }
        //currentQuests.Add(quest);
        currentQuest = currentQuests[0];
        currentQuestArrayNumber = 0;
        onScreenQuestName.text = quest.questName;
        onScreenQuestDesc.text = quest.questDesc;

        ihuiQuestsTotalNumber = 1;
        ihuiQuestsCurrentNumber = 1;
        ihuiQuestsTotal.text = ("0" + ihuiQuestsTotalNumber);
        ihuiQuestsCurrent.text = ("0" + ihuiQuestsCurrentNumber);

        foreach (var qEvent in quest.questEvents)
        {
            var newQuestSteppie = Instantiate(questSteppiePrefab, contentHolder.transform);
            IHUIQuestSteppieScript steppieScript = newQuestSteppie.GetComponent<IHUIQuestSteppieScript>();
            steppieScript.myText.text = qEvent.questEventName;
            steppieScript.myQuest = quest;
            steppieScript.myQuestEvent = qEvent;

            questSteppies.Add(newQuestSteppie);
        }
    }

    public void ShowNewQuest(Quest quest)       //Called for subsequent quests accepted after the first
    {
        questPageType previousqpType = qPType;
        qPType = questPageType.currentQuests;       //Switch to currentQuests

        currentQuest = quest;
        onScreenQuestName.text = quest.questName;
        onScreenQuestDesc.text = quest.questDesc;

        ihuiQuestsTotal.text = ("0" + ihuiQuestsTotalNumber);
        ihuiQuestsCurrent.text = ("0" + ihuiQuestsCurrentNumber);

        foreach (var qEvent in quest.questEvents)
        {
            var newQuestSteppie = Instantiate(questSteppiePrefab, contentHolder.transform);
            IHUIQuestSteppieScript steppieScript = newQuestSteppie.GetComponent<IHUIQuestSteppieScript>();
            steppieScript.myText.text = qEvent.questEventName;
            steppieScript.myQuest = quest;
            steppieScript.myQuestEvent = qEvent;

            questSteppies.Add(newQuestSteppie);
        }
    }

    public void ReFillQuestList(int qNumber, List<Quest> list)    //Called when switching lists between current, completed, and failed quests
    {
        
        if (list == currentQuests) { currentQuest = list[0]; currentQuestArrayNumber = 0; }

        onScreenQuestName.text = list[qNumber].questName;
        onScreenQuestDesc.text = list[qNumber].questDesc;

        ihuiQuestsTotalNumber = list.Count;
        ihuiQuestsCurrentNumber = 1;

        ihuiQuestsTotal.text = ("0" + list.Count);
        ihuiQuestsCurrent.text = ("0" + ihuiQuestsCurrentNumber);

        foreach (var qEvent in list[qNumber].questEvents)
        {
            var newQuestSteppie = Instantiate(questSteppiePrefab, contentHolder.transform);
            IHUIQuestSteppieScript steppieScript = newQuestSteppie.GetComponent<IHUIQuestSteppieScript>();
            steppieScript.myText.text = qEvent.questEventName;
            steppieScript.myQuest = list[qNumber];
            steppieScript.myQuestEvent = qEvent;

            questSteppies.Add(newQuestSteppie);
        }
    }

    public void ShowNewListOfQuests()
    {
        switch (qPType)
        {
            case questPageType.currentQuests:
                for (int i = 0; i < currentQuests.Count; i++)
                {
                    ClearCurrentQuest();
                    ReFillQuestList(i, currentQuests);
                }
                break;
            case questPageType.completedQuests:
                for (int i = 0; i < completedQuests.Count; i++)
                {
                    ClearCurrentQuest();
                    ReFillQuestList(i, completedQuests);
                }
                break;
            case questPageType.failedQuests:
                for (int i = 0; i < failedQuests.Count; i++)
                {
                    ClearCurrentQuest();
                    ReFillQuestList(i, failedQuests);
                }
                break;
            default:
                break;
        }
    }

    [ButtonMethod]
    public void ClearCurrentQuest()
    {
        currentQuest = null;
        onScreenQuestName.text = "";
        onScreenQuestDesc.text = "";

        ihuiQuestsTotal.text = "00";
        ihuiQuestsCurrent.text = "00";


        foreach (var steppie in questSteppies)
        {
            Destroy(steppie);
        }
        questSteppies.Clear();
    }

    public void UpdateQuestSteppie(QuestEvent qe, QuestEvent.EventStatus status)
    {
        foreach (var steppie in questSteppies)
        {
            if (steppie.GetComponent<IHUIQuestSteppieScript>().myQuestEvent == qe)
            {
                GameObject steppieToUpdate = steppie;
                IHUIQuestSteppieScript steppieScript = steppieToUpdate.GetComponent<IHUIQuestSteppieScript>();

                switch (status)
                {
                    case QuestEvent.EventStatus.WAITING:
                        break;
                    case QuestEvent.EventStatus.CURRENT:
                        break;
                    case QuestEvent.EventStatus.DONE:
                        steppieScript.tick.SetActive(true);
                        break;
                    case QuestEvent.EventStatus.FAILED:
                        steppieScript.x.SetActive(true);
                        break;
                    default:
                        break;
                }

                return;
            }
        }
    }

    public void CheckQuestEvents()      //Makes sure that quest steppies are up to date with their corresponding quest events
    {
        foreach (GameObject questSteppie in questSteppies)
        {
            GameObject steppieToUpdate = questSteppie;
            IHUIQuestSteppieScript steppieScript = steppieToUpdate.GetComponent<IHUIQuestSteppieScript>();

            switch (steppieScript.myQuestEvent.status)
            {
                case QuestEvent.EventStatus.WAITING:
                    break;
                case QuestEvent.EventStatus.CURRENT:
                    break;
                case QuestEvent.EventStatus.DONE:
                    steppieScript.tick.SetActive(true);
                    break;
                case QuestEvent.EventStatus.FAILED:
                    steppieScript.x.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    public void AddAnotherQuest(Quest quest)    //This occurs when the player already has a quest, and accepts another. Creates another page inside the Quest canvas
    {
        ihuiQuestsTotalNumber++;
        ihuiQuestsTotal.text = ("0" + ihuiQuestsTotalNumber);
        //currentQuests.Add(quest);
    }

    public void QuestCanvasShowNextQuest()
    {
        switch (qPType)
        {
            case questPageType.currentQuests:
                if (currentQuestArrayNumber + 1 == currentQuests.Count)
                {
                    //Debug.Log("End of quest list reached, not scrolling");
                    return;
                }
                else
                {
                    int nextQuestArrayNumber = currentQuestArrayNumber + 1;
                    Quest newQuestToShow = currentQuests[nextQuestArrayNumber];
                    currentQuestArrayNumber = nextQuestArrayNumber;
                    ihuiQuestsCurrentNumber++;
                    ClearCurrentQuest();
                    ShowNewQuest(newQuestToShow);
                    CheckQuestEvents();
                    currentQuest = newQuestToShow;
                }
                break;
            case questPageType.completedQuests:
                break;
            case questPageType.failedQuests:
                break;
            default:
                break;
        }
    }

    public void QuestCanvasShowPreviousQuest()
    {
        switch (qPType)
        {
            case questPageType.currentQuests:
                if (currentQuestArrayNumber + 1 == 1)
                {
                    //Debug.Log("Beginning of quest list reached, not scrolling");
                    return;
                }
                else
                {
                    int nextQuestArrayNumber = currentQuestArrayNumber - 1;
                    Quest newQuestToShow = currentQuests[nextQuestArrayNumber];
                    currentQuestArrayNumber = nextQuestArrayNumber;
                    ihuiQuestsCurrentNumber--;
                    ClearCurrentQuest();
                    ShowNewQuest(newQuestToShow);
                    CheckQuestEvents();
                    currentQuest = newQuestToShow;
                }
                break;
            case questPageType.completedQuests:
                break;
            case questPageType.failedQuests:
                break;
            default:
                break;
        }
    }

    public void RemoveQuestOnCompletion(Quest quest)
    {
        ihuiQuestsTotalNumber--;
        ihuiQuestsTotal.text = ("0" + ihuiQuestsTotalNumber);
/*        Quest questToRemove = currentQuests.Where(questToRemove => questToRemove.questName == quest.questName).FirstOrDefault();
        currentQuests.Remove(questToRemove);*/
        
        if (quest == currentQuest)
        {
            QuestCanvasShowPreviousQuest();
        }

        if(currentQuests.Count == 0)
        {
            ClearCurrentQuest();
            onScreenQuestName.text = "No Quests!";
            onScreenQuestDesc.text = "No Quests!";
        }
    }

    public int? FindQuestArrayNumber(Quest quest)
    {
        for (int i = 0; i < currentQuests.Count; i++)
        {
            if (currentQuests[i] == quest)
            {
                return i;
            }
        }
        return null;
    }

    public int? FindPreviousQuestArrayNumber(Quest quest)
    {
        for (int i = 0; i < currentQuests.Count; i++)
        {
            if (currentQuests[i] == quest)
            {
                return i--;
            }
        }
        return null;
    }
}
