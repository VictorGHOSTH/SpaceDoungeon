using UnityEngine;

/// <summary>
/// Seguimiento suave de cámara 2D para Space Dungeon.
/// Implementación propia compatible con Unity 6 / URP 2D (sin Cinemachine).
/// Configuración completa desde el Inspector.
/// </summary>
public class CameraController : MonoBehaviour
{
    // ─── Objetivo ───────────────────────────────────────────────────────────
    [Header("Objetivo")]
    [Tooltip("Transform del jugador que seguirá la cámara")]
    [SerializeField] private Transform target;

    // ─── Seguimiento ────────────────────────────────────────────────────────
    [Header("Seguimiento")]
    [Tooltip("Suavizado del seguimiento (menor = más suave, mayor = más rígido)")]
    [SerializeField] [Range(1f, 20f)] private float smoothSpeed = 6f;

    [Tooltip("Desplazamiento en X/Y respecto al jugador (para anticipar el movimiento)")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);

    // ─── Límites del nivel ───────────────────────────────────────────────────
    [Header("Límites del nivel")]
    [Tooltip("Activar límites de cámara para el nivel")]
    [SerializeField] private bool useBounds = false;

    [SerializeField] private float minX = -100f;
    [SerializeField] private float maxX =  100f;
    [SerializeField] private float minY = -100f;
    [SerializeField] private float maxY =  100f;

    // ─── Estado interno ─────────────────────────────────────────────────────
    private Vector3 velocity = Vector3.zero;   // usado por SmoothDamp
    private float   zPosition;                 // la cámara no se mueve en Z

    // ────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        zPosition = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        FollowTarget();
    }

    /// <summary>
    /// Calcula la posición deseada y mueve la cámara suavemente hacia ella.
    /// </summary>
    private void FollowTarget()
    {
        // Posición objetivo: jugador + offset
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            zPosition
        );

        // Aplicar límites si están activados
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Suavizado con SmoothDamp para evitar vibraciones
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            1f / smoothSpeed
        );
    }

    /// <summary>
    /// Reposiciona la cámara instantáneamente sobre el objetivo (uso en spawn/reload).
    /// </summary>
    public void SnapToTarget()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            zPosition
        );

        velocity = Vector3.zero;
    }

    /// <summary>
    /// Establece un nuevo objetivo en tiempo de ejecución.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SnapToTarget();
    }

    /// <summary>
    /// Configura los límites del nivel desde LevelManager u otros sistemas.
    /// </summary>
    public void SetBounds(float minXVal, float maxXVal, float minYVal, float maxYVal)
    {
        minX = minXVal;
        maxX = maxXVal;
        minY = minYVal;
        maxY = maxYVal;
        useBounds = true;
    }

    // ─── Gizmos en el editor ─────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size   = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
