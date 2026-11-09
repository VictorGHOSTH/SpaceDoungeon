
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.8f;

    [Header("Dash Restrictions")]
    [SerializeField] private bool canDashInAir = false;

    [Header("References")]
    [SerializeField] private PlayerMovement2D playerMovement;
    [SerializeField] private PlayerGroundCheck groundCheck;
    [SerializeField] private PlayerCollision2D collisionCheck;

    private Rigidbody2D rb;

    private float dashTimer;
    private float cooldownTimer;
    private float originalGravity;

    private int dashDirection;

    public bool IsDashing { get; private set; }
    public bool CanDash => cooldownTimer <= 0f && !IsDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        originalGravity = rb.gravityScale;
    }

    private void Update()
    {
        UpdateCooldown();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }
    }

    private void FixedUpdate()
    {
        if (!IsDashing)
            return;

        PerformDash();
    }

    private void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void TryDash()
    {
        if (!CanDash)
            return;

        if (!canDashInAir &&
            (groundCheck == null || !groundCheck.IsGrounded))
        {
            return;
        }

        StartDash();
    }

    private void StartDash()
    {
        IsDashing = true;

        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        // Obtener la direcci�n en la que mira el personaje.
        dashDirection = playerMovement != null
            ? playerMovement.FacingDirection
            : (transform.localScale.x >= 0 ? 1 : -1);

        // Desactivar la gravedad temporalmente.
        rb.gravityScale = 0f;

        // Eliminar velocidad vertical durante el Dash.
        rb.linearVelocity = new Vector2(
            dashDirection * dashSpeed,
            0f
        );
    }

    private void PerformDash()
    {
        // Detener el Dash si encuentra una pared.
        if (collisionCheck != null)
        {
            bool hitWall = dashDirection > 0
                ? collisionCheck.IsTouchingWallRight
                : collisionCheck.IsTouchingWallLeft;

            if (hitWall)
            {
                FinishDash();
                return;
            }
        }

        rb.linearVelocity = new Vector2(
            dashDirection * dashSpeed,
            0f
        );

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
        {
            FinishDash();
        }
    }

    private void FinishDash()
    {
        IsDashing = false;

        // Restaurar la gravedad original.
        rb.gravityScale = originalGravity;

        // Evitar conservar el impulso horizontal del Dash.
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }
}
