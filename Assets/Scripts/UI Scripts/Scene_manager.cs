using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene_manager : MonoBehaviour
{
    
    public GameObject MainMenuUi;
    public GameObject LoadingScreen;
    public Slider loadingSlider;

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

    public void ApplicationClose()
    {
        Application.Quit();
    }
}
