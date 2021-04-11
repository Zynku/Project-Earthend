using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    public GameObject Player;
    public GameObject playerRespawnPoint;
    public GameObject[] Enemies;

    public bool mainCharRespawned = false;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        playerRespawnPoint = GameObject.FindWithTag("player_respawn");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Enemies = GameObject.FindGameObjectsWithTag("enemy");
        if (Player.gameObject.GetComponent<Charhealth>().currentHealth <= 0)
        {
            Invoke("ReloadScene", 3f);
        }
    }

    void RespawnPlayer()
    {
        if (mainCharRespawned == false)
        {
            Player.gameObject.transform.position = playerRespawnPoint.gameObject.transform.position;
            Char_control.dead = false;
            mainCharRespawned = true;
            
        }
    }

  
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
