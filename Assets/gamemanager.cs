using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

public class GameManager : MonoBehaviour
{
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

    public InventoryUI inventoryui;
    public InventoryUIHelper inventoryUIHelper;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    #region Singleton and Awake
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Game Manager found!");
            Destroy(gameObject);
            return;
        }
        instance = this;

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
        allModules.Add(inventoryui);
        allModules.Add(inventoryUIHelper);
    }

    [ButtonMethod]
    public void AssignAllReferences()
    {

        Player = GameObject.FindWithTag("Player");
        PlayerAnim = Player.GetComponent<Animator>();

        playerRespawnPoint = GameObject.FindWithTag("player_respawn");
        if (!overwriteTime) Time.timeScale = 1f;

        //hurtScreenAnimator = hurtScreen.GetComponent<Animator>();
        //hurtScreen.SetActive(false);


        dialogueManager = GetComponentInChildren<DialogueManager>();
        questManager = GetComponentInChildren<QuestManager>();
        ihuiquestmanager = GetComponentInChildren<IHUIQuestManager>();
        infoHub = GetComponentInChildren<InfoHubManager>();
        pause_Menu_Manager = GetComponentInChildren<Pause_menu_manager>();
        Particle_Manager = GetComponentInChildren<ParticleManager>();
        Global_Script = GameObject.Find("Global Object").GetComponent<global_script>();
        inventoryui = GetComponentInChildren<InventoryUI>();
        inventoryUIHelper = GetComponentInChildren<InventoryUIHelper>();
        teleporternetwork = GetComponentInChildren<teleporternetwork>();
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
    }

    private void ChangedActiveScene(Scene currentScene, Scene nextScene)
    {
        Debug.Log($"Scene changed to {SceneManager.GetActiveScene().name}");
        

        switch (SceneManager.GetActiveScene().name)
        {
            case "Game Test Scene":
                OnSceneChangedEnableThese(
                    dialogueManager.gameObject,
                    questManager.gameObject,
                    ihuiquestmanager.gameObject,
                    infoHub.gameObject,
                    pause_Menu_Manager.gameObject,
                    Particle_Manager.gameObject,
                    Global_Script.gameObject,
                    teleporternetwork.gameObject,
                    inventoryui.gameObject,
                    inventoryUIHelper.gameObject
                                             ) ;
                AssignAllReferences();
                break;

            case "Main Menu Scene":
                OnSceneChangedDisableThese(
                    dialogueManager.gameObject,
                    questManager.gameObject,
                    ihuiquestmanager.gameObject,
                    infoHub.gameObject,
                    pause_Menu_Manager.gameObject,
                    Particle_Manager.gameObject,
                    Global_Script.gameObject,
                    teleporternetwork.gameObject,
                    inventoryui.gameObject,
                    inventoryUIHelper.gameObject
                                            );
                AssignAllReferences();
                break;

            default:
                Debug.LogWarning($"Current scene name is not part of GameManager switch statement. Please add it. All modules disabled.");
                break;
        }

    }

    public void OnSceneChangedDisableThese(params GameObject[] list) //Sets all modules that have been passed in as disabled. Should be used on scene changes in ChangedActiveScene()
    {
        foreach (var item in list)
        {
            item.SetActive(false);
            Debug.Log($"Scene was changed, {item.name} was disabled");
        }
    }

    public void OnSceneChangedEnableThese(params GameObject[] list) //Sets all modules that have been passed in as enabled. Should be used on scene changes in ChangedActiveScene()
    {
        foreach (var item in list)
        {
            item.SetActive(true);
            Debug.Log($"Scene was changed, {item.name} was enabled");
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
        Player.SetActive(true);
        PlayerAnim.enabled = true;
        pause_Menu_Manager.isGamePaused = false;
        Time.timeScale = 1;
        pause = false;
        paused = false;
        resume = true;
        resumed = true;
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
