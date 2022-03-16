using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Respawn_menu_manager : MonoBehaviour
{
    public GameObject RespawnScreenUI;
    public GameObject Player;
    public Charhealth healthScript;
    private Animator PlayerAnim;
    public GameObject playerRespawnPoint;

    public GameObject LoadingScreen;
    public Slider loadingSlider;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        healthScript = Player.GetComponent<Charhealth>();
        PlayerAnim = Player.GetComponent<Animator>();
        playerRespawnPoint = GameObject.FindWithTag("player_respawn");
        RespawnScreenUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthScript.currentHealth <= 0)
        {
            float showRespawnOptionsDelay = 3.5f;
            Invoke("ShowRespawnOptions", showRespawnOptionsDelay);
        }
    }

    public void ShowRespawnOptions()
    {
        RespawnScreenUI.SetActive(true);
        PlayerAnim.enabled = false;
        Time.timeScale = 0;
    }

    public void RespawnPlayer()
    {
        bool mainCharRespawned = false;
        if (mainCharRespawned == false)
        {
            Player.gameObject.transform.position = playerRespawnPoint.gameObject.transform.position;
            Char_control.dead = false;
            mainCharRespawned = true;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void ApplicationClose()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
