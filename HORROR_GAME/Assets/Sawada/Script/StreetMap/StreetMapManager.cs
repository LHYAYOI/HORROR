using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 手と紙の連番画像を同じフレームで再生するUI
public class StreetMapManager : MonoBehaviour
{
    [Header("表示先（紙の上に手を配置）")]
    [SerializeField] private Image m_handImage;
    [SerializeField] private Image m_paperImage;

    [Header("地図（MapBaseのMask配下に配置）")]
    [SerializeField] private Image m_mapImage;
    [Tooltip("先頭を1枚目としたフェード開始フレーム")]
    [SerializeField, Min(1)] private int m_mapFadeStartFrame = 30;
    [SerializeField, Min(0.001f)] private float m_mapFadeDuration = 0.2f;

    [Header("連番画像（番号の昇順）")]
    [SerializeField] private Texture2D[] m_handFrames;
    [SerializeField] private Texture2D[] m_paperFrames;

    [Header("再生設定")]
    [SerializeField, Min(1f)] private float m_frameRate = 30f;
    [SerializeField] private bool m_useUnscaledTime = true;

    private double m_framePosition;
    private int m_currentFrame = -1;
    private bool m_reverse;
    private bool m_isVisible;
    private Sprite[] m_handSprites;
    private Sprite[] m_paperSprites;

    public bool IsPlaying { get; private set; }
    public bool IsOpen { get; private set; }
    public float MapAlpha => m_isVisible && m_mapImage != null && m_mapImage.isActiveAndEnabled
        ? m_mapImage.color.a : 0f;

    private int FrameCount => Mathf.Min(
        m_handFrames == null ? 0 : m_handFrames.Length,
        m_paperFrames == null ? 0 : m_paperFrames.Length);

    private void OnEnable()
    {
        ResetClosed();
    }

    private void OnDisable()
    {
        ResetClosed();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            Toggle();

        if (!IsPlaying) return;

        float deltaTime = m_useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        m_framePosition += (m_reverse ? -1d : 1d) * deltaTime * Mathf.Max(1f, m_frameRate);
        m_framePosition = System.Math.Clamp(m_framePosition, 0d, FrameCount - 1d);
        ShowFrame((int)m_framePosition);
        UpdateMapAlpha();

        if (m_reverse ? m_framePosition <= 0d : m_framePosition >= FrameCount - 1d)
        {
            Stop();
            if (m_reverse) SetVisible(false);
        }
    }

    /// <summary>Mキー、ButtonのOnClickから表示を切り替える。</summary>
    public void Toggle()
    {
        if (IsOpen) PlayReverse();
        else Play();
    }

    /// <summary>地図を開く。逆再生中なら現在位置から正再生に戻す。</summary>
    [ContextMenu("Play")]
    public void Play()
    {
        BeginPlayback(false);
    }

    /// <summary>現在位置から逆再生し、終了後に地図を非表示にする。</summary>
    [ContextMenu("Play Reverse")]
    public void PlayReverse()
    {
        BeginPlayback(true);
    }

    /// <summary>現在の画像を保持して停止。</summary>
    public void Stop()
    {
        IsPlaying = false;
    }

    private void BeginPlayback(bool reverse)
    {
        if (reverse && !m_isVisible) return;

        Stop();
        if (m_handImage == null || m_paperImage == null || FrameCount == 0)
        {
            Debug.LogWarning("StreetMap: 表示先と手・紙の連番画像を設定してください。", this);
            return;
        }

        if (m_handFrames.Length != m_paperFrames.Length)
            Debug.LogWarning("StreetMap: 手と紙の枚数が異なるため、少ない方に合わせて再生します。", this);

        if (!m_isVisible) m_framePosition = 0d;
        m_reverse = reverse;
        IsOpen = !reverse;
        if (m_handSprites == null)
        {
            m_handSprites = CreateSprites(m_handFrames);
            m_paperSprites = CreateSprites(m_paperFrames);
        }
        m_currentFrame = -1;
        ShowFrame((int)m_framePosition);
        UpdateMapAlpha();
        SetVisible(true);
        IsPlaying = true;
    }

    private void ResetClosed()
    {
        Stop();
        IsOpen = false;
        m_framePosition = 0d;
        m_currentFrame = -1;
        UpdateMapAlpha();
        SetVisible(false);
    }

    private void UpdateMapAlpha()
    {
        if (m_mapImage == null) return;

        // 再生位置から算出するので、方向の切り替え時もαが飛ばない。
        double fadeElapsed = (m_framePosition - (m_mapFadeStartFrame - 1)) / Mathf.Max(1f, m_frameRate);
        Color color = m_mapImage.color;
        color.a = Mathf.Clamp01((float)(fadeElapsed / Mathf.Max(0.001f, m_mapFadeDuration)));
        m_mapImage.color = color;
        // 未指定のImageが白い矩形として表示されるのを防ぐ。
        m_mapImage.enabled = m_mapImage.sprite != null;
    }

    private void SetVisible(bool visible)
    {
        m_isVisible = visible;
        // 入力を受けるStreetMapManagerは有効なまま、表示用の子だけ切り替える。
        if (m_handImage != null) m_handImage.gameObject.SetActive(visible);
        if (m_paperImage != null) m_paperImage.gameObject.SetActive(visible);
    }

    private void ShowFrame(int frame)
    {
        if (frame == m_currentFrame) return;
        m_currentFrame = frame;
        m_handImage.sprite = m_handSprites[frame];
        m_paperImage.sprite = m_paperSprites[frame];
    }

    // 元画像のインポート設定を変更せず、既存のImageで表示する。
    private static Sprite[] CreateSprites(Texture2D[] frames)
    {
        var sprites = new Sprite[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            Texture2D texture = frames[i];
            if (texture == null) continue;
            sprites[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        }
        return sprites;
    }

    private void OnDestroy()
    {
        ReleaseSprites(m_handSprites);
        ReleaseSprites(m_paperSprites);
    }

    private static void ReleaseSprites(Sprite[] sprites)
    {
        if (sprites == null) return;
        foreach (Sprite sprite in sprites)
            if (sprite != null) Destroy(sprite);
    }
}
