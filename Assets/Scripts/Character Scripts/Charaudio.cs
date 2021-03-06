using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charaudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

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
        AudioClip clip = GetRandomClip();
        audiosource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }
}
