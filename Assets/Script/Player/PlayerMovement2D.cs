
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;

    [Header("References")]
    [SerializeField] private PlayerGroundCheck groundCheck;
    [SerializeField] private PlayerCollision2D collisionCheck;
    [SerializeField] private PlayerDash playerDash;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    private float horizontalInput;
    private bool jumpRequested;

    public float HorizontalInput => horizontalInput;

    public int FacingDirection { get; private set; } = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ReadInput();
        UpdateDirection();
    }

    private void FixedUpdate()
    {
        if (playerDash != null && playerDash.IsDashing)
            return;

        Move();
        Jump();
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
        }
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void Jump()
    {
        if (!jumpRequested)
            return;

        if (groundCheck != null && groundCheck.IsGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        jumpRequested = false;
    }

    private void UpdateDirection()
    {
        if (horizontalInput > 0)
        {
            FacingDirection = 1;
        }
        else if (horizontalInput < 0)
        {
            FacingDirection = -1;
        }

        if (spriteRenderer != null && horizontalInput != 0)
        {
            spriteRenderer.flipX = FacingDirection < 0;
        }
    }
}
