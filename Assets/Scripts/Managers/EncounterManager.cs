using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    GameManager gameManager;
    CameraManager cameraManager;

    public List<CombatEncounter> combatEncounters;
    public CombatEncounter currentActiveEncounter;

    private bool newEncounterSetup;

    public CombatEncounter.EncounterStates encounterStates;

    private void Start()
    {
        gameManager = GameManager.instance;
        cameraManager = gameManager.cameraManager;
    }

    private void Update()
    {
        if (!newEncounterSetup && currentActiveEncounter != null) { AssignNewEncounter(currentActiveEncounter);}
        else if (newEncounterSetup && currentActiveEncounter == null) { ClearCurrentEncounter();}

        if (newEncounterSetup) { ManageEncounter(); }
    }

    public void ManageEncounter()
    {
        encounterStates = currentActiveEncounter.encounterState;
        if (currentActiveEncounter.encounterState == CombatEncounter.EncounterStates.Active) { ClearCurrentEncounter(); }
    }

    public void AssignNewEncounter(CombatEncounter newEncounter)
    {
        currentActiveEncounter = newEncounter;
        //Change priorities
        encounterStates = newEncounter.encounterState;
        newEncounterSetup = true;
    }

    public void ClearCurrentEncounter()
    {
        currentActiveEncounter = null;
        cameraManager.SetupCameras();
        newEncounterSetup = false;
    }
}
