#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor utility: crea y configura las escenas del Nivel 1 y Nivel 2
/// con la jerarquía básica necesaria para Space Dungeon.
///
/// Uso: menú superior de Unity → SpaceDungeon → Setup → Create Level Scenes
/// </summary>
public static class LevelSceneSetup
{
    [MenuItem("SpaceDungeon/Setup/Create Level 1 Scene")]
    public static void CreateLevel1()
    {
        CreateLevelScene(
            sceneName:    "Level1_AlienPlanet",
            scenePath:    "Assets/Scenes/Level1_AlienPlanet.unity",
            levelIndex:   1,
            levelName:    "Nivel 1 — Planeta Alienígena",
            spawnPos:     new Vector3(-8f, 1f, 0f),
            cameraMinX:   -12f,
            cameraMaxX:   80f,
            cameraMinY:   -8f,
            cameraMaxY:   10f,
            bgColor:      new Color(0.05f, 0.05f, 0.15f, 1f)   // azul oscuro espacial
        );
    }

    [MenuItem("SpaceDungeon/Setup/Create Level 2 Scene")]
    public static void CreateLevel2()
    {
        CreateLevelScene(
            sceneName:    "Level2_ExtraterrestrialBase",
            scenePath:    "Assets/Scenes/Level2_ExtraterrestrialBase.unity",
            levelIndex:   2,
            levelName:    "Nivel 2 — Base Extraterrestre",
            spawnPos:     new Vector3(-8f, 1f, 0f),
            cameraMinX:   -12f,
            cameraMaxX:   100f,
            cameraMinY:   -8f,
            cameraMaxY:   12f,
            bgColor:      new Color(0.03f, 0.08f, 0.05f, 1f)   // verde oscuro subterráneo
        );
    }

    // ─── Método central de creación ──────────────────────────────────────────
    private static void CreateLevelScene(
        string sceneName, string scenePath,
        int levelIndex, string levelName,
        Vector3 spawnPos,
        float cameraMinX, float cameraMaxX,
        float cameraMinY, float cameraMaxY,
        Color bgColor)
    {
        // 1. Crear escena nueva
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        newScene.name = sceneName;

        // ── Activar la escena nueva ──────────────────────────────────────────
        SceneManager.SetActiveScene(newScene);

        // ── 2. GameManager (si no existe ya) ────────────────────────────────
        if (GameManager.Instance == null)
        {
            var gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
            SceneManager.MoveGameObjectToScene(gm, newScene);
        }

        // ── 3. Main Camera ───────────────────────────────────────────────────
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = 5f;
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.backgroundColor  = bgColor;
        cam.farClipPlane     = 100f;
        camGO.AddComponent<AudioListener>();

        var cameraCtrl = camGO.AddComponent<CameraController>();
        camGO.transform.position = new Vector3(0f, 0f, -10f);
        SceneManager.MoveGameObjectToScene(camGO, newScene);

        // ── 4. Light 2D global ───────────────────────────────────────────────
        var lightGO = new GameObject("Global Light 2D");
        // Se añade el componente Light2D via reflection para no requerir using URP
        var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        if (lightType != null)
        {
            var light2D = lightGO.AddComponent(lightType);
            // LightType.Global = 4
            var lightTypeField = lightType.GetProperty("lightType");
            lightTypeField?.SetValue(light2D, 4);
            var intensityField = lightType.GetProperty("intensity");
            intensityField?.SetValue(light2D, 1f);
        }
        SceneManager.MoveGameObjectToScene(lightGO, newScene);

        // ── 5. Punto de Spawn ────────────────────────────────────────────────
        var spawnGO = new GameObject("PlayerSpawn");
        spawnGO.transform.position = spawnPos;
        SceneManager.MoveGameObjectToScene(spawnGO, newScene);

        // ── 6. Level Manager ─────────────────────────────────────────────────
        var levelMgrGO = new GameObject("LevelManager");
        var levelMgr   = levelMgrGO.AddComponent<LevelManager>();

        // Asignar campos vía SerializedObject para respetar la serialización de Unity
        var so = new SerializedObject(levelMgr);
        so.FindProperty("levelIndex")           .intValue  = levelIndex;
        so.FindProperty("levelName")            .stringValue = levelName;
        so.FindProperty("cameraMinX")           .floatValue  = cameraMinX;
        so.FindProperty("cameraMaxX")           .floatValue  = cameraMaxX;
        so.FindProperty("cameraMinY")           .floatValue  = cameraMinY;
        so.FindProperty("cameraMaxY")           .floatValue  = cameraMaxY;
        so.FindProperty("applyBoundsToCamera")  .boolValue   = true;
        so.FindProperty("playerSpawnPoint")     .objectReferenceValue = spawnGO.transform;
        so.FindProperty("cameraController")     .objectReferenceValue = cameraCtrl;
        so.ApplyModifiedPropertiesWithoutUndo();
        SceneManager.MoveGameObjectToScene(levelMgrGO, newScene);

        // ── 7. Contenedores organizativos vacíos ─────────────────────────────
        CreateEmptyContainer("--- Escenario ---",    newScene);
        CreateEmptyContainer("--- Plataformas ---",  newScene);
        CreateEmptyContainer("--- Enemigos ---",     newScene);
        CreateEmptyContainer("--- Coleccionables ---", newScene);

        // ── 8. Suelo base (BoxCollider2D) ─────────────────────────────────────
        var groundGO = new GameObject("Ground_Base");
        groundGO.layer = LayerMask.NameToLayer("Default");
        var groundCol = groundGO.AddComponent<BoxCollider2D>();
        groundCol.size   = new Vector2(200f, 1f);
        groundGO.transform.position = new Vector3(0f, -2f, 0f);
        SceneManager.MoveGameObjectToScene(groundGO, newScene);

        // ── 9. Guardar escena ────────────────────────────────────────────────
        EditorSceneManager.SaveScene(newScene, scenePath);
        AssetDatabase.Refresh();

        Debug.Log($"[SpaceDungeon] Escena '{sceneName}' creada en: {scenePath}");
    }

    // ─── Helper ──────────────────────────────────────────────────────────────
    private static void CreateEmptyContainer(string name, Scene scene)
    {
        var go = new GameObject(name);
        SceneManager.MoveGameObjectToScene(go, scene);
    }
}
#endif
