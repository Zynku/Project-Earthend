using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainingball_animator : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.K))
        {
            animator.SetBool("BeenHit", true);
        }
        else
        {
            animator.SetBool("BeenHit", false);
        }
    }
}
