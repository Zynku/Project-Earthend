using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Cinemachine;
using MyBox;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float aliveTime;                    //Keeps track of how long this script (and by extension its children) have been alive

    [Header("Time")]
    public bool overwriteTime;
    [Range(0.001f, 1f)]
    public float timeScale = 1;

    [Header("Frame Rate")]
    [Range(1f, 60f)]
    public int frameRate = -1;

    [Separator("Scene Variables")]
    public string currentSceneName;
    public bool currentScenePausable;

    public float hitStopAmount = 0.2f;
    public float hitStopTimeScale = 0.5f;

    public GameObject hurtScreen;
    private Animator hurtScreenAnimator;

    public GameObject Player;
    public GameObject playerRespawnPoint;
    public GameObject[] Enemies;

    public bool mainCharRespawned = false;

    public static bool pause, resume;
    public bool paused, resumed;
    public bool hitStopped = false;

    [Separator("Managers")]
    public DialogueManager dialogueManager;             
    public QuestManager questManager;
    public IHUIQuestManager ihuiquestmanager;
    public InfoHubManager infoHub;
    public Pause_and_Scene_manager pause_and_scene_manager;
    public ParticleManager Particle_Manager;
    public TimerManager timerManager;
    public Player_Manager playerManager;
    public BGAudioScript BGAudioManager;    
    public InventoryUIHelper inventoryUIHelper;
    public Respawn_menu_manager respawn_Menu_Manager;
    public Welcome_screen_manager welcome_screen_manager;
    public InGameUi inGameUI;

    [Separator("Important Game Objects & Scripts")]
    public InGameUi ingame_UI;
    public InventoryUI inventoryui;

    [Separator("Cameras")]
    public CinemachineBrain cMBrain;
    public CinemachineVirtualCamera mainCamera;
    [ReadOnly] public CinemachineVirtualCameraBase mainCameraBase;
    CinemachineBasicMultiChannelPerlin camPerlin;

    [Separator("Event System")]
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
                Debug.LogWarning("Destroying Game Manager found in this scene");
                Destroy(instance);
                instance = this;
            }
            else if (this.aliveTime < instance.aliveTime) //If this alive time is less than the alive time of the current instance, then the instance is older
            {
                Debug.LogWarning("Destroying this version of Game Manager");
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
        AssignAllReferences();
        DontDestroyOnLoad(this);
        SceneReady();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }
    #endregion

    public void Start()
    {

    }

    public void AssignAllReferences()   
    {
        infoHub.gameObject.SetActive(true);         //Infohub is special since when it is active, it blocks the scene window in edit mode. It is set inactive in edit mode and activated here.
        infoHub.ActivatePages();
        playerRespawnPoint = GameObject.FindWithTag("player_respawn");
        if (Player == null) { Player = playerManager.SpawnPlayer(); }
    }
    private void Update()
    {
        //Debug.Log($"Current Scene number is {SceneManager.GetActiveScene().buildIndex} and Scene name is {SceneManager.GetActiveScene().name}");

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
        else
        {
            Time.timeScale = 1;
        }
        Application.targetFrameRate = frameRate;

        //Charhealth.Hit += HurtFlash;
        aliveTime += Time.deltaTime;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void ChangedActiveScene(Scene currentScene, Scene nextScene)    //Is called every time the scene is changed
    {
        Debug.Log($"-----------------------------------Scene changed to {currentSceneName}------------------------------------");
        DisableStuff();
    }

    public void DisableStuff()
    {
        respawn_Menu_Manager.DisableScreen();
        pause_and_scene_manager.HidePauseUI();
        //pause_Menu_Manager.HideLoadingScreen();
    }

    public void SceneReady()
    {
        BGAudioManager.ResetMasterVolToInitial();
        BGAudioManager.RemoveMixerLowPass(BGAudioManager.masterMixer.audioMixer);
        switch (SceneManager.GetActiveScene().name)
        {
            case "Game Test Scene":
                currentScenePausable = true;
                BGAudioManager.PlayAudioClip(0);
                welcome_screen_manager.ShowWelcomeScreen();
                inGameUI.mainCanvas.SetActive(true);
                playerManager.EnablePlayer();
                SetupCameras();
                break;

            case "Main Menu Scene":
                currentScenePausable = false;
                BGAudioManager.PlayAudioClip(1);
                inGameUI.mainCanvas.SetActive(false);
                playerManager.DisablePlayer();
                break;

            default:
                break;
        }
    }
    public void SetupCameras()
    {
        cMBrain = FindObjectOfType<CinemachineBrain>();
        mainCamera = FindObjectOfType<CinemachineVirtualCamera>(); //mainCamera.GetComponent<CinemachineVirtualCameraBase>();
        //mainCameraBase = cMBrain.GetCinemachineComponent<CinemachineVirtualCameraBase>();
        camPerlin = mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        mainCamera.Follow = Player.transform;
    }

    public void PauseGame()
    {
        Debug.Log("Pausing game...");
        playerManager.DisablePlayer();
        BGAudioManager.FadeMixerLowPass(BGAudioManager.masterMixer.audioMixer, "MasterLowPassCutoffFreq", 0.5f);
        pause_and_scene_manager.isGamePaused = true;
        Time.timeScale = 0;
        pause = true;
        paused = true;
        resume = false;
        resumed = false;
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        playerManager.ResetPlayer();
        BGAudioManager.RemoveMixerLowPass(BGAudioManager.masterMixer.audioMixer);
        pause_and_scene_manager.isGamePaused = false;
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
        pause_and_scene_manager.PauseMenuUi.SetActive(false);
        //Time.timeScale = 1;
        pause_and_scene_manager.isGamePaused = false;
        GameManager.instance.ResumeGame();

    }

    public void PauseWithMenu()
    {
        Debug.Log("Pausing game...");
        //PlayerAnim.enabled = false;
        pause_and_scene_manager.PauseMenuUi.SetActive(true);
        //Time.timeScale = 0;
        pause_and_scene_manager.isGamePaused = true;
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

    public IEnumerator DoScreenShake(float intensity, float time)
    {
        camPerlin.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(time);
        camPerlin.m_AmplitudeGain = 0;
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
