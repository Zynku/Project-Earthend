using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulCodeBits : MonoBehaviour
{
    //public static myScript Instance;

    //public float timerTime;
    //public float timerTargetTime;


    /*
    [SerializeField] private AudioClip audioClipName;
    [Range(0f, 1f)]
    public float audioClipNameVol = 1;

    public void AudOnAudioClipName()
    {
        if (audioClipName != null)
        {
            audiosource.volume = audioClipVol;
            audiosource.pitch = 1;
            audiosource.PlayOneShot(audioClipName);
        }
    }

    */


    //Creates a new number every n seconds for variable i. Don't forget to call StartCoroutine(NewNumber())
    /*
    IEnumerator NewNumber()
    {
        randomint = UnityEngine.Random.Range(1, 9999);
        i = randomint.ToString();
        yield return new WaitForSeconds(n);
        StartCoroutine(NewNumber());
    }
    */



    /*#region Singleton
    private void Awake()
    {
    //Don't forget to create an Instance reference
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of myScript found!");
            return;
        }
        Instance = this;
    }
    #endregion
    */

    /*
    timerTime -= Time.deltaTime;
    if (timerTime < 0) { timerTime = 0; }
    */
}
