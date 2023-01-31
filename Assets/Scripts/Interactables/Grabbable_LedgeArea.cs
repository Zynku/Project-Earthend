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
    Charcontrol charcontrol;

    [HideInInspector] public int ledgeDirInt;
    public float ledgeCoolDownTargetTime;
    public float ledgeCoolDownTime;

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
        charcontrol = Player.GetComponent<Charcontrol>();
    }

    // Update is called once per frame
    void Update()
    {//This is some magic code from Visual studio
        ledgeDirInt = direction switch
        {
            LedgeDir.Left => -1,
            LedgeDir.Right => 1,
            _ => 0,
        };

        if (ledgeCoolDownTime > 0) { ledgeCoolDownTime -= Time.deltaTime; }
    }

    public bool DoesPlayerDirMatch()
    {
        if (charcontrol.facingDir == 1 && direction == LedgeDir.Right)
        {
            return true;
        }
        else if (charcontrol.facingDir == -1 && direction == LedgeDir.Left)
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
        charcontrol.FinishLedgePullUp();
    }

    public void PullUpPlayer()
    {
        //animator.Play("Grabbable Ledge Pull Up Player Loc");
    }

    public void ResetCoolDown()
    {
        ledgeCoolDownTime = ledgeCoolDownTargetTime;
    }
}
