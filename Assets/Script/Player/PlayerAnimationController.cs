using UnityEngine;

/// <summary>
/// Controla las animaciones del astronauta en función del estado de movimiento.
/// Requiere un Animator con los parámetros: Speed (float), IsGrounded (bool), VerticalVelocity (float).
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    // ─── Parámetros del Animator (hashed para mejor performance) ───────────
    private static readonly int ParamSpeed           = Animator.StringToHash("Speed");
    private static readonly int ParamIsGrounded      = Animator.StringToHash("IsGrounded");
    private static readonly int ParamVerticalVelocity = Animator.StringToHash("VerticalVelocity");

    // ─── Referencias ────────────────────────────────────────────────────────
    [Header("Referencias")]
    [SerializeField] private PlayerMovement2D playerMovement;
    [SerializeField] private PlayerGroundCheck groundCheck;

    private Animator animator;
    private Rigidbody2D rb;

    // ─── Umbral de velocidad horizontal para activar Run ────────────────────
    [Header("Configuración")]
    [Tooltip("Velocidad mínima horizontal para pasar a estado Run")]
    [SerializeField] private float runThreshold = 0.05f;

    // ────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb       = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateAnimatorParameters();
    }

    /// <summary>
    /// Actualiza los parámetros del Animator cada frame.
    /// </summary>
    private void UpdateAnimatorParameters()
    {
        // Velocidad horizontal absoluta → parámetro Speed
        float horizontalSpeed = playerMovement != null
            ? Mathf.Abs(playerMovement.HorizontalInput)
            : 0f;

        // Velocidad vertical del Rigidbody → parámetro VerticalVelocity
        float verticalVelocity = rb != null ? rb.linearVelocity.y : 0f;

        // ¿Está en el suelo?
        bool isGrounded = groundCheck != null && groundCheck.IsGrounded;

        // Enviar parámetros al Animator
        animator.SetFloat(ParamSpeed, horizontalSpeed);
        animator.SetBool(ParamIsGrounded, isGrounded);
        animator.SetFloat(ParamVerticalVelocity, verticalVelocity);
    }

    // ─── Helpers públicos (para uso futuro desde otros sistemas) ────────────

    /// <summary>Dispara un trigger puntual en el Animator (ej. ataque, daño).</summary>
    public void TriggerAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    /// <summary>Reinicia todos los triggers activos del Animator.</summary>
    public void ResetTriggers(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }
}
