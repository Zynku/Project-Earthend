using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class teleporterscript : MonoBehaviour
{
    public GameObject Player;
    public teleporternetwork Network;
    public float teleportRange;
    public bool DebugDistance;
    public GameObject rechargeBar;
    private float yScale;           //This is needed because sometimes Unity gets the scale wrong based on the teleporter's parent object. Idk man I wrote this at 3:17am
    public float timerTime;
    public float timerTargetTime;
    public bool canActivateTPMenu = true;

    [Range(0f, 1f)]
    public float teleportingVolume = 1;
    public AudioClip teleporting;

    [Range(0f, 1f)]
    public float menuOpenVolume = 1;
    public AudioClip[] menuOpen;

    Animator animator;
    AudioSource audiosource;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        Network = GetComponentInParent<teleporternetwork>();
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        timerTime = timerTargetTime;
        yScale = rechargeBar.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Network.enabled == false) {this.enabled = false;}

        timerTime += Time.deltaTime;
        if (timerTime > timerTargetTime)
        {
            timerTime = timerTargetTime;
            canActivateTPMenu = true;
        }
        else
        {
            canActivateTPMenu = false;
        }
        
        rechargeBar.transform.localScale = new Vector2((timerTime / timerTargetTime) * 0.5f, yScale);
        try
        {
            if ((Vector3.Distance(Player.transform.position, transform.position) < teleportRange))
            {
                if (Charinputs.instance.interact.WasPressedThisFrame() && canActivateTPMenu)
                {
                    if (!Network.isMenuActive)  //If the teleporter menu is not active, we show it
                    {
                        Network.showNetworkUI();
                        PlayMenuTyping();
                        Network.activatedAt = gameObject;
                    }
                    else        //If it is active, we hide it
                    {
                        Network.hideNetworkUI();
                    }

                }
            }
        }
        catch (MissingReferenceException) { }
    }

    public void ResetTPTimer()
    {
        timerTime = 0f;
        canActivateTPMenu = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }

    public void PlayMenuTyping()
    {
        AudioClip randomClip = menuOpen[Random.Range(0, menuOpen.Length -1)];
        audiosource.volume = menuOpenVolume;
        audiosource.PlayOneShot(randomClip);
    }

    public void PlaySoundandAnimation()
    {
        animator.SetTrigger("Teleport");
        audiosource.volume = teleportingVolume;
        audiosource.PlayOneShot(teleporting);
    }
}
