using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbyplayer : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float playerDir = 0;
    public float xForce = 0;
    public float yForce = 0;
    public AudioClip hitGround = null;
    AudioSource audiosource;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponentInParent<Rigidbody2D>();
        audiosource = GetComponentInParent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = GameObject.FindWithTag("Player").GetComponent<Char_control>().facingDir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PlaySound();
            
        }
        if (collision.transform.tag == ("hitbox"))
        {
            rb2d.AddForce(new Vector2(xForce * playerDir, yForce * playerDir));
        }
    }

    private void PlaySound()
    {
        audiosource.pitch = (Random.Range(0.8f, 1f));
        audiosource.PlayOneShot(hitGround);
    }
}
