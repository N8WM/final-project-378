using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float groundSpeed = 80.0f;
    public float airSpeed = 3.0f;
    public float maxSpeed = 5.0f;

    private bool DO_ANIMATION = false;

    private GameManager gameManager;
    private Rigidbody2D rb;
    private Animator animator;
    private ParticleSystem dust;

    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isAccelerating = false;  // Just used for dust effect

    private float movement = 0.0f;
    private float jumpForce = 9.0f;
    private float timeSinceLastJump = -1f;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager._player = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dust = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        GroundCheck();

        if (Mathf.Abs(movement) > 0.01f) {
            if (!isAccelerating && isGrounded) {
                isAccelerating = true;
                CreateDust();
            }
        }
        else isAccelerating = false;

        if (isGrounded) rb.AddForce(Vector3.right * groundSpeed * movement);
        else rb.AddForce(Vector3.right * airSpeed * movement);
        ApplyDrag();

        if (isJumping && isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isJumping = false;
            timeSinceLastJump = 0f;
            CreateDust();
        }

        if (timeSinceLastJump >= 0 && Mathf.Abs(movement) > 0.01f) {
            rb.AddForce(Vector3.right * movement * jumpForce * 0.5f, ForceMode2D.Impulse);
            timeSinceLastJump = -1f;
        }

        if (timeSinceLastJump >= 0f && timeSinceLastJump <= 0.1f)
            timeSinceLastJump += Time.deltaTime;
        else
            timeSinceLastJump = -1f;

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);

        UpdateAnimationSeq();
    }

    void ApplyDrag()
    {
        if (!isJumping && isGrounded && Mathf.Abs(movement) <= 0.01f) rb.drag = 25f;
        else rb.drag = 0.3f;
    }

    void CreateDust()
    {
        dust.Play();
    }

    void UpdateAnimationSeq()
    {
        /* Animations:
            * 1. Idle
            * 2. Run
            * 3. Jump
            * 4. Crouch
            * 5. Fall
            * 6. Death
         * Transitions:
            * Idle -> Run
            * Idle -> Jump
            * Idle -> Crouch
            * Run -> Idle
            * Run -> Jump
            * Run -> Crouch
            * Jump -> Fall
            * Fall -> Idle
            * Fall -> Run
            * Crouch -> Idle
            * Crouch -> Run
            * Any -> Death
         * Priority:
            1. Death
            2. Fall
            3. Crouch (will not occur if player is not grounded)
            4. Jump
            5. Run
            6. Idle
        */
        if (!DO_ANIMATION) return;
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("VerticalSpeed", rb.velocity.y);
    }

    void OnMove(InputValue movementValue)
    {
        movement = movementValue.Get<float>();
    }

    void OnJump(InputValue jumpValue)
    {
        if (jumpValue.isPressed && isGrounded) {
            isJumping = true;
            isCrouching = false;
        }
        else isJumping = false;
    }

    void OnCrouch(InputValue crouchValue)
    {
        if (crouchValue.isPressed && isGrounded) isCrouching = true;
        else isCrouching = false;
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(
            new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2f),
            new Vector2(transform.localScale.x - 0.06f, 0.1f),
            0f,
            LayerMask.GetMask("Ground")
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2f),
            new Vector2(transform.localScale.x - 0.06f, 0.1f)
        );
    }
}
