using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Player_Manager : MonoBehaviour
{
    public GameObject playerPrefab;
    [ReadOnly]public GameObject playerLiveRef;
    [ReadOnly] public GameObject playerRespawnPoint;
    public bool preventPlayerMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRespawnPoint = GameManager.instance.playerRespawnPoint;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public GameObject SpawnPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLiveRef = player;
            return player;
        }
        else
        {
            playerRespawnPoint = GameManager.instance.playerRespawnPoint;
            playerLiveRef = Instantiate(playerPrefab, playerRespawnPoint.transform.position, Quaternion.identity, gameObject.transform);
            playerLiveRef.SetActive(true);
            return playerLiveRef;
        }
    }

    public void DespawnPlayer()
    {
        playerLiveRef.SetActive(false);
        Destroy(playerLiveRef);
    }

    public void DisablePlayer()
    {
        playerLiveRef.GetComponent<Renderer>().enabled = false;
        playerLiveRef.GetComponent<Animator>().enabled = false;
        playerLiveRef.GetComponentInChildren<ParticleSystem>().Pause();
        foreach (var item in playerLiveRef.GetComponentsInChildren<Renderer>())
        {
            var itemRenderer = item.GetComponent<Renderer>();
            var itemAnimator = item.GetComponent<Animator>();

            if (itemRenderer != null) { itemRenderer.enabled = false; }
            if (itemAnimator != null) { itemAnimator.enabled = false; }
        }
        preventPlayerMovement = true;
    }

    public void EnablePlayer()
    {
        playerLiveRef.GetComponent<Charcontrol>().currentState = Charcontrol.State.Idle;
        
        playerLiveRef.GetComponent<Renderer>().enabled = true;
        playerLiveRef.GetComponent<Animator>().enabled = true;
        playerLiveRef.GetComponent<Animator>().Play("Low Poly Idle");          //Plays default anim


        playerLiveRef.GetComponentInChildren<ParticleSystem>().Play();
        playerLiveRef.GetComponent<Charhealth>().ResetHealth(false);

        BoxCollider2D[] boxCols = playerLiveRef.GetComponentsInChildren<BoxCollider2D>();

        foreach (var col in boxCols)    //Disables melee gameObjects
        {
            GameObject colGO = col.gameObject;
            if (colGO.tag == "player_attackhitbox")
            {
                colGO.SetActive(false);
            }
        }

        foreach (var item in playerLiveRef.GetComponentsInChildren<Renderer>())
        {
            item.gameObject.SetActive(true);
            var itemRenderer = item.GetComponent<Renderer>();
            var itemAnimator = item.GetComponent<Animator>();

            if (itemRenderer != null) { itemRenderer.enabled = true; }
            if (itemAnimator != null) { itemAnimator.enabled = true; }
        }
        preventPlayerMovement = false;
        
    }

    public void ResetPlayer()
    {
        playerLiveRef.GetComponent<Charcontrol>().currentState = Charcontrol.State.Idle;

        playerLiveRef.GetComponent<Renderer>().enabled = true;
        playerLiveRef.GetComponent<Animator>().enabled = true;
        playerLiveRef.GetComponent<Animator>().Play("Low Poly Idle");          //Plays default anim


        playerLiveRef.GetComponentInChildren<ParticleSystem>().Play();

        BoxCollider2D[] boxCols = playerLiveRef.GetComponentsInChildren<BoxCollider2D>();

        foreach (var col in boxCols)    //Disables melee gameObjects
        {
            GameObject colGO = col.gameObject;
            if (colGO.tag == "player_attackhitbox")
            {
                colGO.SetActive(false);
            }
        }

        foreach (var item in playerLiveRef.GetComponentsInChildren<Renderer>())
        {
            item.gameObject.SetActive(true);
            var itemRenderer = item.GetComponent<Renderer>();
            var itemAnimator = item.GetComponent<Animator>();

            if (itemRenderer != null) { itemRenderer.enabled = true; }
            if (itemAnimator != null) { itemAnimator.enabled = true; }
        }
        preventPlayerMovement = false;
    }

    bool playerPosRecorded = false;
    Vector2 playerPosAtPause;
    // Update is called once per frame
    public void Update()
    {
        if (preventPlayerMovement)
        {
            if (!playerPosRecorded)
            {
                playerPosAtPause = playerLiveRef.transform.position;
                playerPosRecorded = true;
            }
            playerLiveRef.transform.position = playerPosAtPause;
        }
        else
        {
            playerPosRecorded = false;
        }
    }
}
