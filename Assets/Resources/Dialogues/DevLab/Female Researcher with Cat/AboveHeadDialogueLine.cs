using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
[CreateAssetMenu(fileName = "New Above Head Dialogue", menuName = "Dialogue/New Above Head Dialogue")]
public class AboveHeadDialogueLine : ScriptableObject
{
    public int ahID;                     //What ID# is this Above Head Dialogue?
    [TextArea(2, 10)]
    public string lineString;               //Actual string of words to be said
    [Header("Default Variables")]
    public string lineOwner;                //Who said the line?
    public int typeSpeed = 0;               //How fast is the text said?
    public List<AudioClip> audio;           //List of Audio that says the line
    [Range(0f, 1f)]
    public float audioVol = 1;
}
