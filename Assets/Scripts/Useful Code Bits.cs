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

    /*
public void BFS(string id, int orderNumber = 1) //Breadth first search, gives all paths an order so that we can follow them. Order dictates the order in which they can be done. Two quests with the same order means either can be done
{
    // No longer necessary since order is assigned in inspector now
    QuestEvent thisEvent = FindQuestEvent(id);
    thisEvent.order = orderNumber;

    foreach (QuestPath e in questPaths)
    {
        if (e.endEvent.order == -1)
        {
            BFS(e.endEvent.GetId(), orderNumber + 1);
        }
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



/*  Takes raw timer in seconds and converts it to 00d 00h 00s format and spits it out as shownTimer. Uses TextMeshPro (using TmPro;)
    private void FormatTimer()
    {
        int days = (int)(rawTimer / 86400) % 365;
        int hours = (int)(rawTimer / 3600) % 24;
        int minutes = (int)(rawTimer / 60) % 60;
        int seconds = (int)(rawTimer % 60);

        shownTimer.text = "";
        if (days > 0) { shownTimer.text += days + "d "; }
        if (hours > 0) { shownTimer.text += hours + "h "; }
        if (minutes > 0) { shownTimer.text += minutes + "m "; }
        if (seconds > 0) { shownTimer.text += seconds + "s "; }
    }*/
}
