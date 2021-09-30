using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class teleporternetwork : MonoBehaviour
{
    private GameObject Player;
    private GameObject[] teleporters;
    private List<GameObject> teleporterUiElements = new List<GameObject>();
    public GameObject activatedAt;
    public GameObject uiElements;
    public GameObject exitButton;
    public GameObject teleporterUIPrefab;
    public GameObject tpMenuBG;


    Animator animatorFrom;
    Animator animatorTo;

    AudioSource audiosourceFrom;
    AudioSource audiosourceTo;

    //public int pageNumber = 0;
    //public int startTeleporterNumber;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player");                      //Finds player automatically
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");  //Finds all teleporters in scene and assigns them to the array
    }

    private void Start()
    {
        foreach (GameObject teleporter in teleporters)
        {
            GameObject tp = Instantiate(teleporterUIPrefab, tpMenuBG.transform);
            tp.GetComponent<TeleporterUIScript>().myTeleporter = teleporter;
            tp.GetComponent<TeleporterUIScript>().teleporterNameText.text = teleporter.name;
            teleporterUiElements.Add(tp);
        }
        uiElements.SetActive(false);
    }

    public void showNetworkUI()
    {
        //Shows teleport menu, makes sure the event system can select the menu objects, and pauses game.
        uiElements.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(teleporterUiElements[0]);
        gamemanager.instance.PauseGame();
    }

    public void hideNetworkUI()
    {
        uiElements.SetActive(false);
        gamemanager.instance.ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        //pageNumberText.text = pageNumber.ToString();
        
    }

    public void Teleport(GameObject teleporter)
    {
        //Grabs player transform and sets it to the teleporter transform with some offset so player does not clip into ground.
        //Grabs both teleporters' animators, audiosources, and audioclips and activates them all
        //Deactivates the teleporter menu, resumes game.
        Debug.Log("Teleporting to " + teleporter.name);

        Vector3 teleportoffset = new Vector3(0, 0.5f, 0);
        Player.transform.position = teleporter.transform.position + teleportoffset;
        Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        teleporter.GetComponent<teleporterscript>().PlaySoundandAnimation();
        activatedAt.GetComponent<teleporterscript>().PlaySoundandAnimation();



        foreach (GameObject tp in teleporters)
        {
            tp.GetComponent<teleporterscript>().ResetTPTimer();
        }
        
        //animator = activatedAt.GetComponent<Animator>();
        //animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

        //audiosource = activatedAt.GetComponent<AudioSource>();
        //audiosourceTo = teleporters[3 * (pageNumber - 1)].GetComponent<AudioSource>();

        //animator.SetTrigger("Teleport");
        //animatorTo.SetTrigger("Teleport");

        //audiosource.volume = GetComponent<teleporterscript>().teleportingVolume;
        //audiosource.PlayOneShot(GetComponent<teleporterscript>().teleporting);
        //audiosourceTo.volume = GetComponent<teleporterscript>().teleportingVolume;
        //audiosourceTo.PlayOneShot(GetComponent<teleporterscript>().teleporting);

        uiElements.SetActive(false);

        gamemanager.instance.ResumeGame();
    }

    public void AssignTeleporters(int tp)
    {
/*        //This functions assigns the first, second, and third teleporter names in the teleporter array to the teleporter menu.
        int q = 0;
        for (int i = tp; i < tp + 3; i++, q++)  //Loops through the first 3 names and assigns to the 3 name slots
        {
            if (i < teleporters.Length)
            {
                teleporterNames[q].text = teleporters[i].name.ToString();   //If there is a teleporter in the "i"th slot, it assigns the name
            }
            else
            {
                teleporterNames[q].text = "[No Teleporter Found]";          //If there are no teleporters in the "i" th slot, it says no teleporter found
            }
        }*/
    }
/*
    public void NextPage()
    {
        //Increments page number, stops page numbers from overflowing or underflowing, reassigns teleporters based on the number teleporter we should
        //be starting at. This is done using some algebra and the page number to calculate the teleporter.
        pageNumber++;
        if (pageNumber < 1) { pageNumber = 1; }
        if (pageNumber > (teleporters.Length/3 + 1)) { pageNumber = teleporters.Length / 3 + 1; }
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
    }

    public void PreviousPage()
    {
        //Does the same as above but decrements page number instead.
        pageNumber--;
        if (pageNumber < 1) { pageNumber = 1; }
        if (pageNumber > (teleporters.Length / 3 + 1)) { pageNumber = teleporters.Length / 3 + 1; }
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
    }

    public void TeleportToOne()
    {
        //Grabs player transform and sets it to the teleporter transform with some offset so player does not clip into ground.
        //Grabs both teleporters' animators, audiosources, and audioclips and activates them all
        //Deactivates the teleporter menu, resumes game.
        Debug.Log(teleporters[3 * (pageNumber - 1)]);
        if (teleporters[3 * (pageNumber - 1)] != null)
        {
            Vector3 teleportoffset = new Vector3 (0, 0.5f, 0);
            Player.transform.position = teleporters[3 * (pageNumber - 1)].transform.position + teleportoffset;
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            animator = activatedAt.GetComponent<Animator>();
            animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

            audiosource = activatedAt.GetComponent<AudioSource>();
            audiosourceTo = teleporters[3 * (pageNumber - 1)].GetComponent<AudioSource>();

            *//*animator.SetTrigger("Teleport");
            animatorTo.SetTrigger("Teleport");

            audiosource.volume = GetComponent<teleporterscript>().teleportingVolume;
            audiosource.PlayOneShot(GetComponent<teleporterscript>().teleporting);
            audiosourceTo.volume = GetComponent<teleporterscript>().teleportingVolume;
            audiosourceTo.PlayOneShot(GetComponent<teleporterscript>().teleporting);*//*

            TeleportCanvas.SetActive(false);

            gamemanager.instance.ResumeGame();
        }
    }

    public void TeleportToTwo()
    {
        if (teleporters[(3 * (pageNumber - 1) + 1)] != null)
        {
            Debug.Log(teleporters[(3 * (pageNumber - 1) + 1)]);

            Player.transform.position = teleporters[(3 * (pageNumber - 1) + 1)].transform.position + new Vector3(0, 0.5f, 0);
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            animator = activatedAt.GetComponent<Animator>();
            animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

            TeleportCanvas.SetActive(false);

            gamemanager.instance.ResumeGame();
        }
    }

    public void TeleportToThree()
    {
        if (teleporters[(3 * (pageNumber - 1) + 2)] != null)
        {
            Debug.Log(teleporters[(3 * (pageNumber - 1) + 2)]);

            Player.transform.position = teleporters[(3 * (pageNumber - 1) + 2)].transform.position + new Vector3(0, 0.5f, 0);
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            animator = activatedAt.GetComponent<Animator>();
            animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

            TeleportCanvas.SetActive(false);

            gamemanager.instance.ResumeGame();
        }
    }*/
}