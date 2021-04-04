using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charinputcontrol : MonoBehaviour
{
    Rigidbody2D rb2d;
    public inputDevice currentDevice;
    [Header("Left")]
    public bool left;
    public bool leftDown;
    public bool leftUp;
    public float leftAxis;


    [Header("Right")]
    public bool right;
    public bool rightDown;
    public bool rightUp;
    public float rightAxis;

    [Header("Down")]
    public bool down;
    public bool downDown;
    public bool downUp;
    public float downAxis;

    [Header("Up")]
    public bool up;
    public bool upDown;
    public bool upUp;
    public float upAxis;

    [Header("Light Attack")]
    public bool lightAttack;
    public bool lightAttackDown;
    public bool lightAttackUp;

    [Header("Heavy Attack")]
    public bool heavyAttack;
    public bool heavyAttackDown;
    public bool heavyAttackUp;

    [Header("Ranged Attack")]
    public bool rangedAttack;
    public bool rangedAttackDown;
    public bool rangedAttackUp;

    [Header("Misc")]
    public bool interact;
    public bool interactDown;
    public bool interactUp;

    [Header("States")]
    public float horizontalDir;

    

    public enum inputDevice
    {
        Keyboard,
        Mobile
    }


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }



    // Update is called once per frame
    void Update()
    {
        switch (currentDevice)
        {

            //-------------------------------------------------------------------------------------------------------------------------------------
            case inputDevice.Keyboard:
                //STATES
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    horizontalDir = -1;
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    horizontalDir = 1;
                }


                //LEFT
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                { leftDown = true;}
                else
                { leftDown = false; }

                if (Input.GetKeyUp(KeyCode.LeftArrow))
                { leftUp = true;}
                else 
                { leftUp = false; }

                if (Input.GetKey(KeyCode.LeftArrow)) 
                { left = true; }
                else
                { left = false; }

                if (Input.GetAxis("Horizontal") < 0)
                {
                    leftAxis = Input.GetAxis("Horizontal");
                }
                else
                {
                    leftAxis = 0;
                }
                


                //RIGHT
                if (Input.GetKeyDown(KeyCode.RightArrow))
                { rightDown = true; }
                else
                { rightDown = false; }

                if (Input.GetKeyUp(KeyCode.RightArrow))
                { rightUp = true; }
                else
                { rightUp = false; }

                if (Input.GetKey(KeyCode.RightArrow))
                { right = true; }
                else
                { right = false; }

                if (Input.GetAxis("Horizontal") > 0)
                {
                    rightAxis = Input.GetAxis("Horizontal");
                }
                else
                {
                    rightAxis = 0;
                }


                //UP
                if (Input.GetKeyDown(KeyCode.UpArrow))
                { upDown = true; }
                else
                { upDown = false; }

                if (Input.GetKeyUp(KeyCode.UpArrow))
                { upUp = true; }
                else
                { upUp = false; }

                if (Input.GetKey(KeyCode.UpArrow))
                { up = true; }
                else
                { up = false; }

                if (Input.GetAxis("Vertical") > 0)
                {
                    upAxis = Input.GetAxis("Vertical");
                }
                else
                {
                    upAxis = 0;
                }


                //DOWN
                if (Input.GetKeyDown(KeyCode.DownArrow))
                { downDown = true; }
                else
                { downDown = false; }

                if (Input.GetKeyUp(KeyCode.DownArrow))
                { downUp = true; }
                else
                { downUp = false; }

                if (Input.GetKey(KeyCode.DownArrow))
                { down = true; }
                else
                { down = false; }

                if (Input.GetAxis("Vertical") < 0)
                {
                    downAxis = Input.GetAxis("Vertical");
                }
                else
                {
                    downAxis = 0;
                }


                //LIGHT ATTACK
                if (Input.GetKeyDown(KeyCode.D))
                { lightAttackDown = true; }
                else
                { lightAttackDown = false; }

                if (Input.GetKeyUp(KeyCode.D))
                { lightAttackUp = true; }
                else
                { lightAttackUp = false; }

                if (Input.GetKey(KeyCode.D))
                { lightAttack = true; }
                else
                { lightAttack = false; }


                //HEAVY ATTACK
                if (Input.GetKeyDown(KeyCode.S))
                { heavyAttackDown = true; }
                else
                { heavyAttackDown = false; }

                if (Input.GetKeyUp(KeyCode.S))
                { heavyAttackUp = true; }
                else
                { heavyAttackUp = false; }

                if (Input.GetKey(KeyCode.S))
                { heavyAttack = true; }
                else
                { heavyAttack = false; }


                //RANGED ATTACK
                if (Input.GetKeyDown(KeyCode.A))
                { rangedAttackDown = true; }
                else
                { rangedAttackDown = false; }

                if (Input.GetKeyUp(KeyCode.A))
                { rangedAttackUp = true; }
                else
                { rangedAttackUp = false; }

                if (Input.GetKey(KeyCode.A))
                { rangedAttack = true; }
                else
                { rangedAttack = false; }


                //MISC
                if (Input.GetKeyDown(KeyCode.E))
                { interactDown = true; }
                else
                { interactDown = false; }

                if (Input.GetKeyUp(KeyCode.E))
                { interactUp = true; }
                else
                { interactUp = false; }

                if (Input.GetKey(KeyCode.E))
                { interact = true; }
                else
                { interact = false; }

                break;
            //-------------------------------------------------------------------------------------------------------------------------------------
            
            case inputDevice.Mobile:
                break;
            //-------------------------------------------------------------------------------------------------------------------------------------
        }
    }
}
