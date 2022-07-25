using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class CharMeleeHitBox : MonoBehaviour
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

    public void OnTriggerEnter2D(Collider2D collision)  //Passes information to charattacks since that script should have all the logic relating the enemy collisions
    {
        charattacks.HitEnemy(collision.gameObject, charanimation.currentCombo[0].attackList[0].attackType);    //Gets the first attack in the attack list to determine what type of attack it is
    }
}
