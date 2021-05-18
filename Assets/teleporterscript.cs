using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporterscript : MonoBehaviour
{
    public GameObject[] teleporters;
    public GameObject teleportTo = null;
    public GameObject Player;
    public float teleportRange;
    private float teleportcooldown;
    private float teleportcooldowntargettime = 1;
    private bool canTeleport;
    public bool DebugDistance;

    [Range(0f, 1f)]
    public float teleportingVolume = 1;
    public AudioClip teleporting;

    Animator animator;
    Animator animatorTo;

    AudioSource audiosource;
    AudioSource audiosourceTo;

    // Start is called before the first frame update
    void Start()
    {
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");

        animator = GetComponent<Animator>();
        animatorTo = teleportTo.GetComponent<Animator>();
        
        audiosource = GetComponent<AudioSource>();
        audiosourceTo = teleportTo.GetComponent<AudioSource>();

        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        teleportcooldown -= Time.deltaTime;
        if (teleportcooldown < 0) { teleportcooldown = 0; }
        if (teleportcooldown == 0 && (Vector3.Distance(Player.transform.position, transform.position) < teleportRange)) 
        {
            canTeleport = true;
            Debug.Log("In range");
        }
        else
        {
            canTeleport = false;
        }

        if (canTeleport)
        {
            if (Input.GetButtonDown("Interact"))
            {
                canTeleport = false;
                Teleport();
            }
        }
        if (DebugDistance) { Debug.Log(Vector3.Distance(Player.transform.position, transform.position)); }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /*if (collision.CompareTag("Player"))
        {
            if (Input.GetButtonDown("Interact") && canTeleport)
            {
                collision.transform.position = teleportTo.transform.position + new Vector3(0, 0.5f, 0);
                collision.GetComponent<Rigidbody2D>().velocity = new Vector2 (0,0);
                animator.SetTrigger("Teleport");
                animatorTo.SetTrigger("Teleport");
                teleportcooldown = teleportcooldowntargettime;
                teleportTo.GetComponent<teleporterscript>().teleportcooldown = teleportcooldown;
                canTeleport = false;

                audiosource.volume = teleportingVolume;
                audiosource.PlayOneShot(teleporting);
            }
        }*/
    }

    public void Teleport()
    {
        Player.transform.position = teleportTo.transform.position + new Vector3(0, 0.5f, 0);
        Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        animator.SetTrigger("Teleport");
        animatorTo.SetTrigger("Teleport");
        teleportcooldown = teleportcooldowntargettime;
        teleportTo.GetComponent<teleporterscript>().teleportcooldown = teleportcooldown;

        audiosource.volume = teleportingVolume;
        audiosource.PlayOneShot(teleporting);
        audiosourceTo.volume = teleportingVolume;
        audiosource.PlayOneShot(teleporting);

        Debug.Log(teleportTo);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
