using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charaudio : MonoBehaviour
{
    //Check some Animation States for sound initialization
    [Header("Object audio")]
    [SerializeField] private AudioClip Footstep;
    public float footstepVolume = 1;
    [SerializeField] private AudioClip Swing;
    public float swingVolume = 1;
    [SerializeField] private AudioClip Jump;
    public float jumpVolume = 1;
    [SerializeField] private AudioClip Coin_Pickup;
    public float coinPickupVolume = 1;

    [Header("Voice Lines")]
    [SerializeField] private AudioClip[] voice_jump;
    public float voiceJumpVolume = 1;
    [SerializeField] private AudioClip[] voice_swing;
    public float voiceSwingVolume = 1;
    [SerializeField] private AudioClip[] voice_get_hit;
    public float voiceGetHitVolume = 1;

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
        audiosource.volume = footstepVolume;
        audiosource.pitch = (Random.Range(0.5f, 1f));
        audiosource.PlayOneShot(Footstep);
    }

    public void AudOnSwing()
    {
        audiosource.volume = swingVolume;
        audiosource.pitch = (Random.Range(0.5f, 1f));
        audiosource.PlayOneShot(Swing);
    }

    public void AudOnJump()
    {
        audiosource.volume = jumpVolume;
        audiosource.pitch = 1;
        audiosource.PlayOneShot(Jump);
        Jumped = true;
    }

    public void AudVoiceonJump()
    {
        audiosource.volume = voiceJumpVolume;
        audiosource.pitch = 1;
        audiosource.clip = voice_jump[Random.Range(0, voice_jump.Length)];
        audiosource.Play();
    }

    public void AudVoiceonSwing()
    {
        audiosource.volume = voiceSwingVolume;
        audiosource.pitch = 1;
        audiosource.clip = voice_swing[Random.Range(0, voice_swing.Length)];
        audiosource.Play();
    }

    public void AudVoiceonGetHit()
    {
        audiosource.volume = voiceGetHitVolume;
        audiosource.pitch = 1;
        audiosource.clip = voice_get_hit[Random.Range(0, voice_get_hit.Length)];
        audiosource.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin_collectable"))
        {
            audiosource.volume = coinPickupVolume;
            audiosource.pitch = (Random.Range(0.9f, 1f));
            audiosource.PlayOneShot(Coin_Pickup);
        }

        if (collision.gameObject.CompareTag("enemy_attackhitbox"))
        {
            audiosource.volume = voiceGetHitVolume;
            audiosource.pitch = 1;
            audiosource.clip = voice_get_hit[Random.Range(0, voice_get_hit.Length)];
            audiosource.Play();
        }
    }
}
