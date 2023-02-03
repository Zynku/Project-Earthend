using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

[System.Serializable]
public class TeleporterUIScript
{
    public GameObject myTeleporter;
    public VisualElement uIElement;
    teleporternetwork teleporternetwork;

    public TeleporterUIScript(GameObject myTeleporter, VisualElement uIElement, teleporternetwork teleporternetwork)
    {
        this.myTeleporter = myTeleporter;
        this.uIElement = uIElement;
        this.teleporternetwork = teleporternetwork;
    }

    public void TeleportToMyTeleporter()
    {

        if (myTeleporter != null)
        {
            teleporternetwork.Teleport(myTeleporter);
        }
        else
        {
            Debug.Log("This teleporter UI Element has no teleporter attached to it!");
        }
    }
}
