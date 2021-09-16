using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuestObject : MonoBehaviour
{
    public eventType type;
    public collectItemMonitorType itemToMonitor;

    public bool eventCompleted = false;

    [HideInInspector] public QuestManager qManager;
    [HideInInspector] public QuestEvent qEvent;
    [HideInInspector] public QuestEventScript qScript;
    public bool hasTimeLimit = false;
    public float timerTime;
    public float timerTargetTime;
    [HideInInspector] public Quest myQuest;

    private GameObject player;
    private Charpickup_inventory inventory;

    [Header("Collect Physical Item Quest Type Variables")]
    public int interactRadius;

    [Header("Collect Item Monitor Quest Type Variables")]
    public int moneyRequired, moneyCounter;

    [Header("Collect Inventory Items Quest Type Variables")]
    public int itemAmountTarget;         //itemAmountTarget is set in Inspector. Is how much is required to complete quest event. 
    public int itemAmountRequired;       //itemAmountRequired is how much you need to collect based on how many you have now. Is itemcounter(item.amountHas) + itemAmountTarget
    public int itemCounter;              //How much you actually have. This is item.amountHas
    private bool amountsSet;
    public ItemScriptable itemToCollect;

    public QuestEvent.EventStatus status;
    
    public enum eventType { location, collectPhysicalItem, collectItemMonitor, killEnemies, collectInventoryItems}
    public enum collectItemMonitorType { money}

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Charpickup_inventory>();
        timerTime = timerTargetTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        if (qEvent.status != QuestEvent.EventStatus.CURRENT) return;

        if (type == eventType.location)
        {
            qEvent.UpdateQuestEvent(QuestEvent.EventStatus.DONE);
            qManager.UpdateQuestsOnCompletion(qEvent);
        }
    }

    private void Update()
    {
        if (hasTimeLimit)
        {
            createTimeLimitForEvent();
        }

        status = qEvent.status;
        if (status == QuestEvent.EventStatus.CURRENT)
        {
            switch (type)
            {
                case eventType.location:
                    break;
                case eventType.collectPhysicalItem:
                    if (itemToMonitor == collectItemMonitorType.money)
                    {
                        moneyCounter = player.GetComponent<Charpickup_inventory>().money;
                        if (moneyCounter >= moneyRequired && !eventCompleted)
                        {
                            eventCompleted = true;
                            qEvent.UpdateQuestEvent(QuestEvent.EventStatus.DONE);
                            qManager.UpdateQuestsOnCompletion(qEvent);
                        }
                    }
                    break;
                case eventType.collectItemMonitor:
                    break;
                case eventType.killEnemies:
                    break;
                case eventType.collectInventoryItems:
                    CollectInventoryItems();
                    break;
                default:
                    Debug.Log("Assign a quest object type in the Inspector please");
                    break;
            }
        }
    }

    void CollectInventoryItems()
    {
        //Check if you have the item. If you don't recheck if you have the item every 0.5 seconds.
        //If you have the item, reference the amountHas value. Set itemCounter equal to it
        //Compare it to itemAmountRequired for completion
        if (!inventory.items.Contains(itemToCollect)) //If the specified item is not in the Inventory
        {
            StartCoroutine(CheckforInventoryItem());
        }
        else
        {
            itemCounter = itemToCollect.amountHas;
            if (!amountsSet)
            {
                itemAmountRequired = itemCounter + itemAmountTarget;
                amountsSet = true;
            }
            if (itemCounter > itemAmountRequired)
            {
                eventCompleted = true;
                qEvent.UpdateQuestEvent(QuestEvent.EventStatus.DONE);
                qManager.UpdateQuestsOnCompletion(qEvent);
            }
        }
    }

    IEnumerator CheckforInventoryItem()
    {
        if (!inventory.items.Contains(itemToCollect))
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(CheckforInventoryItem());
        }
        else
        {
            CollectInventoryItems();
        }
    }

    void createTimeLimitForEvent()
    {
        timerTime -= Time.deltaTime;
        if (timerTime <= 0)
        {
            timerTime = 0;
            qEvent.UpdateQuestEvent(QuestEvent.EventStatus.FAILED);
            qManager.UpdateQuestsOnCompletion(qEvent);
            timerTime = timerTargetTime;
        }
        //TODO: Create UI Element to show this, and for Quest as well
    }

    void createTimeLimitForQuest()
    {

    }

    public void Setup(QuestManager qm, QuestEvent qe, QuestEventScript qs)
    {
        qManager = qm;
        qEvent = qe;
        qScript = qs;
        eventCompleted = false;
        amountsSet = false;
    }
}
 