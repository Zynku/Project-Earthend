using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_detection : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Animator animator;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask Groundlayer;

    [SerializeField] Transform groundCheck;

    [Header("Input Checks")]
    public bool Up;

    [Header("State Checks")]
    public bool isGrounded;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

    }


    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Up = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Up = false;
        }
    }


    void Update()
    {
        //Creates Raycast between groundcheck object and ground
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
            animator.SetBool("Grounded", true);
        }
        else
        {
            isGrounded = false;
            animator.SetBool("Grounded", false);
        }
    }
}
