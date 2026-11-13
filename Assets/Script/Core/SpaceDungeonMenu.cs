#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Menú principal de Space Dungeon en el editor de Unity.
/// Agrupa todos los utilitarios de setup en un único menú accesible.
/// </summary>
public static class SpaceDungeonMenu
{
    [MenuItem("SpaceDungeon/Setup/Run Full Setup (Layers + Scenes + Animator)", false, 0)]
    public static void RunFullSetup()
    {
        Debug.Log("[SpaceDungeon] Iniciando setup completo...");

        // 1. Configurar layers y tags
        LayerTagSetup.ConfigureLayersAndTags();

        // 2. Crear AnimatorController del jugador
        PlayerAnimatorSetup.CreatePlayerAnimator();

        // 3. Crear escenas de niveles
        LevelSceneSetup.CreateLevel1();
        LevelSceneSetup.CreateLevel2();

        Debug.Log("[SpaceDungeon] Setup completo finalizado. Revisa la consola para detalles.");
    }

    [MenuItem("SpaceDungeon/Documentation/Open Diagnostico", false, 100)]
    public static void OpenDocs()
    {
        Application.OpenURL("https://github.com/VictorGHOSTH/SpaceDoungeon");
    }
}
#endif
