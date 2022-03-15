using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class global_script : MonoBehaviour
{
    public static global_script instance;


    [Header("Audio")]
    public AudioMixer masterMixer;
    public static float masterVolfloat;
    public static float soundVolfloat;
    public static float musicVolfloat;

    void Awake()
    {
        /*        //Ensures there's only ever one global script
                if (instance == null)
                {
                    instance = this;
                    //DontDestroyOnLoad(this);
                }
                else if (instance != null)
                {
                    Destroy(this);
                }*/
        instance = this;
    }

    private void Update()
    {
        //Gets volumes from volume mixer
        GetMasterVolume();
        GetMusicVolume();
        GetSoundVolume();
    }


    public float GetMasterVolume()
    {
        bool resultmaster = masterMixer.GetFloat("MasterVolume", out masterVolfloat);
        if (resultmaster)
        {
            return masterVolfloat;
        }
        else
        {
            return 0f;
        }
    }

    public float GetMusicVolume()
    {
        bool resultmusic = masterMixer.GetFloat("MusicVolume", out musicVolfloat);
        if (resultmusic)
        {
            return musicVolfloat;
        }
        else
        {
            return 0f;
        }
    }

    public float GetSoundVolume()
    {
        bool resultsound = masterMixer.GetFloat("SoundVolume", out soundVolfloat);
        if (resultsound)
        {
            return soundVolfloat;
        }
        else
        {
            return 0f;
        }
    }
}
