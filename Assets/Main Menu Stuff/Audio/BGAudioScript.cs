using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGAudioScript : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audiosource;

    public AudioMixerGroup masterMixer;
    public float initialMasterMixVol;
    bool initialMasterMixVolSet;

    public int lowPassFreq; //The frequency that mixer will fade to, to low pass everything

    private void Start()
    {
        masterMixer.audioMixer.GetFloat("MasterVol", out initialMasterMixVol);
        //audiosource.Play();
    }

    public void StopAudio()
    {
        audiosource.Stop();
    }

    public void PlayAudioClip(int clipnumber)
    {
        audiosource.Stop();
        audiosource.clip = audioClips[clipnumber];
        audiosource.Play();
        //Debug.Log($"Playing {audioClips[clipnumber].name}");
    }

    public void RestartCurrentAudio()
    {
        audiosource.Play();
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

    public void FadeMixerLowPass(AudioMixer audioMixer, string exposedParam, float duration)
    {
        StartCoroutine(FadeAudioMixerLowPassFreq(audioMixer, exposedParam, duration));
    }

    public IEnumerator FadeAudioMixerLowPassFreq(AudioMixer audioMixer, string exposedParam, float duration)
    {

        float currentTime = 0;
        float currentFreq;
        audioMixer.GetFloat(exposedParam, out currentFreq);
        float targetValue = lowPassFreq;
        while (currentTime < duration)
        {
            Debug.Log("Fading low pass");
            currentTime += Time.deltaTime;
            float newFreq = Mathf.Lerp(currentFreq, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, newFreq);
            yield return null;
        }
        yield break;
    }

    public void RemoveMixerLowPass(AudioMixer audioMixer)
    {
        //Debug.Log("Resetting low pass");
        StopAllCoroutines();
        audioMixer.SetFloat("MasterLowPassCutoffFreq", 22000f);  //The max cutoff freq, effectively bypassing it
    }

    public void SetMixerVol(AudioMixer audioMixer, float targetVolume)
    {
        audioMixer.SetFloat("MasterVol", targetVolume);
    }

    public void ResetMasterVolToInitial()
    {
        if (!initialMasterMixVolSet)
        {
            masterMixer.audioMixer.GetFloat("MasterVol", out initialMasterMixVol);
            initialMasterMixVolSet = true;
        }
        masterMixer.audioMixer.SetFloat("MasterVol", initialMasterMixVol);
    }
}
