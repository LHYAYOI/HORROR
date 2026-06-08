using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(testInspexViw))]
public class testInspex : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty type = serializedObject.FindProperty("m_settingType");

        EditorGUILayout.PropertyField(type);

        switch ((SettingType)type.enumValueIndex)
        {
            case SettingType.A:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_aValue"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_aSpeed"));
                break;
            case SettingType.B:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_bValue"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_bSpeed"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}