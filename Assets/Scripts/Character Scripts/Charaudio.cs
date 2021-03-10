﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charaudio : MonoBehaviour
{
    //Check some Animation States for sound initialization
    [SerializeField] private AudioClip Footstep;
    [SerializeField] private AudioClip Footstep2;
    [SerializeField] private AudioClip Swing;
    [SerializeField] private AudioClip Jump;
    [SerializeField] private AudioClip Hit;

    public bool Jumped;

    private AudioSource audiosource;
    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFootStep()
    {
        audiosource.pitch = (Random.Range(0.5f, 1f));
        audiosource.PlayOneShot(Footstep);
    }

    public void OnSwing()
    {
        audiosource.pitch = (Random.Range(0.9f, 1f));
        audiosource.PlayOneShot(Swing);
    }

    public void OnJump()
    {
        audiosource.pitch = 1f;
        audiosource.PlayOneShot(Jump);
        Jumped = true;
    }

    public void OnHit()
    {
        
    }
}
