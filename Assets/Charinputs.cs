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


    public void Update()
    {
        
    }
}
