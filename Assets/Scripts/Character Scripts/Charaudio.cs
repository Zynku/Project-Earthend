using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charaudio : MonoBehaviour
{
    //Check some Animation States for sound initialization
    [Header("Object audio")]
    [SerializeField] private AudioClip Footstep;
    [SerializeField] private AudioClip Footstep2;
    [SerializeField] private AudioClip Swing;
    [SerializeField] private AudioClip Jump;
    [SerializeField] private AudioClip Coin_Pickup;

    [Header("Voice Lines")]
    [SerializeField] private AudioClip[] voice_jump;
    [SerializeField] private AudioClip[] voice_swing;
    [SerializeField] private AudioClip[] voice_get_hit;

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

    public void AudOnFootStep()
    {
        audiosource.pitch = (Random.Range(0.5f, 1f));
        audiosource.PlayOneShot(Footstep);
    }

    public void AudOnSwing()
    {
        audiosource.pitch = (Random.Range(0.5f, 1f));
        audiosource.PlayOneShot(Swing);
    }

    public void AudOnJump()
    {
        audiosource.pitch = 1;
        audiosource.PlayOneShot(Jump);
        Jumped = true;
    }

    public void AudVoiceonJump()
    {
        audiosource.pitch = 1;
        audiosource.clip = voice_jump[Random.Range(0, voice_jump.Length)];
        audiosource.Play();
    }

    public void AudVoiceonSwing()
    {
        audiosource.pitch = 1;
        audiosource.clip = voice_swing[Random.Range(0, voice_swing.Length)];
        audiosource.Play();
    }

    public void AudVoiceonGetHit()
    {
        audiosource.pitch = 1;
        audiosource.clip = voice_get_hit[Random.Range(0, voice_get_hit.Length)];
        audiosource.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("coin_collectable"))
        {
            audiosource.pitch = (Random.Range(0.9f, 1f));
            audiosource.PlayOneShot(Coin_Pickup);
        }
    }
}
