using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
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
    public GameObject playerRespawnPoint;
    public GameObject[] Enemies;

    public bool mainCharRespawned = false;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        playerRespawnPoint = GameObject.FindWithTag("player_respawn");
        if (!overwriteTime) Time.timeScale = 1f;

        hurtScreenAnimator = hurtScreen.GetComponent<Animator>();
        hurtScreen.SetActive(false);
    }

    private void Update()
    { 
        if (overwriteTime) 
        {            
            Time.timeScale = timeScale; 
        }
        Application.targetFrameRate = frameRate;

        Charhealth.Hit += HurtFlash;
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
