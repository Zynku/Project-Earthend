using MyBox;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    [Tooltip("Will the Encounter restart if the player completely exits and enters again?")]
    public bool resetOnPlayerExit;
    public bool encounterReady;         //Keeps certain functions from activating early. Is changed from Encounter Manager
    [Tooltip("How many times has this encounter been completed?")]
    public int timesCompleted;

    [Foldout("Important Objects", true)]
    public GameObject cameraTarget;
    public SpriteRenderer XMarker;
    public GameObject[] encounterEdges;
    public CinemachineVirtualCamera myCamera;

    [Foldout("Enemies", true)]
    public List<Enemymain> allEnemies;
    public List<Enemymain> defeatedEnemies;
    public int enemyAmount;
    public int enemiesDefeated;
    public float enemySpawnDelay;

    [Foldout("Encounter Limits", true)]
    [Tooltip("If the player collides here, the encounter starts")]
    public Collider2D encounterStartLimits;
    [Tooltip("If the player stops colliding with this, if they manage it, they leave the encounter")]
    public Collider2D encounterExitLimits;

    public enum EncounterStates
    {
        Inactive,   //Won't activate even if the player is ready and inside
        Active,     //Will be ready when the player steps inside and will be assigned to the manager
        Waiting,
        PlayerInside,   
        Completed,
        WaitingForPlayerExit    //Waiting for the player to exit so it can be reset
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.Player;
        encounterState = EncounterStates.Active;
        gameManager = GameManager.instance;
        encounterManager = GetComponentInParent<EncounterManager>();
        cameraTarget.GetComponent<SpriteRenderer>().enabled = false;
        XMarker.enabled = false;

        foreach (var edge in encounterEdges)
        {
            edge.GetComponent<SpriteRenderer>().enabled = false;
            edge.GetComponent<Collider2D>().enabled = false;
        }

        allEnemies = GetComponentsInChildren<Enemymain>().ToList();
        enemyAmount = allEnemies.Count;
        encounterManager.combatEncounters.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInStartCollider && playerInExitCollider && encounterState != EncounterStates.PlayerInside)    //Sets the state to player inside once the player...is inside...
        {
            if (encounterState != EncounterStates.WaitingForPlayerExit) //...and we're not waiting for the player to exit
            {
                if (encounterState != EncounterStates.Completed)
                {
                    encounterState = EncounterStates.PlayerInside;
                }
            }
        }
        else if (!playerInStartCollider && !playerInExitCollider && encounterState != EncounterStates.Active)   //Sets the encounter to active once the player
        {
            if (encounterState != EncounterStates.Completed)
            {
                encounterState = EncounterStates.Active;
            }

        }

        if (!playerInStartCollider && !playerInExitCollider)   //Player completed exited the encounter
        {
            if (resetOnPlayerExit)  //Resets variables here. The combat encounter manager will treat this as a new encounter once the player leaves it
            {
                encounterState = EncounterStates.Active;
                enemiesDefeated = 0;
            }
        }

        if (encounterState == EncounterStates.Completed && resetOnPlayerExit)
        {
            encounterState = EncounterStates.WaitingForPlayerExit;
        }
    }

    public void ChildEnemyDefeated(Enemymain enemy)
    {
        Debug.Log($"Adding {enemy.name} to the list");
        defeatedEnemies.Add(enemy);
        enemy.activated = false;
        enemiesDefeated++;
    }

    public void ResetEncounter()
    {
        Debug.Log("Enemies cleared from list");
        defeatedEnemies.Clear();    //This is essentially useless as the enemies are readded on the very next frame since the ecounter isnt set as inactive yet.
        enemiesDefeated = 0;
    }

    public void ActivateEdges()
    {
        foreach (var edge in encounterEdges)
        {
            edge.GetComponent<Collider2D>().enabled = true;
        }
    }

    public void DeactivateEdges()
    {
        foreach (var edge in encounterEdges)
        {
            edge.GetComponent<Collider2D>().enabled = false;
        }
    }

    public IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < allEnemies.Count; i++)
        {
            allEnemies[i].GetComponent<Enemymain>().spawnOrActivate?.Invoke();
            if (i == 1)
            {
                Debug.Log($"{i} enemies spawned, setting the ecounter to ready!");
                encounterReady = true;  //Sets the ecounter to be ready once the first enemy is spawned
            }
            yield return new WaitForSeconds(enemySpawnDelay);
        }
    }
}
