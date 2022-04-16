using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class CrouchArea : MonoBehaviour
{
    public GameObject crouchingTriggerArea;
    public bool playerInTriggerArea;
    private SpriteRenderer cTASprite;
    public GameObject crouchingTriggerStayZone;
    public bool playerInTriggerStayZone;
    private SpriteRenderer cTSZSprite;
    public GameObject crouchingPointer;
    private SpriteRenderer cPSprite;

    private void Start()
    {
        cTASprite = crouchingTriggerArea.GetComponent<SpriteRenderer>();
        cTSZSprite = crouchingTriggerStayZone.GetComponent<SpriteRenderer>();
        cPSprite = crouchingPointer.GetComponent<SpriteRenderer>();

        cTASprite.enabled = false;
        cTSZSprite.enabled = false;
        cPSprite.enabled = false;
    }

    private void Update()
    {
        cPSprite.enabled = playerInTriggerArea;
    }
}
