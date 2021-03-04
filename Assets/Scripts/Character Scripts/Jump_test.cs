using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_test : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField] private float jumpForce;

    [Header("Jump Variables")]
    [SerializeField] private LayerMask groundLayer;
    private bool canjump => Input.GetKeyDown(KeyCode.UpArrow) && onGround;

    [Header("Ground Collision Variables")]
    [SerializeField] private float groundraycastlength;
    [SerializeField] private bool onGround;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckCollisions();
        if (canjump) Jump();
    }

    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckCollisions()
    {
        onGround = Physics2D.Raycast(transform.position * groundraycastlength, Vector2.down, groundraycastlength, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundraycastlength);

    }
}
