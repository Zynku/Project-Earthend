using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mainaudioscript : MonoBehaviour
{
    public AudioClip[] audioClips;
    public static Mainaudioscript Instance;
    AudioSource audiosource;


    void Awake()
    {
        //Ensures there's only ever one global script
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        audiosource = GetComponent<AudioSource>();


        audiosource.clip = audioClips[0];   //Plays the first audioclip on start
        audiosource.Play();
    }

    public void StopAudio()
    {
        audiosource.Stop();
    }

    public void PlayAudioClip(int clipnumber)
    {
        audiosource.clip = audioClips[clipnumber];
        audiosource.Play();
        Debug.Log("Playing clip number " + clipnumber);
    }
}
