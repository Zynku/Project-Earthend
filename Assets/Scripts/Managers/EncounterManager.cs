using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class EncounterManager : MonoBehaviour
{
    GameManager gameManager;
    CameraManager cameraManager;

    [Foldout("Encounters", true)]
    public List<CombatEncounter> combatEncounters;
    public CombatEncounter currentActiveEncounter;

    [Foldout("Variables", true)]
    [Tooltip("How much time should pass after the last enemy is defeated before the encounter is completed")]
    public float combatCompleteDelay;
    [HideInInspector]public bool newEncounterSetup;

    public CombatEncounter.EncounterStates encounterStates;

    private void Start()
    {
        gameManager = GameManager.instance;
        cameraManager = gameManager.cameraManager;
    }

    private void Update()
    {
        //if (!newEncounterSetup && currentActiveEncounter != null) { AssignNewEncounter(currentActiveEncounter);}
        //else if (newEncounterSetup && currentActiveEncounter == null) { ClearCurrentEncounter();}

/*        foreach (var encounter in combatEncounters) //If any encounter is ready to be used (player inside, enemies ready) then assign it
        {
            if (encounter.encounterState == CombatEncounter.EncounterStates.PlayerInside && encounter.encounterState != CombatEncounter.EncounterStates.Completed)
            {
                if (!newEncounterSetup) //To make sure a new encounter isnt repeatedly set. This is set true in the AssignNewEncounter function
                {
                    AssignNewEncounter(encounter);
                }
            }
        }*/

        if (currentActiveEncounter != null)
        {
            ManageEncounter();  //Manage the current active encounter if there is one
            if (currentActiveEncounter.enemiesDefeated >= currentActiveEncounter.enemyAmount)   //When we defeat enough enemies, complete the encounter
            {
                StartCoroutine(CompleteCurrentEcounter());
            }
        }
    }

    public void ManageEncounter()
    {
        encounterStates = currentActiveEncounter.encounterState;
        //if (currentActiveEncounter.encounterState == CombatEncounter.EncounterStates.Active) { ClearCurrentEncounter(); }
    }

    public void AssignNewEncounter(CombatEncounter newEncounter)
    {
        newEncounter.myCamera.Priority = 100;
        newEncounter.ActivateEdges();
        newEncounter.ResetEncounter();
        StartCoroutine(newEncounter.SpawnEnemies());    //Tell the encounter to start spawning its enemies
        encounterStates = newEncounter.encounterState;
        newEncounterSetup = true;
        currentActiveEncounter = newEncounter;
    }

    public IEnumerator CompleteCurrentEcounter()
    {
        CombatEncounter thisEncounter = currentActiveEncounter;
        currentActiveEncounter = null;

        yield return new WaitForSeconds(combatCompleteDelay);

        thisEncounter.myCamera.Priority = 5;
        thisEncounter.DeactivateEdges();
        thisEncounter.encounterState = CombatEncounter.EncounterStates.Completed;
        thisEncounter.timesCompleted++;
        cameraManager.SetupCameras();
        newEncounterSetup = false;
        thisEncounter.encounterReady = false;
    }

    public void ClearCurrentEncounter()
    {
        currentActiveEncounter.myCamera.Priority = 5;
        currentActiveEncounter.DeactivateEdges();
        cameraManager.SetupCameras();
        currentActiveEncounter = null;
        newEncounterSetup = false;
    }
}
