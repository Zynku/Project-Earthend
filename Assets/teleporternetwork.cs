using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Search;
using static UnityEngine.UI.Image;

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

    public bool isMenuActive;

    VisualElement tpMenu;
    List<VisualElement> tpLines;
    public List<TeleporterUIScript> tpUIScripts;


    Animator animatorFrom;
    Animator animatorTo;

    AudioSource audiosourceFrom;
    AudioSource audiosourceTo;

    //public int pageNumber = 0;
    //public int startTeleporterNumber;

    private void OnValidate()   //Runs at the start of script loading in edit mode
    {
        EditorApplication.playModeStateChanged += DisableUIInEditMode;
    }

    public void DisableUIInEditMode(PlayModeStateChange state)
    {
        try
        {
            //UIDocument UIDoc = GetComponent<UIDocument>();
            //UIDoc.enabled = false;
            //Debug.Log("Disabling teleporter UI in edit mode");
        }
        catch (MissingReferenceException){}
    }

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

        UIDocument UIDoc = GetComponent<UIDocument>();
        UIDoc.enabled = true;
        tpMenu = GetComponent<UIDocument>().rootVisualElement;
        tpMenu.style.display = DisplayStyle.None;
        tpLines = tpMenu.Query<VisualElement>("tp-info").ToList();                  //Gets all 32 visualelements created

        for (int i = 0; i < teleporters.Length-1; i++)  //Loop through all teleporters...
        {
            tpLines[i].Q<Button>("tp-selector-button").text = teleporters[i].name;
            TeleporterUIScript newTpUIScript = new(teleporters[i], tpLines[i], this);
            tpUIScripts.Add(newTpUIScript);
            Debug.Log("Creating a TP UI Object for " + teleporters[i].name);
        }
    }

    public void showNetworkUI()
    {
        //Shows teleport menu, makes sure the event system can select the menu objects, and pauses game.
        //EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(teleporterUiElements[0]);
        Charinputs.instance.DisableMovementOnly();
        tpMenu.style.display = DisplayStyle.Flex;
        isMenuActive = true;
    }

    public void hideNetworkUI()
    {
        Charinputs.instance.EnableMovementOnly();
        tpMenu.style.display = DisplayStyle.None;
        isMenuActive = false;
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

        //GameManager.instance.ResumeGame();
        Charinputs.instance.EnableAllInputs();
    }

    public void PlayTypingSounds()
    {
        activatedAt.GetComponent<teleporterscript>().PlayMenuTyping();
    }
}