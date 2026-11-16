using UnityEngine;

public class EpilogueBackgroundDrift : MonoBehaviour
{
    [Header("ˆÚ“®‘ÎÛ")]
    [SerializeField] private RectTransform m_targetRect;

    [Header("Šg‘å—¦")]
    [SerializeField] private float m_scale = 1.15f;

    [Header("ˆÚ“®—Ê")]
    [SerializeField] private Vector2 m_moveRange = new Vector2(40.0f, 25.0f);

    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] private float m_moveSpeed = 0.08f;

    [Header("ˆÚ“®•ûŒü")]
    [SerializeField]
    private bool m_leftTopToRightBottomFlag = false;



    private Vector2 m_startAnchoredPosition;
    private float m_timer;

    private void Start()
    {
        if (m_targetRect == null)
        {
            m_targetRect = GetComponent<RectTransform>();
        }

        if (m_targetRect == null)
        {
            enabled = false;
            return;
        }

        m_startAnchoredPosition = m_targetRect.anchoredPosition;
        m_targetRect.localScale = Vector3.one * m_scale;
    }

    private void Update()
    {
        m_timer += Time.deltaTime * m_moveSpeed;

        float t = Mathf.PingPong(m_timer, 1.0f);
        float easedT = Mathf.SmoothStep(0.0f, 1.0f, t);

        Vector2 from;
        Vector2 to;

        if (m_leftTopToRightBottomFlag)
        {
            // ¶ã ¨ ‰E‰º
            from = new Vector2(m_moveRange.x, -m_moveRange.y);

            to = new Vector2(-m_moveRange.x, m_moveRange.y);
        }
        else
        {
            // ‰Eã ¨ ¶‰º
            from = new Vector2(-m_moveRange.x, -m_moveRange.y);

            to = new Vector2(m_moveRange.x, m_moveRange.y);
        }

        m_targetRect.anchoredPosition =
            m_startAnchoredPosition + Vector2.Lerp(from, to, easedT);
    }
}