using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class IHUIQuestManager : MonoBehaviour
{
    [Separator("Variables to Assign")]
    Gamemanager gamemanager;
    Charquests charquests;
    Charcontrol charcontrol;
    QuestManager QuestManager;

    public TextMeshProUGUI onScreenQuestName;
    public TextMeshProUGUI onScreenQuestDesc;
    public TextMeshProUGUI ihuiQuestsTotal;
    public TextMeshProUGUI ihuiQuestsCurrent;

    public GameObject questSteppiePrefab;
    public GameObject contentHolder;

    [Separator("Live variables")]
    public Quest currentQuest;
    private int currentQuestArrayNumber;
    public List<Quest> currentQuests;
    public List<GameObject> questSteppies;
    public int ihuiQuestsTotalNumber;
    public int ihuiQuestsCurrentNumber;

    // Start is called before the first frame update
    void Start()
    {
        gamemanager = Gamemanager.instance;
        QuestManager = gamemanager.questManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (QuestManager.currentQuest == null)
        {
            onScreenQuestName.text = "No Quests accepted!";
            onScreenQuestDesc.text = "";
        }
    }

    public void SetupNewQuest(Quest quest)
    {
        currentQuests.Add(quest);
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
            steppieScript.myText.text = qEvent.description;
            steppieScript.myQuest = quest;
            steppieScript.myQuestEvent = qEvent;

            questSteppies.Add(newQuestSteppie);
        }
    }

    public void ShowNewQuest(Quest quest)
    {
        currentQuest = quest;
        onScreenQuestName.text = quest.questName;
        onScreenQuestDesc.text = quest.questDesc;

        ihuiQuestsTotal.text = ("0" + ihuiQuestsTotalNumber);
        ihuiQuestsCurrent.text = ("0" + ihuiQuestsCurrentNumber);

        foreach (var qEvent in quest.questEvents)
        {
            var newQuestSteppie = Instantiate(questSteppiePrefab, contentHolder.transform);
            IHUIQuestSteppieScript steppieScript = newQuestSteppie.GetComponent<IHUIQuestSteppieScript>();
            steppieScript.myText.text = qEvent.description;
            steppieScript.myQuest = quest;
            steppieScript.myQuestEvent = qEvent;

            questSteppies.Add(newQuestSteppie);
        }
    }

    [ButtonMethod]
    public void ClearCurrentQuest()
    {
        currentQuest = null;
        onScreenQuestName.text = "";
        onScreenQuestDesc.text = "";

        ihuiQuestsTotalNumber = 1;
        ihuiQuestsCurrentNumber = 1;
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

    public void AddAnotherQuest(Quest quest)    //This occurs when the player already has a quest, and accepts another. Creates another page inside the Quest canvas
    {
        ihuiQuestsTotalNumber++;
        ihuiQuestsTotal.text = ("0" + ihuiQuestsTotalNumber);
        currentQuests.Add(quest);
    }

    public void QuestCanvasShowNextQuest()
    {
        int nextQuestArrayNumber = currentQuestArrayNumber++;
        Quest newQuestToShow = currentQuests[nextQuestArrayNumber];
        currentQuestArrayNumber = currentQuestArrayNumber++;
        ClearCurrentQuest();
        ShowNewQuest(newQuestToShow);
        currentQuest = newQuestToShow;
    }

    public void QuestCanvasShowPreviousQuest()
    {
        Quest newQuestToShow = currentQuests[currentQuestArrayNumber--];
        currentQuestArrayNumber = currentQuestArrayNumber--;
        ClearCurrentQuest();
        ShowNewQuest(newQuestToShow);
        currentQuest = newQuestToShow;
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
