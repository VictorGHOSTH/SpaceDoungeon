using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager — singleton que persiste entre escenas.
/// Gestiona el estado global del juego: escena activa, nivel actual,
/// vida del jugador y eventos de alto nivel (Game Over, victoria).
///
/// Mantenerlo mínimo: solo los datos que necesiten persistir entre niveles.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ─── Singleton ───────────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ─── Estado del juego ────────────────────────────────────────────────────
    [Header("Estado del juego")]
    [SerializeField] private int playerMaxHealth = 3;

    public int PlayerCurrentHealth { get; private set; }
    public int CurrentLevelIndex   { get; private set; }

    // ─── Nombre de las escenas (configurar en Inspector o constantes) ────────
    [Header("Nombres de escenas")]
    [SerializeField] private string mainMenuScene  = "MainMenu";
    [SerializeField] private string level1Scene    = "Level1_AlienPlanet";
    [SerializeField] private string level2Scene    = "Level2_ExtraterrestrialBase";
    [SerializeField] private string level3Scene    = "Level3_AbandonedShip";

    // ────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        // Patrón Singleton con persistencia entre escenas
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    // ─── Inicialización ──────────────────────────────────────────────────────

    private void InitializeGame()
    {
        PlayerCurrentHealth = playerMaxHealth;
        CurrentLevelIndex   = 1;
    }

    // ─── Navegación entre niveles ────────────────────────────────────────────

    public void LoadLevel(int levelIndex)
    {
        CurrentLevelIndex = levelIndex;

        string sceneName = levelIndex switch
        {
            1 => level1Scene,
            2 => level2Scene,
            3 => level3Scene,
            _ => mainMenuScene
        };

        SceneManager.LoadScene(sceneName);
    }

    public void LoadNextLevel()
    {
        LoadLevel(CurrentLevelIndex + 1);
    }

    public void RestartCurrentLevel()
    {
        PlayerCurrentHealth = playerMaxHealth;
        LoadLevel(CurrentLevelIndex);
    }

    public void LoadMainMenu()
    {
        PlayerCurrentHealth = playerMaxHealth;
        CurrentLevelIndex   = 1;
        SceneManager.LoadScene(mainMenuScene);
    }

    // ─── Vida del jugador ────────────────────────────────────────────────────

    public void TakeDamage(int amount)
    {
        PlayerCurrentHealth = Mathf.Max(0, PlayerCurrentHealth - amount);

        if (PlayerCurrentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    public void RestoreHealth(int amount)
    {
        PlayerCurrentHealth = Mathf.Min(playerMaxHealth, PlayerCurrentHealth + amount);
    }

    // ─── Eventos de alto nivel ────────────────────────────────────────────────

    public void TriggerGameOver()
    {
        Debug.Log("[GameManager] Game Over.");
        // TODO (Módulo UI): mostrar pantalla de Game Over
        // Por ahora reinicia el nivel tras 2 segundos
        Invoke(nameof(RestartCurrentLevel), 2f);
    }

    public void TriggerLevelComplete()
    {
        Debug.Log($"[GameManager] Nivel {CurrentLevelIndex} completado.");
        // TODO (Módulo UI): mostrar pantalla de victoria del nivel
        Invoke(nameof(LoadNextLevel), 1.5f);
    }
}
