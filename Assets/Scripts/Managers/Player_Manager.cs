using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Player_Manager : MonoBehaviour
{
    public GameObject playerPrefab;
    [ReadOnly]public GameObject playerLiveRef;
    [ReadOnly] public GameObject playerRespawnPoint;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
