using System.Collections;
using UnityEngine;

public class SceneFadeInController : MonoBehaviour
{
    [Header("フェード")]
    [SerializeField] private CanvasGroup m_fadeCanvasGroup;

    [Header("実行設定")]
    [SerializeField] private bool m_enableFadeIn = true;
    [SerializeField] private bool m_enableFadeOut = true;

    [Header("時間")]
    [SerializeField, Min(0.0f)]
    private float m_fadeInDuration = 1.0f;

    [SerializeField, Min(0.0f)]
    private float m_fadeOutDuration = 1.0f;

    private Coroutine m_fadeCoroutine;

    private void Start()
    {
        if (m_enableFadeIn)
        {
            StartFadeIn();
        }
        else
        {
            SetTransparent();
        }
    }

    public void StartFadeIn()
    {
        if (!m_enableFadeIn)
        {
            return;
        }

        StartFade(
            1.0f,
            0.0f,
            m_fadeInDuration,
            true);
    }

    public void StartFadeOut()
    {
        if (!m_enableFadeOut)
        {
            return;
        }

        StartFade(
            0.0f,
            1.0f,
            m_fadeOutDuration,
            false);
    }

    private void StartFade(
        float startAlpha,
        float endAlpha,
        float duration,
        bool deactivateAfterFade)
    {
        if (m_fadeCoroutine != null)
        {
            StopCoroutine(m_fadeCoroutine);
        }

        m_fadeCoroutine = StartCoroutine(
            Fade(
                startAlpha,
                endAlpha,
                duration,
                deactivateAfterFade));
    }

    private IEnumerator Fade(
        float startAlpha,
        float endAlpha,
        float duration,
        bool deactivateAfterFade)
    {
        if (m_fadeCanvasGroup == null)
        {
            yield break;
        }

        m_fadeCanvasGroup.gameObject.SetActive(true);
        m_fadeCanvasGroup.blocksRaycasts = true;
        m_fadeCanvasGroup.alpha = startAlpha;

        if (duration <= 0.0f)
        {
            m_fadeCanvasGroup.alpha = endAlpha;
        }
        else
        {
            float timer = 0.0f;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;

                float rate =
                    Mathf.Clamp01(timer / duration);

                m_fadeCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        endAlpha,
                        rate);

                yield return null;
            }

            m_fadeCanvasGroup.alpha = endAlpha;
        }

        if (deactivateAfterFade)
        {
            m_fadeCanvasGroup.blocksRaycasts = false;
            m_fadeCanvasGroup.gameObject.SetActive(false);
        }

        m_fadeCoroutine = null;
    }

    private void SetTransparent()
    {
        if (m_fadeCanvasGroup == null)
        {
            return;
        }

        m_fadeCanvasGroup.alpha = 0.0f;
        m_fadeCanvasGroup.blocksRaycasts = false;
        m_fadeCanvasGroup.gameObject.SetActive(false);
    }
}