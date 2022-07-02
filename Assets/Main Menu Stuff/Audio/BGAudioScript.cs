using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAudioScript : MonoBehaviour
{
    public AudioClip[] audioClips;
    public static BGAudioScript Instance;
    public AudioSource audiosource;

    private void Start()
    {
        //audiosource = GetComponent<AudioSource>();
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
        Debug.Log($"Playing {audioClips[clipnumber].name}");
    }
}
