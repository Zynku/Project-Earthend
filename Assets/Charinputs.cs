using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;


public class Charinputs : MonoBehaviour
{
    Charcontrol charcontrol;
    Gamemanager gamemanager;
    InfoHubManager infohub;

    private void Start()
    {
        gamemanager = Gamemanager.instance;
        charcontrol = GetComponent<Charcontrol>();
        infohub = Gamemanager.instance.infoHub;
    }


    private void Update()
    {
        if (Input.GetButtonDown("Info Hub"))
        {
            bool infoHubenabled = infohub.gameObject.activeSelf;
            infohub.gameObject.SetActive(!infoHubenabled);
            if (!infoHubenabled) { infohub.firstPageShown = false; }
            gamemanager.TogglePauseGame();
        }
    }
}
