using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_collider : MonoBehaviour
{
    Rigidbody2D parentrb2d;
    public bool stunnedcheck;

    // Start is called before the first frame update
    void Start()
    {
        parentrb2d = GetComponentInParent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("hitbox"))
        {
            stunnedcheck = true;       
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("hitbox"))
        {
            stunnedcheck = false;
        }
    }

}
