using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charmovement_2 : MonoBehaviour
{
    private Rigidbody2D rb2d;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration = 9.5f;
    [SerializeField] private float maxMoveSpeed = 1.6f;
    private float HorizontalDirection;
    public bool isGrounded;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        isGrounded = GetComponent<Charjump>().isGrounded;
    }
    // Get Horizontal Input from Method
    private void Update()
    {
        HorizontalDirection = GetInput().x;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }
    // Uses Unity's Input system to return input as a vector
    public Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //Adds a force equal to horizontaldirection variable * acceleration
    //If velocity is more than maxmovespeed, set speed to maxmovespeed
    private void MoveCharacter()
    {
        rb2d.AddForce(new Vector2(HorizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMoveSpeed, rb2d.velocity.y);
    }
}