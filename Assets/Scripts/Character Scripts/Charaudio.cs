using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Charaudio : MonoBehaviour
{
    Charanimation charanimation;
    //Check some Animation States for sound initialization
    [Header("Object audio")]
    [SerializeField] private AudioClip Footstep;
    [Range(0f, 1f)]
    public float footstepVolume = 1;

    [SerializeField] private AudioClip Swing;
    [Range(0f, 1f)]
    public float swingVolume = 1;

    [SerializeField] private AudioClip Jump;
    [Range(0f, 1f)]
    public float jumpVolume = 1;

    [SerializeField] private AudioClip coin_Pickup;
    [Range(0f, 1f)]
    public float coinPickupVolume = 1;

    [SerializeField] private AudioClip[] hit_Something;
    [Range(0f, 1f)]
    public float hitSomethingVolume = 1;



    [Header("Voice Lines")]
    [SerializeField] private AudioClip[] voice_jump;
    [Range(0f, 1f)]
    public float voiceJumpVolume = 1;

    [SerializeField] private AudioClip[] voice_swing;
    [Range(0f, 1f)]
    public float voiceSwingVolume = 1;

    [SerializeField] private AudioClip[] voiceBeenHit;
    [Range(0f, 1f)] public float voiceBeenHitVol = 1;

    [HideInInspector] public bool Jumped;
    private AudioSource audiosource;


    private void Start()
    {
        audiosource = GetComponent<AudioSource>();
        charanimation = GetComponent<Charanimation>();

        Enemyhealth.EnemyBeenHit += HitSomething;
        Charhealth.Hit += BeenHit;
    }

    private void Update()
    {

    }

    public void AudOnFootStep()
    {
        if (Footstep != null)
        {
            audiosource.volume = footstepVolume;
            audiosource.pitch = (Random.Range(0.5f, 1f));
            audiosource.PlayOneShot(Footstep);
        }
    }

    public void AudOnSwing()
    {
        if (Swing != null)
        {
            audiosource.volume = swingVolume;
            audiosource.pitch = (Random.Range(0.5f, 1f));
            audiosource.PlayOneShot(Swing);
        }
    }

    public void AudOnJump()
    {
        if (Jump != null)
        {
            audiosource.volume = jumpVolume;
            audiosource.pitch = 1;
            audiosource.PlayOneShot(Jump);
        }
        Jumped = true;
    }


    public void AudVoiceonJump()
    {
        if (voice_jump.Length > 0)
        {
            audiosource.volume = voiceJumpVolume;
            audiosource.pitch = 1;
            audiosource.clip = voice_jump[Random.Range(0, voice_jump.Length)];
            audiosource.Play();
        }
    }


    public void AudVoiceonSwing()
    {
        if (voice_swing != null)
        {
            audiosource.volume = voiceSwingVolume;
            audiosource.pitch = 1;
            //audiosource.clip = voice_swing[Random.Range(0, voice_swing.Length)];
            //audiosource.Play();
            audiosource.PlayOneShot(voice_swing[Random.Range(0, voice_swing.Length)]);
        }
    }


    public void AudMeleeSwing()
    {
        Combo currentCombo = charanimation.currentCombo[0];

        audiosource.pitch = 1;
        audiosource.volume = currentCombo.attackSwingSoundVol;
        audiosource.clip = currentCombo.attackSwingSound[Random.Range(0, currentCombo.attackSwingSound.Length)];
        audiosource.Play();
    }

    public void HitSomething()
    {
        if (hit_Something != null)
        {
            audiosource.volume = hitSomethingVolume;
            audiosource.pitch = 1;
            audiosource.PlayOneShot(hit_Something[Random.Range(0, hit_Something.Length - 1)]);
        }
    }

    public void BeenHit()
    {
        audiosource.volume = voiceBeenHitVol;
        audiosource.pitch = 1;
        audiosource.PlayOneShot(voiceBeenHit?[Random.Range(0, voiceBeenHit.Length - 1)]);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("coin_collectable"))
        {
            if (collision.gameObject.CompareTag("coin_collectable"))
            {
                if (coin_Pickup != null)
                {
                    audiosource.volume = coinPickupVolume;
                    audiosource.pitch = (Random.Range(0.9f, 1f));
                    audiosource.PlayOneShot(coin_Pickup);
                }
            }
        }
        if (collision.gameObject.CompareTag("enemy_attackhitbox"))
        {
            BeenHit();
        }
    }
}








