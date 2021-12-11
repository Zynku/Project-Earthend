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
        
    }
}
