using UnityEngine;

/// <summary>
/// LevelManager — componente por escena.
/// Configura el nivel al inicio: posición de spawn, límites de cámara,
/// y referencia al jugador activo. Vive en cada escena de nivel.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // ─── Información del nivel ───────────────────────────────────────────────
    [Header("Información del nivel")]
    [SerializeField] private int    levelIndex  = 1;
    [SerializeField] private string levelName   = "Nivel 1 — Planeta Alienígena";

    // ─── Spawn del jugador ───────────────────────────────────────────────────
    [Header("Spawn del jugador")]
    [Tooltip("Transform de spawn inicial del jugador. Si es null, usa posición (0,0,0).")]
    [SerializeField] private Transform playerSpawnPoint;

    [Tooltip("Prefab del jugador. Si ya existe en la escena, dejar en null.")]
    [SerializeField] private GameObject playerPrefab;

    // ─── Límites del nivel ───────────────────────────────────────────────────
    [Header("Límites de la cámara")]
    [SerializeField] private bool  applyBoundsToCamera = true;
    [SerializeField] private float cameraMinX = -20f;
    [SerializeField] private float cameraMaxX =  80f;
    [SerializeField] private float cameraMinY = -10f;
    [SerializeField] private float cameraMaxY =  10f;

    // ─── Referencias ────────────────────────────────────────────────────────
    [Header("Referencias de escena")]
    [SerializeField] private CameraController cameraController;

    // ─── Referencia al jugador instanciado/encontrado ────────────────────────
    public GameObject Player { get; private set; }

    // ────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        InitializeLevel();
    }

    // ────────────────────────────────────────────────────────────────────────

    private void InitializeLevel()
    {
        Debug.Log($"[LevelManager] Iniciando: {levelName}");

        SpawnOrFindPlayer();
        ConfigureCamera();
    }

    /// <summary>
    /// Si hay un prefab asignado, instancia el jugador en el spawn point.
    /// Si no, busca el jugador existente en la escena.
    /// </summary>
    private void SpawnOrFindPlayer()
    {
        Vector3 spawnPosition = playerSpawnPoint != null
            ? playerSpawnPoint.position
            : Vector3.zero;

        if (playerPrefab != null)
        {
            Player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"[LevelManager] Jugador instanciado en {spawnPosition}.");
        }
        else
        {
            // Buscar jugador ya existente en la escena por tag
            Player = GameObject.FindGameObjectWithTag("Player");

            if (Player != null)
            {
                Player.transform.position = spawnPosition;
                Debug.Log($"[LevelManager] Jugador encontrado y reposicionado en {spawnPosition}.");
            }
            else
            {
                Debug.LogWarning("[LevelManager] No se encontró jugador. Asigna 'playerPrefab' o coloca un GameObject con tag 'Player' en la escena.");
            }
        }
    }

    /// <summary>
    /// Configura la cámara: asigna el objetivo y aplica los límites del nivel.
    /// </summary>
    private void ConfigureCamera()
    {
        if (cameraController == null)
        {
            // Buscar la cámara en la escena si no está asignada
            cameraController = Camera.main?.GetComponent<CameraController>();
        }

        if (cameraController == null)
        {
            Debug.LogWarning("[LevelManager] No se encontró CameraController en la cámara principal.");
            return;
        }

        // Asignar jugador como objetivo de la cámara
        if (Player != null)
        {
            cameraController.SetTarget(Player.transform);
        }

        // Aplicar límites del nivel a la cámara
        if (applyBoundsToCamera)
        {
            cameraController.SetBounds(cameraMinX, cameraMaxX, cameraMinY, cameraMaxY);
        }
    }

    // ─── Acceso público ──────────────────────────────────────────────────────

    public int    GetLevelIndex() => levelIndex;
    public string GetLevelName()  => levelName;

    // ─── Gizmos del editor ───────────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        // Dibuja el punto de spawn
        if (playerSpawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(playerSpawnPoint.position, 0.3f);
            Gizmos.DrawLine(
                playerSpawnPoint.position,
                playerSpawnPoint.position + Vector3.up * 1f
            );
        }

        // Dibuja los límites del nivel
        if (applyBoundsToCamera)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
            Vector3 center = new Vector3(
                (cameraMinX + cameraMaxX) / 2f,
                (cameraMinY + cameraMaxY) / 2f,
                0f
            );
            Vector3 size = new Vector3(
                cameraMaxX - cameraMinX,
                cameraMaxY - cameraMinY,
                0f
            );
            Gizmos.DrawCube(center, size);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
