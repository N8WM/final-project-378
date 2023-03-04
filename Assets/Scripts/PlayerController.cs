using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance;

    public float groundSpeed = 80.0f;
    public float airSpeed = 3.0f;
    public float maxSpeed = 5.0f;
    public float maxSpeedCrouching = 1.5f;
    public float jumpForce = 10.0f;

    private bool DO_ANIMATION = true;

    private Rigidbody2D rb;
    private Animator animator;
    private ParticleSystem dust;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isAccelerating = false;  // Just used for dust effect
    private bool isPouncing = false;  // Just used for turning around

    private float movement = 0.0f;
    private float timeSinceLastJump = -1f;

    private float DELTA = 0.01f;

    void Awake()
    {
        if (_instance != this && _instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dust = GetComponentInChildren<ParticleSystem>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
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
            isPouncing = true;
        } else isPouncing = false;

        if (timeSinceLastJump >= 0f && timeSinceLastJump <= 0.1f)
            timeSinceLastJump += Time.deltaTime;
        else
            timeSinceLastJump = -1f;

        float tmp_maxSpeed = (isCrouching && isGrounded) ? maxSpeedCrouching : maxSpeed;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -tmp_maxSpeed, tmp_maxSpeed), rb.velocity.y);

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

        bool walking = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_Walking";
        bool idle = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_Idle";
        bool turning = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_TurningAround";
        bool landing = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_Landing";

        if (!DO_ANIMATION) return;

        if (turning)
        {
            sr.flipX = !sr.flipX;
            animator.SetBool("IsTurningAround", false);
        }

        if (((movement < -DELTA && !sr.flipX) || (movement > DELTA && sr.flipX)) && !animator.GetBool("IsTurningAround"))
        {
            if (isPouncing) {
                sr.flipX = !sr.flipX;
                isPouncing = false;
            }
            else if (walking || idle || landing)
                animator.SetBool("IsTurningAround", true);
        }
        
        animator.SetFloat("SpeedX", rb.velocity.x);
        animator.SetFloat("SpeedY", rb.velocity.y);
        animator.SetBool("IsStopped", Mathf.Abs(rb.velocity.x) < maxSpeedCrouching - DELTA);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsMoving", Mathf.Abs(movement) > DELTA);
        
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
        if (crouchValue.isPressed) isCrouching = true;
        else isCrouching = false;
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(
            new Vector2(transform.position.x + bc.offset.x, transform.position.y + bc.offset.y - bc.size.y / 2f),
            new Vector2(transform.localScale.x * bc.size.x - 0.06f, 0.1f),
            0f,
            LayerMask.GetMask("Ground")
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(
            new Vector2(transform.position.x + bc.offset.x, transform.position.y + bc.offset.y - bc.size.y / 2f),
            new Vector2(transform.localScale.x * bc.size.x - 0.06f, 0.1f)
        );
    }
}
