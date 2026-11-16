using UnityEngine;
using UnityEngine.UI;

// 中央の不透明な範囲を保持し地図の四辺に向かって透明にする
[AddComponentMenu("UI/Effects/Map Image Edge Fade")]
[RequireComponent(typeof(Image))]
public class MapImageEdgeFade : BaseMeshEffect
{
    [Header("不透明な中央範囲")]
    [Tooltip("オンなら中央範囲の割合、オフならFade Widthで指定する。")]
    [SerializeField] private bool m_useOpaqueArea = true;
    [Tooltip("横幅のうち、不透明に保つ中央の割合。0.9なら中央90%、左右各5%がグラデーション。")]
    [SerializeField, Range(0f, 1f)] private float m_opaqueWidth = 0.96f;
    [Tooltip("高さのうち、不透明に保つ中央の割合。0.9なら中央90%、上下各5%がグラデーション。")]
    [SerializeField, Range(0f, 1f)] private float m_opaqueHeight = 0.96f;

    [Header("幅で指定する場合")]
    [Tooltip("Use Opaque Areaがオフの時のぼかし幅（Canvas上の単位）。0で無効。")]
    [SerializeField, Min(0f)] private float m_fadeWidth = 16f;

    protected override void OnEnable()
    {
        EnableUVChannel();
        base.OnEnable();
    }

    protected override void OnCanvasHierarchyChanged()
    {
        base.OnCanvasHierarchyChanged();
        EnableUVChannel();
    }

    private void EnableUVChannel()
    {
        if (graphic != null && graphic.canvas != null)
            graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
    }

    public override void ModifyMesh(VertexHelper vertices)
    {
        if (!IsActive() || vertices.currentVertCount == 0) return;

        // 実際の頂点の範囲を使い、Preserve AspectとSpriteAtlasにも対応する。
        var vertex = new UIVertex();
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        for (int i = 0; i < vertices.currentVertCount; i++)
        {
            vertices.PopulateUIVertex(ref vertex, i);
            min = Vector2.Min(min, vertex.position);
            max = Vector2.Max(max, vertex.position);
        }

        Vector2 size = max - min;
        size.x = Mathf.Max(size.x, 0.001f);
        size.y = Mathf.Max(size.y, 0.001f);
        float fadeX = m_useOpaqueArea
            ? (1f - Mathf.Clamp01(m_opaqueWidth)) * 0.5f
            : Mathf.Clamp(m_fadeWidth / size.x, 0f, 0.5f);
        float fadeY = m_useOpaqueArea
            ? (1f - Mathf.Clamp01(m_opaqueHeight)) * 0.5f
            : Mathf.Clamp(m_fadeWidth / size.y, 0f, 0.5f);
        for (int i = 0; i < vertices.currentVertCount; i++)
        {
            vertices.PopulateUIVertex(ref vertex, i);
            vertex.uv1 = new Vector4((vertex.position.x - min.x) / size.x,
                (vertex.position.y - min.y) / size.y,
                fadeX, fadeY);
            vertices.SetUIVertex(vertex, i);
        }
    }
}
