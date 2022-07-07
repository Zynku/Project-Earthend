using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGAudioScript : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audiosource;

    public AudioMixerGroup masterMixer;
    public float masterMixerVol;

    private void Start()
    {
        masterMixer.audioMixer.GetFloat("MasterVol", out masterMixerVol);
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

    public IEnumerator FadeAudioVolume(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audiosource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audiosource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public IEnumerator FadeAudioMixer(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }

    public void SetMixerVol(AudioMixer audioMixer, float targetVolume)
    {
        audioMixer.SetFloat("MasterVol", targetVolume);
    }

    public void ResetMasterVolToInitial()
    {
        masterMixer.audioMixer.SetFloat("MasterVol", masterMixerVol);
    }
}
