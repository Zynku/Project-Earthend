using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charmovement_2 : MonoBehaviour
{
    private Rigidbody2D rb2d;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float maxMoveSpeed;
    private float HorizontalDirection;
    public bool isGrounded;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        isGrounded = GetComponent<Charjump>().isGrounded;
    }

    private void Update()
    {
        HorizontalDirection = GetInput().x;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    public Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }


    private void MoveCharacter()
    {
        rb2d.AddForce(new Vector2(HorizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
    }
}