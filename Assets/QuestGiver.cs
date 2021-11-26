using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class QuestGiver : MonoBehaviour
{
    [Separator("Quest Variables")]
    public bool acceptQuestByProximity = false;           //Can you just walk up to this NPC, press Interact and get a quest?
    public bool acceptQuestByName;                        //Gives the player a specific quest by its name. Not case sensitive, whitespace sensitive though
    [ConditionalField(nameof(acceptQuestByName))] public string questName;      //The name of the quest you want to give the player
    [ConditionalField(nameof(acceptQuestByName), true)] public Quest myQuest;   //The quest you want to give the player
    public bool changeDialogueOnQuestUpdate;            //Does this NPC change their dialogue if you update the quest in some way?
    [ConditionalField(nameof(changeDialogueOnQuestUpdate))] public string dialogueTreeToSwitchTo;        //Which dialogue tree will the NPC switch to if the designated quest is updated
    [ConditionalField(nameof(changeDialogueOnQuestUpdate))] public int whichQuestEventOrderToChangeOn;   //Which Quest Event, when changed, will change dialogue for this NPC. This is the quest event's order number
    private bool dialogueTreeSwitched = false;  //Has the dialogue tree been switched already? Used to prevent infinite tree changing.
    [ConditionalField(nameof(changeDialogueOnQuestUpdate))] public QuestEventState questEventState;
    public enum QuestEventState {CURRENT, COMPLETED, FAILED };

    Npcscript npcscript;
    GameObject player;
    gamemanager gamemanager;
    QuestManager questManager;
    DialogueManager dialogueManager;
    bool playerInRange;
    public GameObject shownQuestNameText;

    

    //QuestGiver script is not responsible for dictating the type of quest this is, only the questObject that is referenced in this Quest.

    private void Start()
    {
        gamemanager = gamemanager.instance;
        player = gamemanager.Player;
        questManager = gamemanager.questManager;
        dialogueManager = gamemanager.dialogueManager;
        shownQuestNameText.GetComponent<TextMeshPro>().text = myQuest.questName;
        npcscript = GetComponent<Npcscript>();
    }

    private void Update()
    {
       
    }

    void LateUpdate()
    {
        if (playerInRange && Input.GetButtonDown("Interact") && acceptQuestByProximity && !acceptQuestByName)
        {
            if (myQuest == null)
            {
                Debug.LogWarning("Quest Giver Script on " + gameObject + " has no associated quest!");
            }
            else
            {
                StartCoroutine(AcceptQuest(myQuest));
            }
        }
        else if ((playerInRange && Input.GetButtonDown("Interact") && acceptQuestByProximity && acceptQuestByName))
        {
            if (questName == null)
            {
                Debug.LogWarning("Quest Giver Script on " + gameObject + " has no associated quest Name!");
            }
            else
            {
                StartCoroutine(AcceptQuest(FindQuestByID(questName)));
            }
        }

        if (!dialogueManager.playerInConversation) { CheckForDialogueTreeSwitched(); }
    }

    public IEnumerator AcceptQuest(Quest quest)
    {
        //If the quest you're trying to add is not the same as the quest that is already active, or if there are no quests
        //if(questManager.GetComponent<QuestManager>().currentQuest.name == myQuest.name)
        if (CheckforSameQuest(quest))
        {
            Debug.LogWarning("Quest already accepted!");
        }
        else if (questManager.canAcceptQuest)
        {
            //player.GetComponent<Charquests>().currentQuests.Add(quest);
            questManager.GetComponent<QuestManager>().SetupNewQuest(quest);
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public Quest FindQuestByID(string questName)
    {
        string questNameLookingFor = questName.ToLower();
        Quest[] allQuests = Resources.LoadAll("Quests", typeof(Quest)).Cast<Quest>().ToArray();

        for (int i = 0; i < allQuests.Length; i++)
        {
            if (allQuests[i].questName.ToLower() == questNameLookingFor)
            {
                return allQuests[i];
            }
        }
        return null;
    }

    bool CheckforSameQuest(Quest myQuest)
    {
        if (player.GetComponent<Charquests>().currentQuests.Count <= 0)
        {
            return false;
        }
        else
        {
            foreach (Quest quest in player.GetComponent<Charquests>().currentQuests)
            {
                if (quest.questName == myQuest.questName)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void CheckSpecificQuestEvent()
    {
        QuestEvent qeToCheck = myQuest.questEvents.Where(qeToCheck => qeToCheck.order == whichQuestEventOrderToChangeOn).FirstOrDefault();

        switch (questEventState)
        {
            case QuestEventState.CURRENT:
                if (qeToCheck.status == QuestEvent.EventStatus.CURRENT)
                {
                    npcscript.myDialogue.defaultTreeId = dialogueTreeToSwitchTo;
                    dialogueTreeSwitched = true;
                }
                break;
            case QuestEventState.COMPLETED:
                if (qeToCheck.status == QuestEvent.EventStatus.DONE)
                {
                    npcscript.myDialogue.defaultTreeId = dialogueTreeToSwitchTo;
                    dialogueTreeSwitched = true;
                }
                break;
            case QuestEventState.FAILED:
                if (qeToCheck.status == QuestEvent.EventStatus.FAILED)
                {
                    npcscript.myDialogue.defaultTreeId = dialogueTreeToSwitchTo;
                    dialogueTreeSwitched = true;
                }
                break;
            default:
                break;
        }
    }

    //TODO: Let this allow quest events to change dialogue trees on update
    public void CheckForDialogueTreeSwitched()  //If the dialogue tree has been reset to it's first, then give the dialogue another chance to change its dialogue based on a quest event
    {
        if (changeDialogueOnQuestUpdate)
        {
            var firstDialogueTree = npcscript.myDialogue.dialogueTrees.First(); //Find the first Dialogue Tree in this character's dialogue
            if (npcscript.myDialogue.defaultTreeId == firstDialogueTree.treeID) //Allows this value to be reset if the dialogue is reset to its first tree
            {
                dialogueTreeSwitched = false;
            }
            if (!dialogueTreeSwitched) { CheckSpecificQuestEvent(); }
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

    public void GenerateQuestEventID()
    {
        foreach (QuestEvent questevent in myQuest.questEvents)
        {
            questevent.id = Guid.NewGuid().ToString();
        }
    }
}

