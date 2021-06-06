using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hidden_button : MonoBehaviour
{
    public float timesClicked;
    public float[] timesClickedForEvent;
    public GameObject[] EasterEggObject;

    [SerializeField] private AudioClip soundEffect;
    [Range(0f, 1f)]
    public float soundEffectVol = 1;

    AudioSource audiosource;

    private void Start()
    {
        timesClicked = 0;
        if (GetComponent<AudioSource>() != null)
        {
            audiosource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (timesClickedForEvent.Length >= 1)
        {
            if (timesClicked >= timesClickedForEvent[0])
            {
                if (EasterEggObject[0] != null)
                {
                    EasterEggObject[0].SetActive(true);

                    if (EasterEggObject[0].GetComponent<Animator>() != null)
                    {
                        Animator animator;
                        animator = EasterEggObject[0].GetComponent<Animator>();
                        animator.SetBool("Activate", true);
                    }
                }
            }
        }

        if (timesClickedForEvent.Length >= 2)
        {
            if (timesClicked >= timesClickedForEvent[1])
            {
                if (EasterEggObject[1] != null) { EasterEggObject[1].SetActive(true); }
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < EasterEggObject.Length; i++)
        {
            EasterEggObject[i].SetActive(false);
        }
        timesClicked = 0;
    }

    public void IncrementCount()
    {
        timesClicked++;
    }

    public void PlaySoundEffect()
    {
        if (soundEffect != null)
        {
            audiosource.volume = soundEffectVol;
            audiosource.pitch = 1;
            audiosource.PlayOneShot(soundEffect);
        }
    }

}
