using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbyplayer : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float playerDir = 0;
    public float xForce = 0;
    public float yForce = 0;
    public float torqueForce = 0;
    public AudioClip hitGround, hitByPlayer;
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
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            PlayGroundHit();
            
        }
        if (collision.transform.tag == ("player_attackhitbox"))
        {
            rb2d.AddForce(new Vector2(xForce * playerDir * 10, yForce * 10));
            rb2d.AddTorque(Random.Range(torqueForce, -torqueForce));
            playerDir = collision.GetComponentInParent<Char_control>().facingDir;
            PlayPlayerHit();
        }
    }

    private void PlayGroundHit()
    {
        //audiosource.pitch = (Random.Range(0.8f, 1f));
        //audiosource.PlayOneShot(hitGround);
    }

    private void PlayPlayerHit()
    {
        //audiosource.pitch = (Random.Range(0.8f, 1f));
        //audiosource.PlayOneShot(hitByPlayer);
    }
}
