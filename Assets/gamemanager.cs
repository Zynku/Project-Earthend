using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Cinemachine;
using MyBox;
using UnityEngine.UIElements;
using System;
using UnityEngine.InputSystem;
using Cinemachine.PostFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    public Cinemachine.PostFX.CinemachineVolumeSettings volumeSettings;    //Global post processing volume settings
    private LiftGammaGain darkness;

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
    //public InfoHubManager infoHub;
    public Pause_and_Scene_manager pause_and_scene_manager;
    public ParticleManager Particle_Manager;
    public TimerManager timerManager;
    public Player_Manager playerManager;
    public BGAudioScript BGAudioManager;
    public InventoryManager inventoryManager;
    public InventoryUIHelper inventoryUIHelper;
    public Information_hub_ui_manager ihuiManager;
    public Respawn_menu_manager respawn_Menu_Manager;
    public Welcome_screen_manager welcome_screen_manager;
    public EncounterManager encounterManager;
    public CameraManager cameraManager;
    //public Enemymanager enemyManager;
    public InGameUi inGameUI;

    [Separator("Important Game Objects & Scripts")]
    public InGameUi ingame_UI;
    public InventoryUI inventoryui;
    VisualElement fadeToBlackScreen;
    private bool fadeColorReached;
    private float lerpTimeElapsed;

    [Separator("Event System")]
    public EventSystem theEventSystem;

    [Separator("Important Variables")]
    public const string playerName = "Nikita";

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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
        AssignAllReferences();  //Assigns player
        DontDestroyOnLoad(this);
        SceneReady();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    public void Start()
    {
        VisualElement root = ingame_UI.GetComponent<UIDocument>().rootVisualElement;
        fadeToBlackScreen = root.Q<VisualElement>("blackSprite");

        volumeSettings = cameraManager.mainCamera.GetComponent<CinemachineVolumeSettings>();

        foreach (VolumeComponent volumeComponent in volumeSettings.m_Profile.components)
        {
            if (volumeComponent.name == "LiftGammaGain") { darkness = volumeComponent as LiftGammaGain;}
        }
        darkness.gain.value = new Vector4(1, 1, 1, 0);
    }

    public void AssignAllReferences()   
    {
        playerRespawnPoint = GameObject.FindWithTag("player_respawn");
        if (Player == null) { Player = playerManager.SpawnPlayer(); }
    }
    private void Update()
    {
        //Debug.Log($"Current Scene number is {SceneManager.GetActiveScene().buildIndex} and Scene name is {SceneManager.GetActiveScene().name}");
        //Debug.Log($"Time scale is {Time.timeScale}");
        //Debug.Log($"Gain is {darkness.gain.value}.");
        if (Keyboard.current.pKey.wasPressedThisFrame) { PauseGame(); }
        if (Keyboard.current.oKey.wasPressedThisFrame) { ResumeGame(); }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (ihuiManager.parentElement.style.display == DisplayStyle.Flex)
            {
                ihuiManager.parentElement.style.display = DisplayStyle.None;
            }
            else
            {
                ihuiManager.parentElement.style.display = DisplayStyle.Flex;
            }
            /*bool infoHubenabled = infoHub.gameObject.activeSelf;
            infoHub.gameObject.SetActive(!infoHubenabled);
            if (!infoHubenabled) { infoHub.firstPageShown = false; }
            TogglePauseGame();

            if (questManager.qBlankOnScreen)
            {
                ihuiquestmanager.ClearCurrentQuest();
                ihuiquestmanager.ShowNewQuest(questManager.lastAcceptedQuest);
            }*/
        }

        if (pause) { paused = true; }
        if (resume) { resumed = true; }


        if (overwriteTime && !hitStopped) 
        {            
            Time.timeScale = timeScale; 
        }
        else if (!hitStopped)
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
        currentSceneName = SceneManager.GetActiveScene().name;
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
                cameraManager.SetupCameras();
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

    public void PauseGame()
    {
        //Debug.Log("Pausing game...");
        //playerManager.DisablePlayer();
        Charinputs.instance.DisableAllInputs();
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
        //Debug.Log("Resuming game...");
        //playerManager.ResetPlayer();
        Charinputs.instance.EnableAllInputs();
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
        pause_and_scene_manager.PauseMenuUi.SetActive(false);
        pause_and_scene_manager.isGamePaused = false;
        ResumeGame();

    }

    public void PauseWithMenu()
    {
        Debug.Log("Pausing game...");
        pause_and_scene_manager.PauseMenuUi.SetActive(true);
        pause_and_scene_manager.isGamePaused = true;
        PauseGame();
    }

    public void FadeToBlack(float duration) //I can't figure out why this won't work.
    {
        //fadeToBlackScreen.style.display = DisplayStyle.Flex;
        //fadeToBlackScreen.AddToClassList("fadeToBlack");
        StartCoroutine(FadeToBlackCO(duration));
    }

    public void FadeFromBlack(float duration)
    {
        //fadeToBlackScreen.style.display = DisplayStyle.Flex;
        //fadeToBlackScreen.RemoveFromClassList("fadeToBlack");
        StartCoroutine(FadeFromBlackCO(duration));
    }

    public IEnumerator FadeToBlackCO(float duration)
    {
        Debug.Log("Starting fade TO black Coroutine");

        fadeColorReached = false;
        float startColor = darkness.gain.value.w;
        float endColor = -1;
        while (!fadeColorReached)
        {
            //Debug.Log($"Gain is {darkness.gain.value}.");
            yield return new WaitForEndOfFrame();

            lerpTimeElapsed += Time.deltaTime;
            float percentageComplete = lerpTimeElapsed / duration;
            float lerpedValue = Mathf.Lerp(startColor, endColor, percentageComplete);
            darkness.gain.value = new Vector4(1, 1, 1, lerpedValue);

            if (darkness.gain.value.w == -1)
            {
                Debug.Log("Screen is fully opaque");
                fadeColorReached = true;
                lerpTimeElapsed = 0;
            }
        }
    }

    public IEnumerator FadeFromBlackCO(float duration)
    {
        Debug.Log("Starting fade FROM black Coroutine");

        fadeColorReached = false;
        float startColor = darkness.gain.value.w;
        float endColor = 0;
        while (!fadeColorReached)
        {
            //Debug.Log($"Gain is {darkness.gain.value}.");
            yield return new WaitForEndOfFrame();

            lerpTimeElapsed += Time.deltaTime;
            float percentageComplete = lerpTimeElapsed / duration;
            float lerpedValue = Mathf.Lerp(startColor, endColor, percentageComplete);
            darkness.gain.value = new Vector4(1, 1, 1, lerpedValue);

            if (darkness.gain.value.w == 0)
            {
                Debug.Log("Screen is fully transparent");
                fadeColorReached = true;
                lerpTimeElapsed = 0;
            }
        }
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

    public void DoScreenShake(float intensity, float time)
    {
        StartCoroutine(cameraManager.DoScreenShake(intensity, time));
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
