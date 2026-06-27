using System;
using UnityEngine;

[Serializable]
public class SceneReference
{
    [SerializeField] private string m_sceneGuid = string.Empty;
    [SerializeField] private string m_scenePath = string.Empty;

    public bool HasSceneFlag()
    {
        return !string.IsNullOrWhiteSpace(m_scenePath);
    }

    public string GetScenePath()
    {
        return m_scenePath;
    }

#if UNITY_EDITOR
    public void SetSceneAsset(UnityEditor.SceneAsset sceneAsset)
    {
        if (sceneAsset == null)
        {
            m_sceneGuid = string.Empty;
            m_scenePath = string.Empty;
            return;
        }

        m_scenePath = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
        m_sceneGuid = UnityEditor.AssetDatabase.AssetPathToGUID(m_scenePath);
    }

    public void RefreshScenePath()
    {
        if (string.IsNullOrWhiteSpace(m_sceneGuid))
        {
            return;
        }

        string scenePath =
            UnityEditor.AssetDatabase.GUIDToAssetPath(m_sceneGuid);

        if (string.IsNullOrWhiteSpace(scenePath))
        {
            return;
        }

        m_scenePath = scenePath;
    }
#endif
}