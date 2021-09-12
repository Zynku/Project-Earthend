using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    public QuestManager qManager;
    public QuestEvent qEvent;
    public QuestEventScript qScript;

    [Header("Collect Item Quest Type Variables")]
    public int interactRadius;

    [Header("Kill Enemies Quest Type Variables")]
    public int amountRequired;
    public int amountHas;

    public QuestEvent.EventStatus status;
    public eventType type;

    public enum eventType { location, collectItem, killEnemies}

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
    }

    public void Setup(QuestManager qm, QuestEvent qe, QuestEventScript qs)
    {
        qManager = qm;
        qEvent = qe;
        qScript = qs;
    }
}
 