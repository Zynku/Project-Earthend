using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devlab_stand_in : MonoBehaviour
{
    [Foldout("Audio", true)]
    [Range(0f, 1f)]
    public float appearAlertVolume = 1;
    public AudioClip appearAlertAud;
    [Range(0f, 1f)]
    public float appearVolume = 1;
    public AudioClip appearAud;
    [Range(0f, 1f)]
    public float brokenVolume = 1;
    public AudioClip brokenAud;
    [Range(0f, 1f)]
    public float completedVolume = 1;
    public AudioClip completedAud;
    AudioSource audioSource;
    Animator animator;
    Enemymain enemymainscript;
    Collider2D hitbox;
    SpriteRenderer squareSprite;

    [Foldout("Part Colors", true)]
    public GameObject sideLightOff, sideLightGreen, sideLightRed, sideLightBroken;
    public GameObject faceColorOff, faceColorGreen, faceColorRed, faceColorBroken;
    public GameObject top;
    public Sprite topNormal, topBroken;
    public string currentColor;

    [Foldout("State and Completion Variables", true)]
    [Tooltip("Will the stand spawn by the player coming close or by an encounter?")]
    public bool spawnViaDistance;
    [Tooltip("How much damage until it's completed?")]
    public int requiredDamage;
    [Tooltip("How much damage have I taken so far?")]
    [ReadOnly] public int damageSustained;
    [Tooltip("Percentage damage of requiredDamage necessary for a big hit")]
    public float percentDmgForBigHit;
    [Tooltip("Has the stand appeared or disappeared yet?")]
    public bool appeared, disappeared;
    [Tooltip("Has it taken enough damage to be completed")]
    public bool completed;
    private bool finishedCompletion;
    [Tooltip("Does the stand reset itself after is has been completed?")]
    public bool resetOnCompleted;
    [Tooltip("How long after deactivating should I reset, provided resetOnCompleted is true")]
    public float resetTime;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        enemymainscript = GetComponent<Enemymain>();
        hitbox = GetComponent<Collider2D>();
        hitbox.enabled = false;
        squareSprite = GetComponent<SpriteRenderer>();
        squareSprite.enabled = false;
        enemymainscript.enemyBeenHit += BeenHit;
        enemymainscript.spawnOrActivate += ActivateStand;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemymainscript.playerInsideRadius)
        {
            if (!spawnViaDistance) { return; }  //Do nothing if not in range
            if (completed) { return; }          //Do nothing if enemy has been defeated before and doesn't reset
            ActivateStand();
        }
    }

    public void UpdateColors(string color)
    {
        string colorToUse = color.ToLower();
        switch (colorToUse)
        {
            case "off":
                if (currentColor != "off")
                {
                    sideLightGreen.SetActive(false);
                    faceColorGreen.SetActive(false);
                    sideLightRed.SetActive(false);
                    faceColorRed.SetActive(false);
                    sideLightBroken.SetActive(false);
                    faceColorBroken.SetActive(false);

                    sideLightOff.SetActive(true);
                    faceColorOff.SetActive(true);
                    currentColor = "off";
                }
                break;
            case "green":
                if (currentColor != "green")
                {
                    sideLightOff.SetActive(false);
                    faceColorOff.SetActive(false);
                    sideLightRed.SetActive(false);
                    faceColorRed.SetActive(false);
                    sideLightBroken.SetActive(false);
                    faceColorBroken.SetActive(false);

                    sideLightGreen.SetActive(true);
                    faceColorGreen.SetActive(true);
                    currentColor = "green";
                }
                break;
            case "red":
                if (currentColor != "red")
                {
                    sideLightOff.SetActive(false);
                    faceColorOff.SetActive(false);
                    sideLightGreen.SetActive(false);
                    faceColorGreen.SetActive(false);
                    sideLightBroken.SetActive(false);
                    faceColorBroken.SetActive(false);

                    sideLightRed.SetActive(true);
                    faceColorRed.SetActive(true);
                    currentColor = "red";
                }
                break;
            case "broken":
                if (currentColor != "broken")
                {
                    sideLightOff.SetActive(false);
                    faceColorOff.SetActive(false);
                    sideLightGreen.SetActive(false);
                    faceColorGreen.SetActive(false);
                    sideLightRed.SetActive(false);
                    faceColorRed.SetActive(false);

                    sideLightBroken.SetActive(true);
                    faceColorBroken.SetActive(true);
                    currentColor = "broken";
                }
                break;
            default:
                break;
        }
    }

    public void BeenHit()
    {
        damageSustained = enemymainscript.maxHealth - enemymainscript.currentHealth;

        if (!completed)
        {
            float damageForBigHit = requiredDamage * (percentDmgForBigHit / 100);

            if (enemymainscript.damageDoneToMe > requiredDamage)    //If damage is taken larger than the required damage...
            {
                UpdateColors("broken");
                top.GetComponent<SpriteRenderer>().sprite = topBroken;
                completed = true;
                if (enemymainscript.collisionDir == -1) { animator.Play("Stand-in Hit from Left Broken"); }  //Been hit from left
                if (enemymainscript.collisionDir == 1) { animator.Play("Stand-in Hit from Right Broken"); }  //Been hit from right
                if (damageSustained >= requiredDamage && !finishedCompletion) { CompleteStand(); }
                return;
            }
            else if (enemymainscript.damageDoneToMe > damageForBigHit)  //Else if the damage is larger than the damage for big hit threshold
            {
                if (enemymainscript.collisionDir == -1) { animator.Play("Stand-in Hit from Left Large"); }  //Been hit from left
                if (enemymainscript.collisionDir == 1) { animator.Play("Stand-in Hit from Right Large"); }  //Been hit from right
            }
            else
            {
                if (enemymainscript.collisionDir == -1) { animator.Play("Stand-in Hit from Left Small"); }  //Been hit from left
                if (enemymainscript.collisionDir == 1) { animator.Play("Stand-in Hit from Right Small"); }  //Been hit from right
            }
            if (damageSustained >= requiredDamage && !finishedCompletion) { CompleteStand(); }
        }
    }

    public void CompleteStand()
    {
        enemymainscript.DefeatEnemy();
        completed = true;
        if (!disappeared)
        {
            disappeared = true;
            animator.SetTrigger("Disappear");
            appeared = false;
            hitbox.enabled = false;
            if (resetOnCompleted) { Invoke(nameof(ResetStand), resetTime); }
        }
        finishedCompletion = true;
    }

    public void ActivateStand()
    {
        if (!appeared)
        {
            animator.SetTrigger("Appear");
            top.GetComponent<SpriteRenderer>().sprite = topNormal;
            appeared = true;
            disappeared = false;
            completed = false;
            finishedCompletion = false;
            damageSustained = 0;
            enemymainscript.ResetHealth();
            Invoke(nameof(ActivateEnemy), 2f);
        }
    }

    public void ActivateEnemy()
    {
        enemymainscript.SetAsReady();
    }

    public void ActivateHitBox()
    {
        hitbox.enabled = true;
    }

    public void DeactivateStand()
    {
        if (!disappeared)
        {
            disappeared = true;
            animator.SetTrigger("Disappear");
            appeared = false;
            hitbox.enabled = false;
            if (resetOnCompleted) { Invoke(nameof(ResetStand), resetTime); }
        }
    }

    public void ResetStand()
    {
        ActivateStand();
        appeared = false;
        completed = false;
        enemymainscript.enemyDefeated = false;
        enemymainscript.damageDoneToMe = 0;
        finishedCompletion = false;
    }

    public void PlayActivateAlertSound()
    {
        if (appearAlertAud != null) PlaySoundFX(1, appearAlertAud);
    }

    public void PlayActivateSound()
    {
        if (appearAud != null) PlaySoundFX(1, appearAud);
    }

    public void PlaySoundFX(float volume, AudioClip clip)
    {
        audioSource.volume = volume;
        //audioSource.pitch = (Random.Range(0.5f, 1f));
        audioSource.PlayOneShot(clip);
    }
    public void DeactivateHitbox()
    {
        hitbox.enabled = false;
    }
}
