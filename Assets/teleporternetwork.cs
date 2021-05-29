using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class teleporternetwork : MonoBehaviour
{
    public GameObject[] teleporters;
    public TextMeshProUGUI[] teleporterNames;
    public TextMeshProUGUI pageNumberText;
    public GameObject TeleportCanvas;
    public int pageNumber = 1;
    public int startTeleporterNumber;
    //public TextMeshProUGUI TeleporterOne;
    //public TextMeshProUGUI TeleporterTwo;
    //public TextMeshProUGUI TeleporterThree;

    private void Start()
    { 
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");
        pageNumber = 1;
        AssignTeleporters(0);
    }

    // Update is called once per frame
    void Update()
    {
        pageNumberText.text = pageNumber.ToString();

        if (Input.GetKeyDown(KeyCode.P))
        {
            NextPage();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            PreviousPage();
        }
    }
    public void AssignTeleporters(int startteleporter)
    {
        for (int i = startteleporter; i < startteleporter + 3; i++)
        {
            int q = 0;
            if (i == teleporters.Length)
            {
                teleporterNames[q].text = "No teleporter found!";
                i = startteleporter;
                Debug.Log("No teleporter at " + teleporters[i]);
            }
            else
            {
                if (q < 3)
                {
                    teleporterNames[q].text = teleporters[i].name.ToString();
                    q++;
                    Debug.Log(teleporters.Length);
                    Debug.Log(i);
                }
                else
                {
                    q = 0;
                }
            }
        }
    }

    public void NextPage()
    {
        pageNumber++;
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
    }

    public void PreviousPage()
    {
        pageNumber--;
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
        //Decrement Page number
        //Calculate Start Teleporter Number
        //Call AssignTeleporters
    }


    //Page 1 start at teleporter 0 ;
    //Page 2 start at teleporter 3 ;
    //Page 3 start at teleporter 6 ;
    //Page 4 start at teleporter 9 ;
    //Page 5 start at teleporter 12 ;
    //Formula is tp# = 3 (pg# - 1)

}
