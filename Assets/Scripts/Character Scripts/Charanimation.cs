using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charanimation : MonoBehaviour
{
    public bool isGrounded;

    Animator animator;

    [SerializeField] Transform groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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


        //Go righttttt, play run anim
        if ((Input.GetKey("right")) && isGrounded)
        {
            animator.SetBool("Run", true);
            transform.localScale = new Vector3(1, 1, 1);
        }
        if ((Input.GetKeyUp("right")))
            animator.SetBool("Run", false);


        //Go left, play run anim flipped
        if ((Input.GetKey("left")) && isGrounded)
        {
            animator.SetBool("Run", true);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if ((Input.GetKeyUp("left")))
        {
            animator.SetBool("Run", false);
        }

        if ((Input.GetKey("d")) && isGrounded)
        {
            animator.SetBool("Ground Melee", true);
        }
        else animator.SetBool("Ground Melee", false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, groundCheck.position);
    }
}