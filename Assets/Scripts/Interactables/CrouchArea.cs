using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class CrouchArea : MonoBehaviour
{
    public GameObject Player;
    public GameObject crouchingTriggerArea;
    public bool playerInTriggerArea;
    private SpriteRenderer cTASprite;
    public GameObject crouchingTriggerStayZone;
    public bool playerInTriggerStayZone;
    private SpriteRenderer cTSZSprite;
    public GameObject crouchingPointer;
    private SpriteRenderer cPSprite;
    public GameObject blockOffCollider;
    private SpriteRenderer bOCSprite;

    private void Start()
    {
        Player = GameManager.instance.Player;
        cTASprite = crouchingTriggerArea.GetComponent<SpriteRenderer>();
        cTSZSprite = crouchingTriggerStayZone.GetComponent<SpriteRenderer>();
        cPSprite = crouchingPointer.GetComponent<SpriteRenderer>();
        bOCSprite = blockOffCollider.GetComponent<SpriteRenderer>();

        cTASprite.enabled = false;
        cTSZSprite.enabled = false;
        cPSprite.enabled = false;
        bOCSprite.enabled = false;
    }

    private void Update()
    {
        cPSprite.enabled = playerInTriggerArea;

        Charcontrol charcontrol = Player.GetComponent<Charcontrol>();
        if (charcontrol.currentState == Charcontrol.State.CrouchWalking || charcontrol.currentState == Charcontrol.State.Crouching_Idle)
        {
            blockOffCollider.gameObject.SetActive(false);
        }
        else
        {
            blockOffCollider.gameObject.SetActive(true);
        }
    }
}
