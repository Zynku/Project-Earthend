using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_animation : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Animator anim;
    public bool Running;
    public bool DetectingRightapparently;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();  
    }

    public void FixedUpdate()
    {
        RunningAnim();

        if (gameObject.GetComponent<Find_char>().PlayerRight == true)
        {
            DetectingRightapparently = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Find_char>().PlayerLeft == true || gameObject.GetComponent<Find_char>().PlayerLeft == true)
        {
            Running = true;
        }

        if (gameObject.GetComponent<Find_char>().PlayerRight == false || gameObject.GetComponent<Find_char>().PlayerRight == false)
        {
            Running = false;
        }

    }
    public void RunningAnim()
    {
        if (Running == true)
        {
            anim.SetBool("Enemy_Run", true);
        }

        if (Running == false)
        {
            anim.SetBool("Enemy_Run", false);
        }
    }

}
