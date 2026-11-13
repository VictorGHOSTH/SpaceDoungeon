#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility: configura las capas (Layers) y Tags necesarios para Space Dungeon.
/// Deben existir antes de asignarlos a prefabs y scripts.
///
/// Uso: menú superior de Unity → SpaceDungeon → Setup → Configure Layers and Tags
/// </summary>
public static class LayerTagSetup
{
    [MenuItem("SpaceDungeon/Setup/Configure Layers and Tags")]
    public static void ConfigureLayersAndTags()
    {
        AddTag("Player");
        AddTag("Enemy");
        AddTag("Collectible");
        AddTag("Ground");

        AddLayer("Ground");
        AddLayer("Player");
        AddLayer("Enemy");
        AddLayer("Collectible");

        AssetDatabase.SaveAssets();
        Debug.Log("[SpaceDungeon] Tags y Layers configurados correctamente.");
    }

    // ─── Añadir Tag si no existe ─────────────────────────────────────────────
    private static void AddTag(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
                return; // ya existe
        }

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
        tagManager.ApplyModifiedProperties();
        Debug.Log($"[SpaceDungeon] Tag añadido: '{tagName}'");
    }

    // ─── Añadir Layer si no existe (capas 8-31 disponibles para usuario) ─────
    private static void AddLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layersProp = tagManager.FindProperty("layers");

        // Verificar si ya existe
        for (int i = 8; i < layersProp.arraySize; i++)
        {
            var existingLayer = layersProp.GetArrayElementAtIndex(i);
            if (existingLayer.stringValue == layerName)
                return; // ya existe
        }

        // Buscar un slot vacío (capas 8-31)
        for (int i = 8; i < Mathf.Min(layersProp.arraySize, 32); i++)
        {
            var layer = layersProp.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"[SpaceDungeon] Layer añadido: '{layerName}' en slot {i}");
                return;
            }
        }

        Debug.LogWarning($"[SpaceDungeon] No se pudo añadir el layer '{layerName}': no hay slots disponibles.");
    }
}
#endif
