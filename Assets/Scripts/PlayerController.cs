using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float groundSpeed = 80.0f;
    public float airSpeed = 3.0f;
    public float maxSpeed = 5.0f;

    private GameManager gameManager;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isCrouching = false;

    private float movement = 0.0f;

    private float jumpForce = 9.0f;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager._player = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        GroundCheck();

        if (isGrounded) rb.AddForce(Vector3.right * groundSpeed * movement);
        else rb.AddForce(Vector3.right * airSpeed * movement);
        ApplyDrag();

        if (isJumping && isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isJumping = false;
        }
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
    }

    void ApplyDrag()
    {
        if (!isJumping && isGrounded && Mathf.Abs(movement) <= 0.01f) rb.drag = 15f;
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
