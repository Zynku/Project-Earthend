using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainingball_controller : MonoBehaviour
{
    private Animator animator;
    private AudioSource audiosource;
    public int currentHealth;
    public int maxHealth;
    [SerializeField] AudioClip Hit;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hitboxes"))
        {
            animator.SetBool("BeenHit", true);
            audiosource.PlayOneShot(Hit);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
            animator.SetBool("BeenHit", false);
    }
}
