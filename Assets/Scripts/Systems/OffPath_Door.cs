using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OffPath_Door : MonoBehaviour
{
    GameObject player;
    OffPath_Door linkedDoorScript;
    public GameObject linkedDoor;
    private bool doorLinked = false;

    public bool playerInRange;
    public bool playerAtMiddle;
    public float interactionRange;
    private float doorcooldown = 0;
    private float doorcooldowntargettime = 0.5f;
    public float lerpTargetDuration = 0.01f;
    private float lerpTimeElapsed;
    public float minimumRange = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.Player;
        if (linkedDoor != null) 
        {
            linkedDoorScript = linkedDoor.GetComponent<OffPath_Door>();
            doorLinked = true;
            StartRemote();
        }
        
    }
    public void StartRemote()
    {
        if (linkedDoorScript == null) { linkedDoorScript = linkedDoor.GetComponent<OffPath_Door>(); }
        linkedDoorScript.linkedDoor = this.gameObject;
        linkedDoorScript.linkedDoorScript = this;
        linkedDoorScript.doorLinked = true;
    }


    // Update is called once per frame
    void Update()
    {
        doorcooldown -= Time.deltaTime;
        if (doorcooldown < 0) { doorcooldown = 0; }

        if (Vector3.Distance(player.transform.position, transform.position) < interactionRange)
        { playerInRange = true; }
        else { playerInRange = false; }

        if (Input.GetAxisRaw("Interact") > 0)
        {
            if (!playerInRange) { return; }
            if (doorLinked)
            {
                if (doorcooldown != 0) { return; }
                StartCoroutine(MovePlayerToMiddle());
            }
            else
            {
                Debug.LogWarning("No linked door found, offPath will not work!");
            }
        }
    }

    public IEnumerator MovePlayerToMiddle()
    {
        Charcontrol charcontrol = player.GetComponent<Charcontrol>();
        playerAtMiddle = false;
        lerpTimeElapsed = 0;
        float playerYPos = player.transform.position.y;
        Vector2 startPosition = player.transform.position;
        Vector2 endPosition = transform.position;
        while (!playerAtMiddle)
        {
            //Debug.Log($"Player is {Vector3.Distance(player.transform.position, transform.position)} away");
            lerpTimeElapsed += Time.deltaTime;
            float percentageComplete = lerpTimeElapsed / lerpTargetDuration;
            player.transform.position = Vector2.Lerp(startPosition, endPosition, percentageComplete);
            player.transform.position = new Vector3(player.transform.position.x, playerYPos, player.transform.position.z);
            yield return new WaitForEndOfFrame();
            if (Vector3.Distance(player.transform.position, transform.position) < minimumRange && !playerAtMiddle)
            {
                playerAtMiddle = true;
                //Debug.Log("Player is in the middle of the door!");
                Coroutine FadeOut = StartCoroutine(GameManager.instance.playerManager.DoFadeOut(0.2f));
                yield return new WaitForSeconds(0.5f);
                MovePlayer();
                yield return new WaitForSeconds(0.2f);
                StopCoroutine(FadeOut);
                StartCoroutine(GameManager.instance.playerManager.DoFadeIn(0.2f));
            }
        }
        
    }

    public void MovePlayer()
    {
        player.transform.position = linkedDoor.transform.position;
        doorcooldown = doorcooldowntargettime;
        linkedDoorScript.doorcooldown = linkedDoorScript.doorcooldowntargettime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
