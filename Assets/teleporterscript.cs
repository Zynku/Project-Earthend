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
    public float timerTime;
    public float timerTargetTime;
    public bool canActivateTPMenu = true;

    [Range(0f, 1f)]
    public float teleportingVolume = 1;
    public AudioClip teleporting;

    Animator animator;
    AudioSource audiosource;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        Network = GetComponentInParent<teleporternetwork>();
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        timerTime = timerTargetTime;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector3.Distance(Player.transform.position, transform.position) < teleportRange))
        {
            if (Input.GetButtonDown("Interact") && canActivateTPMenu)
            {
                Network.showNetworkUI();
                Network.activatedAt = gameObject;
            }
        }

        
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
        
        rechargeBar.transform.localScale = new Vector2((timerTime / timerTargetTime) * 0.5f, transform.localScale.y);
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

    public void PlaySoundandAnimation()
    {
        animator.SetTrigger("Teleport");
        audiosource.volume = teleportingVolume;
        audiosource.PlayOneShot(teleporting);
    }
}
