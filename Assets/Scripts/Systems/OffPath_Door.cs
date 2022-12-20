using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class OffPath_Door : MonoBehaviour
{
    [Separator("Important GameObjects")]
    GameObject player;
    OffPath_Door linkedDoorScript;
    public GameObject linkedDoor;
    public GameObject pointer;
    public GameObject doorName;

    [Separator("Variables")]
    private bool doorLinked = false;
    public string doorDestination;
    public float interactionRange;
    [ReadOnly]public bool playerInRange;
    [ReadOnly]public bool playerAtMiddle;
    
    [Separator("Ranges")]
    private float doorcooldown = 0;
    private float doorcooldowntargettime = 0.5f;
    public float lerpTargetDuration = 0.01f;
    private float lerpTimeElapsed;
    public float minimumRange = 0.05f;
    
    void Start()
    {
        player = GameManager.instance.Player;
        doorName.GetComponent<TextMeshPro>().text = doorDestination;
        if (linkedDoor != null) //If this is the door that is assigned its destination in the inspector, it automatically sets up the destination door
        {
            linkedDoorScript = linkedDoor.GetComponent<OffPath_Door>();
            doorLinked = true;
            StartRemote();  //Remotely sets up the destination door
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

        if (Vector3.Distance(player.transform.position, transform.position) < interactionRange) //If player is closer than interaction range
        { 
            playerInRange = true;
            pointer.SetActive(true);    //Enables name and pointer
            doorName.SetActive(true);
        }
        else
        {
            playerInRange = false;
            pointer.SetActive(false);   //Makes sure they're disabled when not close
            doorName.SetActive(false);
        }

        if (Input.GetAxisRaw("Interact") > 0)   //If the player interacts
        {
            if (!playerInRange) { return; }
            if (doorLinked)
            {
                if (doorcooldown != 0) { return; }
                StartCoroutine(MovePlayerToMiddle());   //If the door isnt on cooldown, lerp the player to the center of the door
                doorcooldown = doorcooldowntargettime;
            }
            else
            {
                Debug.LogWarning("No linked door found, offPath will not work!");
            }
        }
    }

    public IEnumerator MovePlayerToMiddle()
    {
        Debug.Log("Starting door procedures");
        playerAtMiddle = false;
        lerpTimeElapsed = 0;
        float playerYPos = player.transform.position.y; //Records current Y pos so we stay constant
        Vector2 startPosition = player.transform.position;
        Vector2 endPosition = transform.position;   //End position is the door transform point
        while (!playerAtMiddle) //While loop until player reaches the middle
        {
            //Debug.Log($"Player is {Vector3.Distance(player.transform.position, transform.position)} away");
            lerpTimeElapsed += Time.deltaTime;
            float percentageComplete = lerpTimeElapsed / lerpTargetDuration;
            player.transform.position = Vector2.Lerp(startPosition, endPosition, percentageComplete);   //Lerps between the start and end positions
            player.transform.position = new Vector3(player.transform.position.x, playerYPos, player.transform.position.z);
            yield return new WaitForEndOfFrame();   //Must wait til end of frame or this will happen multiple times per frame
            if (Vector3.Distance(player.transform.position, transform.position) < minimumRange && !playerAtMiddle)  //Once we reach the middle, start doing things
            {
                playerAtMiddle = true;
                //Debug.Log("Player is in the middle of the door!");
                StartCoroutine(GameManager.instance.playerManager.DoFadeOut(0.2f)); //Fade Player out
                yield return new WaitForSeconds(0.2f);
                GameManager.instance.FadeToBlack(0.5f);
                yield return new WaitForSeconds(0.6f);
                MovePlayer();   //Teleports to new door location
                yield return new WaitForSeconds(0.1f);
                GameManager.instance.FadeFromBlack(0.5f);
                yield return new WaitForSeconds(0.4f);
                StartCoroutine(GameManager.instance.playerManager.DoFadeIn(0.2f));  //Fade Player in
                yield break;
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
