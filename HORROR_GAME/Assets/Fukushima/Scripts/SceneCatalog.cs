using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SceneCatalog",
    menuName = "HorrorGame/Scene Catalog")]
public class SceneCatalog : ScriptableObject
{
    [Header("固定シーン")]
    [SerializeField]
    private SceneReference m_titleSceneReference =
        new SceneReference();

    [SerializeField]
    private SceneReference m_prologueSceneReference =
        new SceneReference();

    [SerializeField]
    private SceneReference m_areaSelectSceneReference =
        new SceneReference();

    [SerializeField]
    private SceneReference m_resultSceneReference =
        new SceneReference();

    [Header("エリアシーン")]
    [SerializeField]
    private List<AreaSceneData> m_areaSceneDataList =
        new List<AreaSceneData>();

    public bool TryGetSceneReference(
        SCENE_ID sceneId,
        out SceneReference sceneReference)
    {
        sceneReference = null;

        switch (sceneId)
        {
            case SCENE_ID.TITLE:
                sceneReference = m_titleSceneReference;
                break;

            case SCENE_ID.PROLOGUE:
                sceneReference = m_prologueSceneReference;
                break;

            case SCENE_ID.AREA_SELECT:
                sceneReference = m_areaSelectSceneReference;
                break;

            case SCENE_ID.RESULT:
                sceneReference = m_resultSceneReference;
                break;

            default:
                return false;
        }

        return sceneReference != null &&
               sceneReference.HasSceneFlag();
    }

    public bool TryGetAreaSceneReference(
        int areaId,
        out SceneReference sceneReference)
    {
        sceneReference = null;

        foreach (AreaSceneData areaSceneData in m_areaSceneDataList)
        {
            if (areaSceneData == null)
            {
                continue;
            }

            if (!areaSceneData.IsMatchAreaIdFlag(areaId))
            {
                continue;
            }

            return areaSceneData.TryGetSceneReference(
                out sceneReference);
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        RefreshSceneReference(m_titleSceneReference);
        RefreshSceneReference(m_prologueSceneReference);
        RefreshSceneReference(m_areaSelectSceneReference);
        RefreshSceneReference(m_resultSceneReference);

        foreach (AreaSceneData areaSceneData in m_areaSceneDataList)
        {
            if (areaSceneData == null)
            {
                continue;
            }

            areaSceneData.RefreshScenePath();
        }
    }

    private void RefreshSceneReference(SceneReference sceneReference)
    {
        if (sceneReference == null)
        {
            return;
        }

        sceneReference.RefreshScenePath();
    }
#endif
}

[Serializable]
public class AreaSceneData
{
    [SerializeField] private int m_areaId = 1;

    [SerializeField]
    private SceneReference m_sceneReference =
        new SceneReference();

    public bool IsMatchAreaIdFlag(int areaId)
    {
        return m_areaId == areaId;
    }

    public bool TryGetSceneReference(
        out SceneReference sceneReference)
    {
        sceneReference = m_sceneReference;

        return sceneReference != null &&
               sceneReference.HasSceneFlag();
    }

#if UNITY_EDITOR
    public void RefreshScenePath()
    {
        if (m_sceneReference == null)
        {
            return;
        }

        m_sceneReference.RefreshScenePath();
    }
#endif
}