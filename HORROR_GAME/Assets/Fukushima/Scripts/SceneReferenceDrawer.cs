#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        SerializedProperty sceneGuidProperty =
            property.FindPropertyRelative("m_sceneGuid");

        SerializedProperty scenePathProperty =
            property.FindPropertyRelative("m_scenePath");

        string scenePath =
            AssetDatabase.GUIDToAssetPath(sceneGuidProperty.stringValue);

        if (string.IsNullOrWhiteSpace(scenePath))
        {
            scenePath = scenePathProperty.stringValue;
        }

        SceneAsset currentSceneAsset =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();

        SceneAsset selectedSceneAsset =
            (SceneAsset)EditorGUI.ObjectField(
                position,
                label,
                currentSceneAsset,
                typeof(SceneAsset),
                false);

        if (EditorGUI.EndChangeCheck())
        {
            if (selectedSceneAsset == null)
            {
                sceneGuidProperty.stringValue = string.Empty;
                scenePathProperty.stringValue = string.Empty;
            }
            else
            {
                string selectedScenePath =
                    AssetDatabase.GetAssetPath(selectedSceneAsset);

                sceneGuidProperty.stringValue =
                    AssetDatabase.AssetPathToGUID(selectedScenePath);

                scenePathProperty.stringValue = selectedScenePath;
            }
        }
        else if (currentSceneAsset != null &&
                 scenePathProperty.stringValue != scenePath)
        {
            scenePathProperty.stringValue = scenePath;
        }

        EditorGUI.EndProperty();
    }
}
#endif