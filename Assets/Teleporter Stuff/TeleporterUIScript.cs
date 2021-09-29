using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeleporterUIScript : MonoBehaviour
{
    public TextMeshProUGUI teleporterNameText;
    public GameObject myTeleporter;
    teleporternetwork teleporternetwork;

    // Start is called before the first frame update
    void Start()
    {
        teleporternetwork = GetComponentInParent<teleporternetwork>();
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
