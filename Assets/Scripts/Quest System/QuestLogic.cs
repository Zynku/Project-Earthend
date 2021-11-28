using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;

[System.Serializable]
public class QuestLogic
{
    public EventType type;
    public QuestEvent.EventStatus status;
    public bool eventCompleted = false;

    [System.NonSerialized] public QuestManager qManager;
    [System.NonSerialized] public QuestEvent qEvent;
    [System.NonSerialized] public QuestEventPrefabScript qScript;
    [System.NonSerialized] public Quest myQuest;

    public bool hasEventTimeLimit = false;

    [Separator("Dynamic Variables")]
    [Tooltip("The gameObject that the player must collide with the complete this step")]
    //[ConditionalField(nameof(type), false, eventType.location)] public GameObject associatedGameObject;
    [ConditionalField(nameof(type), false, EventType.location)] public Vector3 areaStart;
    [ConditionalField(nameof(type), false, EventType.location)] public Vector3 areaEnd;

    [ConditionalField(nameof(type), false, EventType.collectUniqueItem)] public int interactRadius;

    [Tooltip("What player aspect should be monitored")]
    [ConditionalField(nameof(type), false, EventType.playerAspectMonitor)] public PlayerAspectMonitorType playerAspectToMonitor;


    [Tooltip("How much MORE the player is intended to collect to reach the target. Is assigned in Inspector.")]
    [ConditionalField(nameof(playerAspectToMonitor), false, PlayerAspectMonitorType.money)] public int moneyTarget;

    [Tooltip("How much player actually has.")]
    [ConditionalField(nameof(playerAspectToMonitor), false, PlayerAspectMonitorType.money)] public int moneyCounter;

    [Tooltip("How much player needs to collect based on how much they currently have. Is equal to the money counter + the money target.")]
    [HideInInspector] public int moneyRequired;


    [Tooltip("What level the player should be at or above to finish this quest. Is assigned in Inspector.")]
    [ConditionalField(nameof(playerAspectToMonitor), false, PlayerAspectMonitorType.level)] public int levelTarget;

    [Tooltip("What level the player is upon accepting this quest.")]
    [ConditionalField(nameof(playerAspectToMonitor), false, PlayerAspectMonitorType.level)] public int levelCounter;


    [Tooltip("What amount of health the player should be at or above to finish this quest. Is assigned in Inspector.")]
    [ConditionalField(nameof(playerAspectToMonitor), false, PlayerAspectMonitorType.health)] public int healthTarget;

    [Tooltip("What amount of health the player is upon accepting this quest.")]
    [ConditionalField(nameof(playerAspectToMonitor), false, PlayerAspectMonitorType.health)] public int healthCounter;

    [Tooltip("How much is required to complete quest event.")]
    [ConditionalField(nameof(type), false, EventType.collectInventoryItems)] public int itemAmountTarget;         //itemAmountTarget is set in Inspector. Is how much is required to complete quest event. 
    
    [Tooltip("How much player needs to collect based on how much they currently have. Is equal to the item counter + the amount target.")]
    [HideInInspector] public int itemAmountRequired;       //itemAmountRequired is how much you need to collect based on how many you have now. Is itemcounter(item.amountHas) + itemAmountTarget
    
    [Tooltip("How much player actually has.")]
    [ConditionalField(nameof(type), false, EventType.collectInventoryItems)] public int itemCounter;              //How much you actually have. This is item.amountHas
    [HideInInspector] public bool amountsSet;
    
    [ConditionalField(nameof(type), false, EventType.collectInventoryItems)] public ItemScriptable itemToCollect;

    [Separator("Timer Variables")]
    [ConditionalField(nameof(hasEventTimeLimit))] public float timerTargetTime;       //Time that timer starts at before counting down. Should be assigned in Inspector


    public enum EventType {
        none, 
        location, 
        collectUniqueItem, 
        playerAspectMonitor, 
        killEnemies, 
        collectInventoryItems
    }

    public enum PlayerAspectMonitorType {
        none, 
        money, 
        health, 
        level
    }

    public void Setup(QuestManager qm, QuestEvent qe, QuestEventPrefabScript qs, Quest qq)
    {
        qManager = qm;
        qEvent = qe;
        qScript = qs;
        eventCompleted = false;
        amountsSet = false;
        myQuest = qq;
    }
}
 