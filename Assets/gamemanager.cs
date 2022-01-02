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

    public static GameManager instance;

    public DialogueManager dialogueManager;
    public QuestManager questManager;
    public IHUIQuestManager ihuiquestmanager;
    public InfoHubManager infoHub;
    public Pause_menu_manager pause_Menu_Manager;
    public ParticleManager Particle_Manager;

    public InventoryUI inventoryui;
    public InventoryUIHelper inventoryUIHelper;

    #region Singleton and Awake
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Game Manager found!");
            return;
        }
        instance = this;

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

        inventoryui = GetComponentInChildren<InventoryUI>();
        inventoryUIHelper = GetComponentInChildren<InventoryUIHelper>();
    }
    #endregion

    private void Update()
    {
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
        Time.timeScale = hitStopTimeScale;
        yield return new WaitForSecondsRealtime(hitStopAmount);
        Time.timeScale = 1f;
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

    [ButtonMethod]
    public void AssignAllReferences()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Game Manager found at " + instance);
            return;
        }
        instance = this;

        Player = GameObject.FindWithTag("Player");
        PlayerAnim = Player.GetComponent<Animator>();

        playerRespawnPoint = GameObject.FindWithTag("player_respawn");
        if (!overwriteTime) Time.timeScale = 1f;

        hurtScreenAnimator = hurtScreen.GetComponent<Animator>();
        hurtScreen.SetActive(false);

        dialogueManager = GetComponentInChildren<DialogueManager>();
        questManager = GetComponentInChildren<QuestManager>();
        ihuiquestmanager = GetComponentInChildren<IHUIQuestManager>();
        infoHub = GetComponentInChildren<InfoHubManager>();
        pause_Menu_Manager = GetComponentInChildren<Pause_menu_manager>();

        inventoryui = GetComponentInChildren<InventoryUI>();
        inventoryUIHelper = GetComponentInChildren<InventoryUIHelper>();
    }
}
