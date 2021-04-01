using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporterscript : MonoBehaviour
{
    public GameObject[] teleporters;
    public GameObject teleportTo = null;
    private float teleportcooldown;
    private float teleportcooldowntargettime = 1;
    Animator animator;
    Animator animatorTo;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animatorTo = teleportTo.GetComponent<Animator>();
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");
    }

    // Update is called once per frame
    void Update()
    {
        teleportcooldown -= Time.deltaTime;
        if (teleportcooldown < 0) { teleportcooldown = 0; }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.S) && teleportcooldown == 0)
            {
                collision.transform.position = teleportTo.transform.position + new Vector3(0, 0.5f, 0);
                collision.GetComponent<Rigidbody2D>().velocity = new Vector2 (0,0);
                animator.SetTrigger("Teleport");
                animatorTo.SetTrigger("Teleport");
                teleportcooldown = teleportcooldowntargettime;
            }
        }
    }
}
