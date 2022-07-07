using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using MyBox;
using TMPro;

public class Pause_menu_manager : MonoBehaviour
{
    private GameObject Player;
    private BGAudioScript BGAud;
    private Animator PlayerAnim;
    private EventSystem eventsystem;
    

    public bool isGamePaused = false;
    public int sceneToLoad;
    public GameObject PauseMenuUi;
    public GameObject LoadingScreen;
    public GameObject LoadingScreenBG;
    public GameObject continueText;
    public Slider loadingSlider;
    public float minLoadingTargetTime;
    [ReadOnly] public float minLoadingTimer;
    public bool loading;
    public bool inputToContinue;

    [Header("Audio")]
    public AudioMixer masterMixer;
    public Slider masterVol;
    public Slider soundVol;
    public Slider musicVol;

    void Awake()
    {
        //masterMixer.SetFloat("MasterVolume", global_script.masterVolfloat);
        //masterMixer.SetFloat("SoundVolume", global_script.soundVolfloat);
        //masterMixer.SetFloat("MusicVolume", global_script.musicVolfloat);

        //masterVol.value = global_script.masterVolfloat;
        //soundVol.value = global_script.soundVolfloat;
        //musicVol.value = global_script.musicVolfloat;

        eventsystem = FindObjectOfType<EventSystem>();
    }

    public void HidePauseUI()
    {
        PauseMenuUi.SetActive(false);
    }

    public void HideLoadingScreen()
    {
        LoadingScreen.SetActive(false);
    }

    private void Start()
    {
        Player = GameManager.instance.Player;
        BGAud = GameManager.instance.BGAudioManager;
        PlayerAnim = Player.GetComponent<Animator>();
        PauseMenuUi.SetActive(false);
        LoadingScreen.SetActive(false);
        continueText.SetActive(false);
        LoadingScreenBG.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Activates pause menu and stops time if pause menu is not already activated. Restarts time and deactivates pause menu if it is activated
        if (Input.GetKeyDown("escape") && GameManager.instance.currentScenePausable)
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && loading)
        {
            inputToContinue = true;
        }
    }

    public void SceneSwitcher(int index)
    {
        //Loads scene without disabling any other gameobjects so that progress bar can run
        Time.timeScale = 1;
        sceneToLoad = index;
        StartCoroutine(BGAud.FadeAudioMixer(BGAud.masterMixer.audioMixer, "MasterVol", 1f, 0f));
        LoadingScreen.SetActive(true);
        continueText.SetActive(false);
        LoadingScreenBG.SetActive(true);
        LoadingScreenBG.GetComponent<LoadingScreenBG>().StartFading();
    }

    public void ReloadCurrentScene()
    {
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex));
    }

    public void FadeOutFinished()   //Called from Loading Screen BG Script
    {
        StartCoroutine(LoadAsynchronously(sceneToLoad));
    }
    IEnumerator LoadAsynchronously(int index)
    {
        //Gets operation values from scenemanager loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        minLoadingTimer = minLoadingTargetTime;
        LoadingScreen.SetActive(true);
        loading = true;

        while (!operation.isDone)
        {
            //Do some quick maths and apply loading progress to float, then slider bar
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            minLoadingTimer -= Time.deltaTime;
            yield return null;
        }

        while (minLoadingTimer >= 0)
        {
            minLoadingTimer -= Time.deltaTime;
            yield return null;
        }

        continueText.SetActive(true);

        while (!inputToContinue)
        {
            yield return null;
        }
        
        if (inputToContinue)
        {
            LoadingScreen.SetActive(false);
            LoadingScreenBG.SetActive(false);
            loadingSlider.value = 0;
            inputToContinue = false;
            loading = false;
            GameManager.instance.SceneReady();
        }
    }


    public void Resume()
    {
        PauseMenuUi.SetActive(false);
        isGamePaused = false;
        GameManager.instance.ResumeGame();
    }

    public void Pause()
    {
        PauseMenuUi.SetActive(true);
        isGamePaused = true;
        GameManager.instance.PauseGame();
    }


    public void ApplicationClose()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    //For some reason these 3 methods absolutely destroy this script. Uncomment at your own risk
    /*public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MusicVolume", volume);
    }

    public void SetSoundVolume(float volume)
    {
        masterMixer.SetFloat("SoundVolume", volume);
    }*/
}
