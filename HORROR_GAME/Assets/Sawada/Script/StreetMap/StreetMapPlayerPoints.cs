using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>現在のエリア名に対応するPoint_の子要素を1つだけ表示する。</summary>
[RequireComponent(typeof(CanvasGroup))]
public class StreetMapPlayerPoints : MonoBehaviour
{
    [SerializeField] private StreetMapManager m_streetMap;
    [SerializeField] private PlayerController m_player;
    [SerializeField] private AreaManager m_areaManager;
    [Tooltip("例：エリアAにはPoint_Aを表示する。")]
    [SerializeField] private string m_pointPrefix = "Point_";

    private readonly Dictionary<string, GameObject> m_points =
        new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
    private CanvasGroup m_group;
    private GameObject m_selectedPoint;
    private string m_lastMissingArea;

    private void Awake()
    {
        m_group = GetComponent<CanvasGroup>();
        m_group.alpha = 0f;
        m_group.interactable = false;
        m_group.blocksRaycasts = false;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            if (!child.name.StartsWith(m_pointPrefix, StringComparison.OrdinalIgnoreCase)) continue;
            string areaName = child.name.Substring(m_pointPrefix.Length).Trim();
            if (!m_points.TryAdd(areaName, child.gameObject))
                Debug.LogWarning($"StreetMap: 現在地マーカーの名前が重複しています: {child.name}", this);
        }
    }

    private void Start()
    {
        if (m_streetMap == null) m_streetMap = FindFirstObjectByType<StreetMapManager>();
        if (m_player == null) m_player = FindFirstObjectByType<PlayerController>();
        if (m_areaManager == null) m_areaManager = FindFirstObjectByType<AreaManager>();
        if (m_streetMap == null || m_player == null || m_areaManager == null)
            Debug.LogWarning("StreetMap: 現在地表示のMap・Player・AreaManagerの参照を設定してください。", this);
    }

    private void LateUpdate()
    {
        float alpha = m_streetMap != null ? m_streetMap.MapAlpha : 0f;
        m_group.alpha = alpha;
        GameObject nextPoint = null;
        if (alpha > 0f && m_player != null && m_areaManager != null)
        {
            MapData map = m_areaManager.GetMapData();
            AreaController area = map != null ? map.GetArea(m_player.GetCurrentIndex()) : null;
            string areaName = area != null ? area.GetAreaName()?.Trim() : null;
            if (!string.IsNullOrEmpty(areaName))
            {
                if (m_points.TryGetValue(areaName, out nextPoint))
                    m_lastMissingArea = null;
                else if (m_lastMissingArea != areaName)
                {
                    Debug.LogWarning($"StreetMap: 現在地に対応する{m_pointPrefix}{areaName}がありません。", this);
                    m_lastMissingArea = areaName;
                }
            }
        }

        if (nextPoint == m_selectedPoint) return;
        if (m_selectedPoint != null) m_selectedPoint.SetActive(false);
        m_selectedPoint = nextPoint;
        if (m_selectedPoint != null) m_selectedPoint.SetActive(true);
    }

    private void OnDisable()
    {
        if (m_group != null) m_group.alpha = 0f;
        if (m_selectedPoint != null) m_selectedPoint.SetActive(false);
        m_selectedPoint = null;
    }
}
