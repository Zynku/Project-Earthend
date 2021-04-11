using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Main_Menu_manager : MonoBehaviour
{
    public AudioMixer masterMixer;
    
    public GameObject MainMenuUi;
    public GameObject LoadingScreen;
    public Slider loadingSlider;

    [Header("Audio")]
    public static float masterVolfloat;
    public static float soundVolfloat;
    public static float musicVolfloat;

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

    private void Update()
    {

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

        //While its loading...
        while (!operation.isDone)
        {
            Debug.Log("Loading...");
            //Do some quick maths and apply loading progress to float, then slider bar
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }

    public void ApplicationClose()
    {
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
