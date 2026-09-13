using System.Collections;
using UnityEngine;

public class SceneFadeInController : MonoBehaviour
{
    [Header("フェード")]
    [SerializeField] private CanvasGroup m_fadeCanvasGroup;

    [SerializeField, Min(0.0f)]
    private float m_fadeInDuration = 1.0f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        if (m_fadeCanvasGroup == null)
        {
            yield break;
        }

        m_fadeCanvasGroup.gameObject.SetActive(true);
        m_fadeCanvasGroup.blocksRaycasts = true;
        m_fadeCanvasGroup.alpha = 1.0f;

        if (m_fadeInDuration <= 0.0f)
        {
            FinishFade();
            yield break;
        }

        float timer = 0.0f;

        while (timer < m_fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;

            float rate =
                Mathf.Clamp01(timer / m_fadeInDuration);

            m_fadeCanvasGroup.alpha =
                Mathf.Lerp(1.0f, 0.0f, rate);

            yield return null;
        }

        FinishFade();
    }

    private void FinishFade()
    {
        m_fadeCanvasGroup.alpha = 0.0f;
        m_fadeCanvasGroup.blocksRaycasts = false;
        m_fadeCanvasGroup.gameObject.SetActive(false);
    }
}