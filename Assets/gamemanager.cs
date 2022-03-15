using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using MyBox;

public class GameManager : MonoBehaviour
{
    public float aliveTime;                    //Keeps track of how long this script (and by extension its children) have been alive

    [Header("Time")]
    public bool overwriteTime;
    [Range(0.001f, 1f)]
    public float timeScale = 1;

    [Header("Frame Rate")]
    [Range(1f, 60f)]
    public int frameRate = -1;

    public float hitStopAmount = 0.2f;
    public float hitStopTimeScale = 0.5f;

    public GameObject hurtScreen;
    private Animator hurtScreenAnimator;

    public GameObject Player;
    private Animator PlayerAnim;
    public GameObject playerRespawnPoint;
    public GameObject[] Enemies;

    public bool mainCharRespawned = false;

    public static bool pause, resume;
    public bool paused, resumed;
    public bool hitStopped = false;

    public static GameManager instance;

    public List<MonoBehaviour> allModules;
    

    //Make sure when you declare a new reference below, add it to all Modules list in AddAllReferencesToModuleList(), make sure it is assigned to in AssignAllReferences()
    //and it is enabled and disabled in various scenes in ChangedActiveScene()-

    public DialogueManager dialogueManager;             
    public QuestManager questManager;
    public IHUIQuestManager ihuiquestmanager;
    public InfoHubManager infoHub;
    public Pause_menu_manager pause_Menu_Manager;
    public ParticleManager Particle_Manager;
    public global_script Global_Script;
    public teleporternetwork teleporternetwork;
    public TimerManager timerManager;

    public InventoryUI inventoryui;
    public InventoryUIHelper inventoryUIHelper;

    public InGameUi ingame_UI;
    public Respawn_menu_manager respawn_Menu_Manager;

    public EventSystem theEventSystem;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    #region Singleton and Awake
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Game Manager found!");

            if (this.aliveTime > instance.aliveTime)    //If this alive time is greater than the alive time of the current instance, then the instance is younger
            {
                Destroy(instance);
                instance = this;
            }
            else if (this.aliveTime < instance.aliveTime) //If this alive time is less than the alive time of the current instance, then the instance is older
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
        
        AssignAllReferences();
        AddAllReferencesToModuleList();
        DontDestroyOnLoad(this);
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }
    #endregion
    
    private void AddAllReferencesToModuleList()
    {
        allModules.Add(dialogueManager);
        allModules.Add(questManager);
        allModules.Add(ihuiquestmanager);
        allModules.Add(infoHub);
        allModules.Add(pause_Menu_Manager);
        allModules.Add(Particle_Manager);
        allModules.Add(Global_Script);
        allModules.Add(teleporternetwork);
        allModules.Add(timerManager);
        allModules.Add(inventoryui);
        allModules.Add(inventoryUIHelper);
        allModules.Add(ingame_UI);
        allModules.Add(respawn_Menu_Manager);
        allModules.Add(theEventSystem);
    }

    [ButtonMethod]
    public void AssignAllReferences()   
    {
        dialogueManager = GetComponentInChildren<DialogueManager>();
        questManager = GetComponentInChildren<QuestManager>();
        ihuiquestmanager = GetComponentInChildren<IHUIQuestManager>();
        infoHub = GetComponentInChildren<InfoHubManager>();
        pause_Menu_Manager = GetComponentInChildren<Pause_menu_manager>();
        Particle_Manager = GetComponentInChildren<ParticleManager>();
        Global_Script = GetComponentInChildren<global_script>();
        teleporternetwork = GetComponentInChildren<teleporternetwork>();
        timerManager = GetComponentInChildren<TimerManager>();
        inventoryui = GetComponentInChildren<InventoryUI>();
        inventoryUIHelper = GetComponentInChildren<InventoryUIHelper>();
        ingame_UI = GetComponentInChildren<InGameUi>();
        respawn_Menu_Manager = GetComponentInChildren<Respawn_menu_manager>();
        theEventSystem = GetComponentInChildren<EventSystem>();


        //hurtScreenAnimator = hurtScreen.GetComponent<Animator>();
        //hurtScreen.SetActive(false);
    }

    [ButtonMethod]
    public void AssignPlayerReferences()
    {
        try { Player = GameObject.FindWithTag("Player"); } catch { }
        if (Player != null) PlayerAnim = Player.GetComponent<Animator>();
        if (Player != null) playerRespawnPoint = GameObject.FindWithTag("player_respawn");
    }

    private void Update()
    {
        Debug.Log($"Current Scene number is {SceneManager.GetActiveScene().buildIndex} and Scene name is {SceneManager.GetActiveScene().name}");

        if (Input.GetKeyDown(KeyCode.P)) { PauseGame(); }
        if (Input.GetKeyDown(KeyCode.O)) { ResumeGame(); }

        if (Input.GetButtonDown("Info Hub"))
        {
            bool infoHubenabled = infoHub.gameObject.activeSelf;
            infoHub.gameObject.SetActive(!infoHubenabled);
            if (!infoHubenabled) { infoHub.firstPageShown = false; }
            TogglePauseGame();

            if (questManager.qBlankOnScreen)
            {
                ihuiquestmanager.ClearCurrentQuest();
                ihuiquestmanager.ShowNewQuest(questManager.lastAcceptedQuest);
            }
        }

        if (pause) { paused = true; }
        if (resume) { resumed = true; }


        if (overwriteTime) 
        {            
            Time.timeScale = timeScale; 
        }
        Application.targetFrameRate = frameRate;

        //Charhealth.Hit += HurtFlash;
        aliveTime += Time.deltaTime;
    }

    private void ChangedActiveScene(Scene currentScene, Scene nextScene)    //Is called every time the scene is changed
    {
        Debug.Log($"-----------------------------------Scene changed to {SceneManager.GetActiveScene().name}------------------------------------");

        switch (SceneManager.GetActiveScene().name)
        {
            case "Game Test Scene":
                OnSceneChangedEnableThese(
                    dialogueManager,
                    questManager,
                    ihuiquestmanager,
                    infoHub,
                    pause_Menu_Manager,
                    Particle_Manager,
                    Global_Script,
                    teleporternetwork,
                    timerManager,
                    inventoryui,
                    inventoryUIHelper,
                    ingame_UI,
                    respawn_Menu_Manager
                                             ) ;

                OnSceneChangedDisableThese();
                AssignPlayerReferences();
                break;

            case "Main Menu Scene":
                OnSceneChangedDisableThese(
                    dialogueManager,
                    questManager,
                    ihuiquestmanager,
                    infoHub,
                    pause_Menu_Manager,
                    Particle_Manager,
                    Global_Script,
                    teleporternetwork,
                    timerManager,
                    inventoryui,
                    inventoryUIHelper,
                    ingame_UI,
                    respawn_Menu_Manager,
                    theEventSystem                  //Main Menu Scene already has its own event system
                                            );

                OnSceneChangedEnableThese();
                break;

            default:
                Debug.LogWarning($"Current scene name is not part of GameManager switch statement. Please add it. All modules disabled.");
                break;
        }
        //AssignAllReferences();
        
    }

    public void OnSceneChangedDisableThese(params MonoBehaviour[] list) //Sets all modules that have been passed in as disabled. Should be used on scene changes in ChangedActiveScene()
    {
        //Debug.Log($"Scene was changed...");
        if (list.Length > 0)
        {
            foreach (var item in list)
            {
                item.enabled = false;
                Debug.Log($"{item.name} was disabled");

                if (item == list[list.Length - 1])
                {
                    Debug.Log($"----------------------------------All {list.Length} modules in list successfully updated!-------------------------------------------");
                }
            }
        }
    }

    public void OnSceneChangedEnableThese(params MonoBehaviour[] list) //Sets all modules that have been passed in as enabled. Should be used on scene changes in ChangedActiveScene()
    {
        //Debug.Log($"Scene was changed...");
        if (list.Length > 0)
        {
            foreach (var item in list)
            {
                item.enabled = true;
                Debug.Log($"{item.name} was enabled");

                if (item == list[list.Length - 1])
                {
                    Debug.Log($"----------------------------------All {list.Length} modules in list successfully updated!-------------------------------------------");
                }
            }
        }
    }

    [ButtonMethod]
    public void OnSceneChangedDisableAll()
    {
        foreach (var item in allModules)
        {
            item.gameObject.SetActive(false);
        }
    }

    [ButtonMethod]
    public void OnSceneChangedEnableAll()
    {
        foreach (var item in allModules)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void PauseGame()
    {
        Debug.Log("Pausing game...");
        Player.SetActive(false);
        PlayerAnim.enabled = false;
        pause_Menu_Manager.isGamePaused = true;
        Time.timeScale = 0;
        pause = true;
        paused = true;
        resume = false;
        resumed = false;
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        pause_Menu_Manager.isGamePaused = false;
        Time.timeScale = 1;
        pause = false;
        paused = false;
        resume = true;
        resumed = true;

        try
        {
            Player.SetActive(true);
            PlayerAnim.enabled = true;
        }
        catch (MissingReferenceException) { }
    }

    public void TogglePauseGame()
    {
        Debug.Log("Toggling Pause");
        if (paused == false)
        {
            PauseGame();
            paused = true;
        }
        else
        {
            ResumeGame();
            paused = false;
        }
    }

    public void ResumeFromMenu()
    {
        Debug.Log("Resuming game...");
        //PlayerAnim.enabled = true;
        pause_Menu_Manager.PauseMenuUi.SetActive(false);
        //Time.timeScale = 1;
        pause_Menu_Manager.isGamePaused = false;
        GameManager.instance.ResumeGame();

    }

    public void PauseWithMenu()
    {
        Debug.Log("Pausing game...");
        //PlayerAnim.enabled = false;
        pause_Menu_Manager.PauseMenuUi.SetActive(true);
        //Time.timeScale = 0;
        pause_Menu_Manager.isGamePaused = true;
        GameManager.instance.PauseGame();
    }

    public IEnumerator MeleeHitStop()
    {
        if (!hitStopped)
        {
            hitStopped = true;
            Time.timeScale = hitStopTimeScale;
            yield return new WaitForSecondsRealtime(hitStopAmount);
            Time.timeScale = 1f;
            hitStopped = false;
        }
    }

    void HurtFlash()
    {
        StartCoroutine("DoHurtFlash");
    }

    IEnumerator DoHurtFlash()
    {
        hurtScreen.SetActive(true);
        hurtScreenAnimator.SetTrigger("HurtFlash");
        float waittime = 0.2f;
        yield return new WaitForSeconds(waittime);
        hurtScreen.SetActive(false);
    }

    void FixedUpdate()
    {
        Enemies = GameObject.FindGameObjectsWithTag("enemy");
    }
  
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
