using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Pause_menu_manager : MonoBehaviour
{
    private GameObject Player;
    private Animator PlayerAnim;
    

    public static bool isGamePaused = false;
    public GameObject PauseMenuUi;
    public GameObject LoadingScreen;
    public Slider loadingSlider;

    [Header("Audio")]
    public AudioMixer masterMixer;
    public Slider masterVol;
    public Slider soundVol;
    public Slider musicVol;

    void Awake()
    {
        masterMixer.SetFloat("MasterVolume", global_script.masterVolfloat);
        masterMixer.SetFloat("SoundVolume", global_script.soundVolfloat);
        masterMixer.SetFloat("MusicVolume", global_script.musicVolfloat);

        masterVol.value = global_script.masterVolfloat;
        soundVol.value = global_script.soundVolfloat;
        musicVol.value = global_script.musicVolfloat;
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerAnim = Player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Activates pause menu and stops time if pause menu is not already activated. Restarts time and deactivates pause menu if it is activated
        if (Input.GetKeyDown("escape"))
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
    }

    public void SceneSWitcher(int index)
    {
        //Loads scene without disabling any other gameobjects so that progress bar can run
        Time.timeScale = 1;
        StartCoroutine(LoadAsynchronously(index));
    }

    IEnumerator LoadAsynchronously(int index)
    {
        //Gets operation values from scenemanager loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        LoadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            //Do some quick maths and apply loading progress to float, then slider bar
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }

    public void Resume()
    {
        PlayerAnim.enabled = true;
        PauseMenuUi.SetActive(false);
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void Pause()
    {
        PlayerAnim.enabled = false;
        PauseMenuUi.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;
    }


    public void ApplicationClose()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void SetMasterVolume(float volume)
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
    }
}
