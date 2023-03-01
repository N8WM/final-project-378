using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float groundSpeed = 80.0f;
    public float airSpeed = 10.0f;
    public float maxSpeed = 5.0f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isCrouching = false;

    private float movement = 0.0f;

    private float jumpForce = 6.0f;
    private float gravityModifier = 1.8f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics.gravity *= gravityModifier;
    }

    void FixedUpdate()
    {
        GroundCheck();
        print(isGrounded);

        float forceFactor = (Mathf.Sign(rb.velocity.x) == Mathf.Sign(movement)) ? 1.0f : 2.0f;
        if (isGrounded) rb.AddForce(Vector3.right * groundSpeed * movement * forceFactor);
        else rb.AddForce(Vector3.right * airSpeed * movement * forceFactor);
        if (isJumping && isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isJumping = false;
        }
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);

        ApplyDrag();
    }

    void ApplyDrag()
    {
        if (isGrounded && Mathf.Abs(movement) <= 0.01f) rb.drag = 15f;
        else rb.drag = 0.3f;
    }

    void OnMove(InputValue movementValue)
    {
        movement = movementValue.Get<float>();
    }

    void OnJump(InputValue jumpValue)
    {
        if (jumpValue.isPressed && isGrounded) isJumping = true;
        else isJumping = false;
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(
            new Vector2(transform.position.x, transform.position.y - 0.5f),
            new Vector2(transform.localScale.x - 0.05f, 0.1f),
            0f,
            LayerMask.GetMask("Ground")
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            new Vector2(transform.position.x, transform.position.y - 0.5f),
            new Vector2(transform.localScale.x - 0.05f, 0.1f)
        );
    }
}
