using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    public eventType type;
    public collectItemMonitorType itemToMonitor;

    [HideInInspector] public QuestManager qManager;
    public QuestEvent qEvent;
    [HideInInspector] public QuestEventScript qScript;
    private GameObject player;

    [Header("Collect Physical Item Quest Type Variables")]
    public int interactRadius;

    [Header("Collect Item Monitor Quest Type Variables")]
    public int moneyCounter;
    public int amountRequired;
    public int amountHas;

    [Header("Kill Enemies Quest Type Variables")]
    

    public QuestEvent.EventStatus status;
    
    public enum eventType { location, collectPhysicalItem, collectItemMonitor, killEnemies}
    public enum collectItemMonitorType { money}

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        status = qEvent.status;

        //TODO: Turn this into a switch please
        if (type == eventType.collectItemMonitor)
        {
            if (itemToMonitor == collectItemMonitorType.money)
            {
                moneyCounter = player.GetComponent<Charpickup_inventory>().money;
                if (moneyCounter >= amountRequired)
                {
                    qEvent.UpdateQuestEvent(QuestEvent.EventStatus.DONE);
                    qManager.UpdateQuestsOnCompletion(qEvent);
                }
            }
        }
    }

    public void Setup(QuestManager qm, QuestEvent qe, QuestEventScript qs)
    {
        qManager = qm;
        qEvent = qe;
        qScript = qs;
    }
}
 