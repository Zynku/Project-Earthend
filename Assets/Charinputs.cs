using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;


public class Charinputs : MonoBehaviour
{
    Charcontrol charcontrol;
    GameManager gamemanager;
    InfoHubManager infohub;

    private void Start()
    {
        gamemanager = GameManager.instance;
        charcontrol = GetComponent<Charcontrol>();
        infohub = GameManager.instance.infoHub;
    }


    public void Update()
    {
        //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA FUCK
    }

    // P Pauses the game. Called from Gamemanager Update()
    // O Unpauses the game. Called from Gamemanager Update()
    // J Resets health. Called from Charhealth Update()
    // H Adds 20 health. Called from Charhealth Update()
    // K Takes max damage from health. Called from Charhealth Update()
    // K Also resets enemy health values to max. Called from EnemyHealth Update()
    // L Takes 20 damage from health. Called from Charhealth Update()
    // esc Pauses and unpauses the game. Called from Pause_and_Scene_manager Update()
    // Tab opens and closes the Info Hub. Called from Gamemanager Update()
}
