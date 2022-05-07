using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class Combo
{
    public string comboName = "null";
    public string moveName;
    public AudioClip[] attackSwingSound;    //An array of sounds that can be chosen from to play during this combo
    [Range(0f, 1f)]
    public float attackSwingSoundVol = 1;   
    public string animationName;
    public float comboChainTimeLocation;    //The time at which, if there is another combo to perform, it will cut this animation at to play the next. Compared to animationInfo.normalizedTime in Charanimation
    public bool canChangeState;
    [ConditionalField(nameof(canChangeState))] public float stateChangeTimeLocation;   //The time after which, if this combo requires a state change (check Charattacks) it will allow state changes. Compared to animationInfo.normalizedTime in Charanimation
    [ConditionalField(nameof(canChangeState))] public Charcontrol.State stateChange;   //The state to change to after stateChangeTime;
    public bool endOfComboChain;
    public List<Attack> attackList; //Contains the list of attacks required to execute this combo
}
