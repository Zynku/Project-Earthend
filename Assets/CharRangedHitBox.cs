using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class CharRangedHitBox : MonoBehaviour
{
    GameObject Player;
    Charattacks charattacks;
    Charanimation charanimation;

    private void Start()
    {
        Player = GetComponentInParent<Charcontrol>().gameObject;
        charattacks = Player.GetComponent<Charattacks>();
        charanimation = Player.GetComponent<Charanimation>();
    }

    public void OnTriggerEnter2D(Collider2D collision)  //Please put this somewhere that makes sense
    {
        charattacks.HitEnemy(collision.gameObject, Attack.AttackType.RANGED);    //Gets the first attack in the attack list to determine what type of attack it is
    }
}