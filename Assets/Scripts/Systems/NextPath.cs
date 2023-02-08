using Cinemachine;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class NextPath : MonoBehaviour
{
    private GameObject player;
    public EnterDir enterDirection;              //Which direction does the player go to enter this nextpath?
    public GameObject teleportPoint;
    public GameObject linkedPath;
    public GameObject cameraStayPos;
    public GameObject block;
    public CinemachineVirtualCamera myCamera;
    [HideInInspector] public NextPath linkedPathScript;

    public float pathCoolDown = 0;
    public float pathCoolDownTargetTime = 1f;
    [HideInInspector] public bool pathLinked;

    //TODO: Add an event scripts can subscribe to

    public enum EnterDir
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    private void Start()
    {
        player = GameManager.instance.Player;
        myCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        teleportPoint.GetComponent<SpriteRenderer>().enabled = false;
        cameraStayPos.GetComponent<SpriteRenderer>().enabled = false;
        block.GetComponent<SpriteRenderer>().enabled = false;
        block.SetActive(false);
        if (linkedPath != null) //If this is the path that is assigned its destination in the inspector, it automatically sets up the destination path
        {
            linkedPathScript = linkedPath.GetComponent<NextPath>();
            pathLinked = true;
            StartRemote();  //Remotely sets up the destination path
        }
    }

    [ButtonMethod]
    public void ChangeTPPointColor()
    {
        SpriteRenderer tpPointRenderer = GetComponentInChildren<SpriteRenderer>();
        tpPointRenderer.color = Random.ColorHSV(0, 1, 0.7f, 1);
    }

    public void StartRemote()
    {
        if (linkedPathScript == null) { linkedPathScript = linkedPath.GetComponent<NextPath>(); }
        linkedPathScript.linkedPath = this.gameObject;
        linkedPathScript.linkedPathScript = this;
        linkedPathScript.pathLinked = true;
    }

    private void Update()
    {
        if(pathCoolDown > 0) { pathCoolDown -= Time.deltaTime; }
        else { pathCoolDown = 0; block.SetActive(false); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player":
                if (pathCoolDown != 0) { Debug.Log($"{gameObject.name} is currently on cooldown, cannot teleport"); return; }
                myCamera.Priority = 100;
                if (enterDirection == EnterDir.RIGHT) { GameManager.instance.playerManager.ForceRun(Player_Manager.WalkDirection.RIGHT, 1); }
                else if (enterDirection == EnterDir.LEFT) { GameManager.instance.playerManager.ForceRun(Player_Manager.WalkDirection.LEFT, 1); }
                StartCoroutine(TeleportPlayer());
                pathCoolDown = pathCoolDownTargetTime;
                break;
        }
    }

    public IEnumerator TeleportPlayer()
    {
        if (linkedPath == null) { Debug.LogWarning("This path is not linked to another, no teleportation can happen"); yield break; }

        StartCoroutine(Charinputs.instance.DisableMovementOnlyForDuration(1.5f));
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.FadeToBlack(0.5f);
        yield return new WaitForSeconds(0.6f);
        MovePlayer();   //Teleports to new path location
        myCamera.Priority = 5;
        linkedPathScript.myCamera.Priority = 100;                                               //TODO: FUCKING FADE TO BLACK ONLY WORKS ON MAIN CAM. MAKE IT APPLY TO ALL CAMS BOI
        if (linkedPathScript.enterDirection == EnterDir.RIGHT) { GameManager.instance.playerManager.ForceRun(Player_Manager.WalkDirection.LEFT, .6f); }
        else if (linkedPathScript.enterDirection == EnterDir.LEFT) { GameManager.instance.playerManager.ForceRun(Player_Manager.WalkDirection.RIGHT, .6f); }
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.FadeFromBlack(0.5f);
        yield return new WaitForSeconds(0.4f);
        linkedPathScript.myCamera.Priority = 5;
        block.SetActive(true);
        linkedPathScript.block.SetActive(true);
    }

    public void MovePlayer()
    {
        player.transform.position = linkedPathScript.teleportPoint.transform.position;
        pathCoolDown = pathCoolDownTargetTime;
        linkedPathScript.pathCoolDown = pathCoolDownTargetTime;
    }
}
