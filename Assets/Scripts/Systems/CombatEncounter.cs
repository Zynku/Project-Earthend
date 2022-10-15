using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEncounter : MonoBehaviour
{
    GameObject player;
    EncounterManager encounterManager;
    GameManager gameManager;

    [Foldout("Encounter States & Variables", true)]
    [Tooltip("Shows what state the encounter is in")]
    public EncounterStates encounterState;
    public bool playerInStartCollider;  //This and the below one are controlled from the encounterhitbox children
    public bool playerInExitCollider;


    [Foldout("Encounter Limits", true)]
    [Tooltip("If the player collides here, the encounter starts")]
    public Collider2D encounterStartLimits;
    [Tooltip("If the player stops colliding with this, if they manage it, they leave the encounter")]
    public Collider2D encounterExitLimits;

    public enum EncounterStates
    {
        Inactive,
        Active,
        Waiting,
        PlayerInside,
        Completed
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.Player;
        encounterState = EncounterStates.Active;
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInStartCollider && playerInExitCollider)
        {
            encounterState = EncounterStates.PlayerInside;
        }
        else
        {
            encounterState=EncounterStates.Active;
        }
    }
}
