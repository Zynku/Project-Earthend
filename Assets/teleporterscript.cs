using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporterscript : MonoBehaviour
{
    public GameObject[] teleporters;
    public GameObject teleportTo = null;
    public GameObject Player;
    public AudioClip teleport;
    public bool teleportable;
    //private float teleportcooldown;
    //private float teleportcooldowntargettime = 1;
    Animator animator;
    Animator animatorTo;
    AudioSource audiosource;
    AudioSource audiosourceTo;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        animatorTo = teleportTo.GetComponent<Animator>();
        audiosourceTo = teleportTo.GetComponent<AudioSource>();
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");
    }

    // Update is called once per frame
    void Update()
    {
        //teleportcooldown -= Time.deltaTime;
        //if (teleportcooldown < 0) { teleportcooldown = 0; }

        if (teleportable == true && (Player.GetComponentInParent<Charinputcontrol>().interactDown))
        {
            Teleport();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            teleportable = true;
            Player = collision.gameObject;
            /*&& teleportcooldown == 0*/
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            teleportable = false;
            Player = null;
        }
    }

    private void Teleport()
    {
        Player.transform.position = teleportTo.transform.position + new Vector3(0, 0.5f, 0);
        Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        animator.SetTrigger("Teleport");
        animatorTo.SetTrigger("Teleport");
        //teleportcooldown = teleportcooldowntargettime;

        audiosource.PlayOneShot(teleport);
        audiosourceTo.PlayOneShot(teleport);
    }
}
