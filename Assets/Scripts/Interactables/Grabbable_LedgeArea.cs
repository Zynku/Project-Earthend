using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Grabbable_LedgeArea : MonoBehaviour
{
    [ReadOnly] public GameObject Player;
    public GameObject PlayerLoc;    //The location the player will be attached to when it grabs the ledge
    public GameObject PlayerAfterGrabLoc;   //The location the player is deposited at when it is done pulling up
    Animator animator;
    [HideInInspector] public int ledgeDirInt;
    public LedgeDir direction;
    public enum LedgeDir    //The direction the player must hold in order to face the edge
    {
        Left,
        Right
    }   

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (direction)
        {
            case LedgeDir.Left:
                ledgeDirInt = -1;
                break;
            case LedgeDir.Right:
                ledgeDirInt = 1;
                break;
            default:
                ledgeDirInt = 0;
                break;
        }
    }

    public bool DoesPlayerDirMatch()
    {
        if (Player.GetComponent<Charcontrol>().facingDir == 1 && direction == LedgeDir.Right)
        {
            return true;
        }
        else if (Player.GetComponent<Charcontrol>().facingDir == -1 && direction == LedgeDir.Left)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LetPlayerIdle()
    {
        Player.GetComponent<Charcontrol>().FinishLedgePullUp();
    }

    public void PullUpPlayer()
    {
        //animator.Play("Grabbable Ledge Pull Up Player Loc");
    }
}
