using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance;
    public bool inputDisabled;
    public HesedLevelManager hesedManager;

    public float groundSpeed = 80.0f;
    public float airSpeed = 6.0f;
    public float maxSpeed = 5.0f;
    public float maxSpeedCrouching = 1.5f;
    public float jumpForce = 10.0f;

    private bool DO_ANIMATION = true;

    private Rigidbody2D rb;
    private Animator animator;
    private ParticleSystem dust;
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private DoorTarget targetDoor;

    private Vector2 defaultColliderOffset;
    private Vector2 defaultColliderSize;

    private bool isGrounded = false;
    private bool isTouchingCeiling = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isAccelerating = false;  // Just used for dust effect
    private bool isPouncing = false;  // Just used for turning around
    private bool isTryingToCrouch = false; // Just used for automatically undoing crouch
    private bool isTouchingWall = false; // Prevents clipping under walls when turning around
    private bool alreadyFlipped = false;

    private float movement = 0.0f;
    private float timeSinceLastJump = -1f;
    private float colliderStartOffsetX;

    private float DELTA = 0.01f;
    public bool isMoving { get { return Mathf.Abs(movement) > DELTA; } }

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

        defaultColliderOffset = bc.offset;
        defaultColliderSize = bc.size;
        colliderStartOffsetX = bc.offset.x;

        if (GameManager._instance.spawnAtDoor != null) {
            DoorController[] dctrls = GameObject.FindObjectsOfType<DoorController>();
            foreach (DoorController dctrl in dctrls) {
                if (dctrl.doorTarget == GameManager._instance.spawnAtDoor) {
                    transform.position = dctrl.transform.position;
                    break;
                }
            }
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
        CeilingCheck();
        PerformCrouch(isTryingToCrouch); // automatocally undoes crouching when not under ceiling
        WallCheck();

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
        if (!isJumping && isGrounded && Mathf.Abs(movement) <= 0.01f && rb.velocity.y < DELTA) rb.drag = 100f;
        else rb.drag = 0.3f;
    }

    void CreateDust()
    {
        dust.Play();
    }

    void UpdateAnimationSeq()
    {
        bool walking, idle, turning, landing;
        if (animator.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            walking = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_Walking";
            idle = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_Idle";
            turning = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_TurningAround";
            landing = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Cat_Landing";
        }
        else
            walking = idle = turning = landing = false;
        

        if (!DO_ANIMATION) return;

        if (turning && !alreadyFlipped)
        {
            sr.flipX = !sr.flipX;
            alreadyFlipped = true;
            animator.SetBool("IsTurningAround", false);
        }

        if (((movement < -DELTA && !sr.flipX) || (movement > DELTA && sr.flipX)) && !animator.GetBool("IsTurningAround"))
        {
            if (isPouncing) {
                sr.flipX = !sr.flipX;
                isPouncing = false;
            }
            else if ((walking || idle || landing) && !isTouchingWall)
                animator.SetBool("IsTurningAround", true);
            alreadyFlipped = false;
        }
        
        animator.SetFloat("SpeedX", rb.velocity.x);
        animator.SetFloat("SpeedY", rb.velocity.y);
        animator.SetBool("IsStopped", Mathf.Abs(rb.velocity.x) < DELTA);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsTouchingCeiling", isTouchingCeiling);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsMoving", Mathf.Abs(movement) > DELTA);
        
        if (sr.flipX) bc.offset = new Vector2(-colliderStartOffsetX, bc.offset.y);
        else bc.offset = new Vector2(colliderStartOffsetX, bc.offset.y);
    }

    void OnMove(InputValue movementValue)
    {
        if (!inputDisabled)
            movement = movementValue.Get<float>();
        else if (hesedManager != null)
            hesedManager.Move(movementValue);
    }

    void OnSelect(InputValue selectValue)
    {
        if (inputDisabled && hesedManager != null)
            hesedManager.Select(selectValue);
    }

    void OnJump(InputValue jumpValue)
    {
        if (!inputDisabled)
        {
            if (jumpValue.isPressed && isGrounded && !isTouchingCeiling) {
                isJumping = true;
                isCrouching = false;
            }
            else isJumping = false;
        }
    }

    void OnCrouch(InputValue crouchValue)
    {
        if (!inputDisabled)
        {
            if (targetDoor != null &&
                crouchValue.isPressed &&
                !isMoving && (
                    targetDoor.unlockedInLevel ||
                    !targetDoor.locked
                )
            ) {
                EnterDoor();
                return;
            }
            isTryingToCrouch = crouchValue.isPressed;
        }
        
    }

    void OnPause(InputValue pauseValue)
    {
        if (pauseValue.isPressed)
            if (GameManager._instance.isPaused)
                GameManager._instance.Resume();
            else
                GameManager._instance.Pause();
    }

    void PerformCrouch(bool shouldCrouch)
    {
        if (shouldCrouch)
        {
            isCrouching = true;
            bc.offset = new Vector2(bc.offset.x, -0.2f);
            bc.size = new Vector2(bc.size.x, 0.53f - bc.edgeRadius * 2f);
        }
        else
        { 
            if (isCrouching && !isTouchingCeiling)
            {
                isCrouching = false;
                bc.offset = defaultColliderOffset;
                bc.size = defaultColliderSize;
            }
        }
    }

    void EnterDoor()
    {
        foreach (DoorTarget door in GameManager._instance.winUnlocks)
            door.locked = false;
        if (GameManager._instance.currentLevelDoor != null)
            GameManager._instance.currentLevelDoor.levelCompleted = true;
        Time.timeScale = 0f;
        GameManager._instance.FadeToLevel(targetDoor.doorScene, targetDoor.doorScene.Equals("Scenes/Menu"));
        if (targetDoor.doorTitle.Equals("Reset"))
            GameManager._instance.ResetLevels();
        targetDoor = null;
    }

    public void OnDeath()
    {
        animator.SetTrigger("HasDied");
    }

    public void SetTargetDoor(DoorTarget door)
    {
        targetDoor = door;
    }

    void GroundCheck()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(
            new Vector2(transform.position.x + bc.offset.x, transform.position.y + bc.offset.y - (bc.size.y + bc.edgeRadius * 2f) / 2f),
            new Vector2(transform.localScale.x * (bc.size.x + bc.edgeRadius * 2f) - 0.06f, 0.05f),
            0f,
            LayerMask.GetMask("Ground") | LayerMask.GetMask("Movable")
        );
        foreach (Collider2D col in cols) {
            if (col != null && !col.isTrigger) {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    void CeilingCheck()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(
            new Vector2(transform.position.x + bc.offset.x, 0.2f + transform.position.y + bc.offset.y + (bc.size.y + bc.edgeRadius * 2f) / 2f),
            new Vector2(transform.localScale.x * (bc.size.x + bc.edgeRadius * 2f) - 0.06f, 0.1f),
            0f,
            LayerMask.GetMask("Ground")
        );
        foreach (Collider2D col in cols) {
            if (col != null && !col.isTrigger) {
                isTouchingCeiling = true;
                return;
            }
        }
        isTouchingCeiling = false;
    }

    void WallCheck()
    {
        Collider2D[] cols = Physics2D.OverlapBoxAll(
            new Vector2(transform.position.x + bc.offset.x - ((sr.flipX ? -1f : 1f) * (bc.size.x + bc.edgeRadius * 2f) * 1.3f), transform.position.y + bc.offset.y),
            new Vector2(0.1f, transform.localScale.y * (bc.size.y + bc.edgeRadius * 2f) - 0.06f),
            0f,
            LayerMask.GetMask("Ground")
        );
        foreach (Collider2D col in cols) {
            if (col != null && !col.isTrigger) {
                isTouchingWall = true;
                return;
            }
        }
        isTouchingWall = false;
    }

    public int GetDirection()
    {
        return sr.flipX ? -1 : 1;
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube( // Ground
            new Vector2(transform.position.x + bc.offset.x, transform.position.y + bc.offset.y - (bc.size.y + bc.edgeRadius * 2f) / 2f),
            new Vector2(transform.localScale.x * (bc.size.x + bc.edgeRadius * 2f) - 0.06f, 0.05f)
        );
        Gizmos.DrawWireCube( // Ceiling
            new Vector2(transform.position.x + bc.offset.x, 0.2f + transform.position.y + bc.offset.y + (bc.size.y + bc.edgeRadius * 2f) / 2f),
            new Vector2(transform.localScale.x * (bc.size.x + bc.edgeRadius * 2f) - 0.06f, 0.1f)
        );
        Gizmos.DrawWireCube( // Wall
            new Vector2(transform.position.x + bc.offset.x - ((bc.size.y + bc.edgeRadius * 2f) * 1.3f), transform.position.y + bc.offset.y),
            new Vector2(0.1f, transform.localScale.y * (bc.size.y + bc.edgeRadius * 2f) - 0.06f)
        );
    }
}
