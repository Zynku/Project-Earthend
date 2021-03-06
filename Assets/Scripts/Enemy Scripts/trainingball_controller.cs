﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainingball_controller : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hitboxes"))
        {
            animator.SetBool("BeenHit", true);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
            animator.SetBool("BeenHit", false);
    }
}