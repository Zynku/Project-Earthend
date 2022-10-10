using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devlab_stand_in : MonoBehaviour
{
    Animator animator;
    Enemymain enemymainscript;
    Collider2D hitbox;
    SpriteRenderer squareSprite;

    CurrentColor currentColor;

    [Foldout("Part Colors", true)]
    public GameObject sideLightOff, sideLightGreen, sideLightRed, sideLightBroken;
    public GameObject faceColorOff, faceColorGreen, faceColorRed, faceColorBroken;

    [Foldout("State and Completion Variables", true)]
    [Tooltip("How much damage until it's completed?")]
    public int requiredDamage;
    [Tooltip("How much damage have I taken so far?")]
    public int damageSustained;         
    [Tooltip("Has the stand appeared or disappeared yet?")]
    public bool appeared, disappeared;  
    [Tooltip("Has it taken enough damage to be completed")]
    public bool completed;              
    [Tooltip("Does the stand reset itself after is has been completed?")]
    public bool resetOnCompleted;       

    public enum CurrentColor
    {
        Off,
        Green,
        Red,
        Broken
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemymainscript = GetComponent<Enemymain>();
        hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false;
        squareSprite = GetComponent<SpriteRenderer>();
        squareSprite.enabled = false;  
        enemymainscript.enemyBeenHit += BeenHit;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemymainscript.playerInsideRadius) { ActivateStand(); }
        //else if (appeared && !enemymainscript.playerInsideRadius) { DeactivateStand(); }
    }

    public void UpdateColors(string color)
    {
        //sideLightOff.SetActive(false);
        Debug.Log($"Side light off is {sideLightOff.activeSelf}");
        sideLightGreen.SetActive(false);
        sideLightRed.SetActive(false);
        sideLightBroken.SetActive(false);
        //faceColorOff.SetActive(false);
        Debug.Log($"Face Color off is {faceColorOff.activeSelf}");
        faceColorGreen.SetActive(false);
        faceColorRed.SetActive(false);
        faceColorBroken.SetActive(false);

        string colorToUse = color.ToLower();
        switch (colorToUse)
        {
            case "off":
                Debug.LogWarning("Setting face and light off to active does not work for some reason. Please enabled and disable via animations");
                break;
            case "green":
                sideLightGreen.SetActive(true);
                faceColorGreen.SetActive(true);
                currentColor = CurrentColor.Green;
                break;
            case "red":
                sideLightRed.SetActive(true);
                faceColorRed.SetActive(true);
                currentColor = CurrentColor.Red;
                break;
            case "broken":
                sideLightBroken.SetActive(true);
                faceColorBroken.SetActive(true);
                currentColor = CurrentColor.Broken;
                break;
            default:
                break;
        }
    }

    public void BeenHit()
    {
        damageSustained = enemymainscript.maxHealth - enemymainscript.currentHealth;
        if (damageSustained >= requiredDamage) { Debug.Log("Enough damage sustained"); CompleteStand(); }

        if (!completed)
        {
            if (enemymainscript.collisionDir == -1) { animator.Play("Stand-in Hit from Left Large"); }  //Been hit from left
            if (enemymainscript.collisionDir == 1) { animator.Play("Stand-in Hit from Right Large"); }  //Been hit from right
        }   
    }

    public void CompleteStand()
    {
        completed = true;
        DeactivateStand();
    }

    public void ActivateStand()
    {
        if (!appeared && !completed)
        { 
            animator.SetTrigger("Appear");
            appeared = true;
            disappeared = false;
            enemymainscript.ResetHealth();
        }
    }

    public void ActivateHitBox()
    {
        hitbox.enabled = true;
    }


    public void DeactivateStand()
    {
        if (!disappeared)
        {
            animator.SetTrigger("Disappear");
            disappeared = true;
            appeared=false;
            DeactivateHitbox();
            if (resetOnCompleted) { ActivateStand(); appeared = false; completed = false; }
        }
    }

    public void DeactivateHitbox()
    {
        hitbox.enabled = false;
    }
}
