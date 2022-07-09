using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class teleporternetwork : MonoBehaviour
{
    private GameObject Player;
    public GameObject[] teleporters;
    private teleporternetwork teleportNetwork;
    private List<GameObject> teleporterUiElements = new List<GameObject>();
    public GameObject activatedAt;              //Is assigned to from a teleporter when you interact with it. The teleporter assigns itself here
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

    private void OnEnable()
    {
        foreach (var tp in teleporters)             //Make sure all child teleporters' scripts are awake
        {
            tp.GetComponent<teleporterscript>().enabled = true;
        }
    }

    private void Start()
    {
        Player = GameManager.instance.Player;                      //Finds player automatically
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");  //Finds all teleporters in scene and assigns them to the array
        teleportNetwork = GetComponentInChildren<teleporternetwork>();

        foreach (GameObject teleporter in teleporters)
        {
            teleporter.GetComponent<teleporterscript>().Network = teleportNetwork;
            GameObject tp = Instantiate(teleporterUIPrefab, tpMenuBG.transform);
            tp.GetComponent<TeleporterUIScript>().myTeleporter = teleporter;
            tp.GetComponent<TeleporterUIScript>().teleporterNameText.text = teleporter.name;
            teleporterUiElements.Add(tp);
            //Debug.Log("Creating a TP UI Object for " + teleporter.name);
        }
        uiElements.SetActive(false);
    }

    public void showNetworkUI()
    {
        //Shows teleport menu, makes sure the event system can select the menu objects, and pauses game.
        uiElements.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(teleporterUiElements[0]);
        GameManager.instance.PauseGame();
    }

    public void hideNetworkUI()
    {
        uiElements.SetActive(false);
        GameManager.instance.ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
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
        uiElements.SetActive(false);

        GameManager.instance.ResumeGame();
    }

    public void PlayTypingSounds()
    {
        activatedAt.GetComponent<teleporterscript>().PlayMenuTyping();
    }
}