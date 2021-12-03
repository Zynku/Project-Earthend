using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    [Header("Time")]
    public bool overwriteTime;
    [Range(0.001f, 1f)]
    public float timeScale = 1;

    [Header("Frame Rate")]
    [Range(1f, 60f)]
    public int frameRate = -1;

    public GameObject hurtScreen;
    private Animator hurtScreenAnimator;

    public GameObject Player;
    private Animator PlayerAnim;
    public GameObject playerRespawnPoint;
    public GameObject[] Enemies;

    public bool mainCharRespawned = false;

    public static bool pause, resume;
    public bool paused, resumed;

    public static Gamemanager instance;

    public DialogueManager dialogueManager;
    public QuestManager questManager;
    public InfoHubManager infoHub;
    public Pause_menu_manager pause_Menu_Manager;




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

        hurtScreenAnimator = hurtScreen.GetComponent<Animator>();
        hurtScreen.SetActive(false);

        dialogueManager = GetComponentInChildren<DialogueManager>();
        questManager = GetComponentInChildren<QuestManager>();
        infoHub = GetComponentInChildren<InfoHubManager>();
        pause_Menu_Manager = GetComponentInChildren<Pause_menu_manager>();

    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) { PauseGame(); }
        if (Input.GetKeyDown(KeyCode.O)) { ResumeGame(); }

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
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game...");
        Player.SetActive(true);
        PlayerAnim.enabled = true;
        pause_Menu_Manager.isGamePaused = false;
        Time.timeScale = 1;
    }

    public void TogglePauseGame()
    {
        if (pause_Menu_Manager.isGamePaused == false)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeFromMenu()
    {
        Debug.Log("Resuming game...");
        //PlayerAnim.enabled = true;
        pause_Menu_Manager.PauseMenuUi.SetActive(false);
        //Time.timeScale = 1;
        pause_Menu_Manager.isGamePaused = false;
        Gamemanager.instance.ResumeGame();

    }

    public void PauseWithMenu()
    {
        Debug.Log("Pausing game...");
        //PlayerAnim.enabled = false;
        pause_Menu_Manager.PauseMenuUi.SetActive(true);
        //Time.timeScale = 0;
        pause_Menu_Manager.isGamePaused = true;
        Gamemanager.instance.PauseGame();
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
