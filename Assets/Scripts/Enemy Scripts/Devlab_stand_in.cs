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

    Standstate standState;
    CurrentColor currentColor;

    [Foldout("Part Colors", true)]
    public GameObject sideLightOff, sideLightGreen, sideLightRed, sideLightBroken;
    public GameObject faceColorOff, faceColorGreen, faceColorRed, faceColorBroken;

    public bool appeared, disappeared;  //Has the stand appeared or disappeared yet?


    public enum Standstate
    {
        Appearing,
        Idle,
        Disappearing
    }

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
        Enemymain.EnemyBeenHit += BeenHit;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemymainscript.playerInsideRadius) 
        {
            ActivateStand(); 
        }
        else if (appeared && enemymainscript.playerInsideRadius)
        {
            DeactivateStand();
        }
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
                //For some reason this doesn't work and I can't figure it out. Enabled and disable as needed from animations.
                //sideLightOff.SetActive(true);
                //Debug.Log($"Side light off is {sideLightOff.activeSelf}");
                //faceColorOff.SetActive(true);
                //Debug.Log($"Face Color off is {faceColorOff.activeSelf}");
                Debug.LogWarning("Setting face and light off to active does not work for some reason. Please enabled and disable via animations");
                break;
            case "green":
                sideLightGreen.SetActive(true);
                Debug.Log($"Side light green is {sideLightGreen.activeSelf}");
                faceColorGreen.SetActive(true);
                Debug.Log($"Face Color green is {faceColorGreen.activeSelf}");
                break;
            case "red":
                sideLightRed.SetActive(true);
                Debug.Log($"Side light red is {sideLightRed.activeSelf}");
                faceColorRed.SetActive(true);
                Debug.Log($"Face Color red is {faceColorRed.activeSelf}");
                break;
            case "broken":
                sideLightBroken.SetActive(true);
                faceColorBroken.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void BeenHit()
    {
        Debug.Log("Ouch!");
    }

    public void ActivateStand()
    {
        if (!appeared)
        { 
            animator.SetTrigger("Appear");
            appeared = true;
            disappeared = false;
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
        }
    }

    public void DeactivateHitbox()
    {
        hitbox.enabled = false;
    }
}
