using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class OffPath_Door : MonoBehaviour
{
    [Foldout("Necessary Components", true)]
    [MustBeAssigned] public GameObject pointer;
    [MustBeAssigned] public GameObject doorNameGameObject;

    [Foldout("Important GameObjects", true)]
    GameObject player;
    OffPath_Door linkedDoorScript;
    public GameObject linkedDoor;

    [Foldout("Variables", true)]
    private bool doorLinked = false;
    public string doorDestination;
    public bool doFadeOut = true;
    [ReadOnly]public bool playerInRange;
    [ReadOnly]public bool playerAtMiddle;
    
    private float interactionRange = 0.3f;
    private float doorcooldown = 0;
    private float doorcooldowntargettime = 0.5f;
    private float lerpTargetDuration = 0.5f;
    private float lerpTimeElapsed;
    private float minimumRange = 0.05f;
    
    void Start()
    {
        player = GameManager.instance.Player;
        doorNameGameObject.GetComponent<TextMeshPro>().text = doorDestination;
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
            doorNameGameObject.SetActive(true);
        }
        else
        {
            playerInRange = false;
            pointer.SetActive(false);   //Makes sure they're disabled when not close
            doorNameGameObject.SetActive(false);
        }

        if (Charinputs.instance.interact.WasPressedThisFrame())   //If the player interacts
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
        float inputDisabledTime;
        if (doFadeOut) { inputDisabledTime = 2; }
        else { inputDisabledTime = 1; }
        StartCoroutine(Charinputs.instance.DisableAllInputsForDuration(inputDisabledTime));        //Stops player from moving around
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
                if (doFadeOut) 
                {
                    GameManager.instance.FadeToBlack(0.5f);
                    yield return new WaitForSeconds(0.6f);
                }
                MovePlayer();   //Teleports to new door location
                yield return new WaitForSeconds(0.3f);
                if (doFadeOut) 
                {
                    GameManager.instance.FadeFromBlack(0.5f);
                    yield return new WaitForSeconds(0.4f);
                }
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

    [ButtonMethod]
    public void ChangeDoorNameInEditor()
    {
        doorNameGameObject.GetComponent<TextMeshPro>().text = doorDestination;
    }

    [ButtonMethod]
    public void ResetLinkedDoor()
    {
        linkedDoor = null;
        linkedDoorScript = null;
        doorLinked = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
