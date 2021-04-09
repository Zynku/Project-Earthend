using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause_menu_manager : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject PauseMenuUi;
    public GameObject LoadingScreen;
    public Slider loadingSlider;

    // Start is called before the first frame update
    void Start()
    {
        
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

        //While its loading...
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
        PauseMenuUi.SetActive(false);
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void Pause()
    {
        PauseMenuUi.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;
    }


    public void ApplicationClose()
    {
        Application.Quit();
    }
}
