using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charjump : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField] Transform groundCheck;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float groundLinearDrag;
    [SerializeField] private float fallJumpMultiplier;
    [SerializeField] private float lowJumpFallMultiplier;
    private float HorizontalDirection;
    private bool canjump => Input.GetKeyDown(KeyCode.UpArrow) && isGrounded;
    public bool isGrounded;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (canjump) Jump();
        {
            HorizontalDirection = GetInput().x;
        }

        //Creates Raycast between ground and groundcheck object
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        ApplyGroundLinearDrag();
        ApplyAirLinearDrag();
        FallMultiplier();

        if (isGrounded)
        {
            ApplyGroundLinearDrag();
        }
        else
        {
            ApplyAirLinearDrag();
        }
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void FallMultiplier()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.gravityScale = fallJumpMultiplier;
        }
        else if (rb2d.velocity.y > 0 && !Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb2d.gravityScale = lowJumpFallMultiplier;
        }
        else
        {
            rb2d.gravityScale = 1f;
        }
    }

    private void ApplyAirLinearDrag()
    {
            rb2d.drag = airLinearDrag;
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(HorizontalDirection) < 0.4 /*|| changingDirection*/)
        {
            rb2d.drag = groundLinearDrag;
        }
        else
        {
            rb2d.drag = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, groundCheck.position);
    }
}
