using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPrologueController : MonoBehaviour
{
    [Header("チャット全体")]
    [SerializeField] private GameObject m_chatRoot;

    [Header("後半のストーリー全体")]
    [SerializeField] private GameObject m_storyRoot;

    [Header("後半テキスト制御")]
    [SerializeField] private StorySequenceController m_storySequenceController;

    [Header("最初のメッセージと同時に出すアイコン")]
    [SerializeField] private RectTransform m_firstIconItem;

    [Header("順番に表示するチャット画像")]
    [SerializeField]
    private List<RectTransform> m_chatItemList =
        new List<RectTransform>();

    [Header("スクロール")]
    [SerializeField] private ScrollRect m_scrollRect;

    [Header("アニメーション設定")]
    [SerializeField] private float m_slideDistance = 35.0f;
    [SerializeField] private float m_appearDuration = 0.18f;

    [Header("画面切り替えフェード")]
    [SerializeField] private CanvasGroup m_transitionFadeCanvasGroup;

    [SerializeField, Min(0.0f)]
    private float m_screenFadeDuration = 0.8f;

    [Header("入力設定")]
    [SerializeField] private bool m_enableInputFlag = true;

    private int m_currentIndex;
    private bool m_animatingFlag;
    private bool m_finishedChatFlag;

    private readonly Dictionary<RectTransform, Vector2> m_defaultPositionTable =
        new Dictionary<RectTransform, Vector2>();

    private void Start()
    {
        if (m_chatRoot != null)
        {
            m_chatRoot.SetActive(true);
        }

        if (m_storyRoot != null)
        {
            m_storyRoot.SetActive(false);
        }

        if (m_transitionFadeCanvasGroup != null)
        {
            m_transitionFadeCanvasGroup.gameObject.SetActive(false);
            m_transitionFadeCanvasGroup.alpha = 0.0f;
            m_transitionFadeCanvasGroup.blocksRaycasts = false;
        }

        SetupChatItems();
    }

    private void Update()
    {
        if (!m_enableInputFlag || m_animatingFlag)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            AdvanceChat();
        }
    }

    private void SetupChatItems()
    {
        m_defaultPositionTable.Clear();

        SetupItem(m_firstIconItem);

        foreach (RectTransform chatItem in m_chatItemList)
        {
            SetupItem(chatItem);
        }
    }

    private void SetupItem(RectTransform item)
    {
        if (item == null)
        {
            return;
        }

        if (!m_defaultPositionTable.ContainsKey(item))
        {
            m_defaultPositionTable.Add(item, item.anchoredPosition);
        }

        CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = item.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0.0f;
        item.gameObject.SetActive(false);
    }

    private void AdvanceChat()
    {
        if (m_finishedChatFlag)
        {
            StartStory();
            return;
        }

        if (m_currentIndex >= m_chatItemList.Count)
        {
            m_finishedChatFlag = true;
            return;
        }

        RectTransform targetItem = m_chatItemList[m_currentIndex];
        m_currentIndex++;

        if (targetItem == null)
        {
            AdvanceChat();
            return;
        }

        // 最初のメッセージだけ、アイコンとText1を同時に出す
        if (m_currentIndex == 1 && m_firstIconItem != null)
        {
            StartCoroutine(ShowChatItems(m_firstIconItem, targetItem));
            return;
        }

        StartCoroutine(ShowChatItem(targetItem));
    }

    private IEnumerator ShowChatItem(RectTransform targetItem)
    {
        m_animatingFlag = true;

        targetItem.gameObject.SetActive(true);

        CanvasGroup canvasGroup =
            targetItem.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = targetItem.gameObject.AddComponent<CanvasGroup>();
        }

        Vector2 endPosition = m_defaultPositionTable[targetItem];
        Vector2 startPosition = endPosition + new Vector2(0.0f, -m_slideDistance);

        targetItem.anchoredPosition = startPosition;
        targetItem.localScale = Vector3.one * 0.96f;
        canvasGroup.alpha = 0.0f;

        float timer = 0.0f;

        while (timer < m_appearDuration)
        {
            timer += Time.unscaledDeltaTime;

            float rate = Mathf.Clamp01(timer / m_appearDuration);
            float easedRate = EaseOutBack(rate);

            targetItem.anchoredPosition =
                Vector2.LerpUnclamped(startPosition, endPosition, easedRate);

            targetItem.localScale =
                Vector3.LerpUnclamped(
                    Vector3.one * 0.96f,
                    Vector3.one,
                    easedRate);

            canvasGroup.alpha = rate;

            yield return null;
        }

        targetItem.anchoredPosition = endPosition;
        targetItem.localScale = Vector3.one;
        canvasGroup.alpha = 1.0f;

        yield return ScrollToBottom();

        m_animatingFlag = false;
    }

    private IEnumerator ShowChatItems(
    RectTransform firstItem,
    RectTransform secondItem)
    {
        m_animatingFlag = true;

        PrepareShowItem(firstItem);
        PrepareShowItem(secondItem);

        float timer = 0.0f;

        while (timer < m_appearDuration)
        {
            timer += Time.unscaledDeltaTime;

            float rate = Mathf.Clamp01(timer / m_appearDuration);
            float easedRate = EaseOutBack(rate);

            UpdateShowItem(firstItem, rate, easedRate);
            UpdateShowItem(secondItem, rate, easedRate);

            yield return null;
        }

        FinishShowItem(firstItem);
        FinishShowItem(secondItem);

        yield return ScrollToBottom();

        m_animatingFlag = false;
    }

    private void PrepareShowItem(RectTransform item)
    {
        if (item == null)
        {
            return;
        }

        item.gameObject.SetActive(true);

        CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = item.gameObject.AddComponent<CanvasGroup>();
        }

        Vector2 endPosition = m_defaultPositionTable[item];
        Vector2 startPosition =
            endPosition + new Vector2(0.0f, -m_slideDistance);

        item.anchoredPosition = startPosition;
        item.localScale = Vector3.one * 0.96f;
        canvasGroup.alpha = 0.0f;
    }

    private void UpdateShowItem(
        RectTransform item,
        float rate,
        float easedRate)
    {
        if (item == null)
        {
            return;
        }

        CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();

        Vector2 endPosition = m_defaultPositionTable[item];
        Vector2 startPosition =
            endPosition + new Vector2(0.0f, -m_slideDistance);

        item.anchoredPosition =
            Vector2.LerpUnclamped(startPosition, endPosition, easedRate);

        item.localScale =
            Vector3.LerpUnclamped(
                Vector3.one * 0.96f,
                Vector3.one,
                easedRate);

        canvasGroup.alpha = rate;
    }

    private void FinishShowItem(RectTransform item)
    {
        if (item == null)
        {
            return;
        }

        CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();

        item.anchoredPosition = m_defaultPositionTable[item];
        item.localScale = Vector3.one;
        canvasGroup.alpha = 1.0f;
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null;

        if (m_scrollRect == null)
        {
            yield break;
        }

        Canvas.ForceUpdateCanvases();

        // 一番下にスクロール
        m_scrollRect.verticalNormalizedPosition = 0.0f;
    }

    private void StartStory()
    {
        if (m_animatingFlag)
        {
            return;
        }

        StartCoroutine(StartStoryCoroutine());
    }

    private IEnumerator StartStoryCoroutine()
    {
        m_animatingFlag = true;

        yield return FadeCanvasGroup(
            m_transitionFadeCanvasGroup,
            0.0f,
            1.0f,
            m_screenFadeDuration);

        if (m_chatRoot != null)
        {
            m_chatRoot.SetActive(false);
        }

        if (m_storyRoot != null)
        {
            m_storyRoot.SetActive(true);
        }

        yield return FadeCanvasGroup(
            m_transitionFadeCanvasGroup,
            1.0f,
            0.0f,
            m_screenFadeDuration);

        if (m_storySequenceController != null)
        {
            m_storySequenceController.BeginSequence();
        }

        enabled = false;
    }

    private IEnumerator FadeCanvasGroup(
    CanvasGroup canvasGroup,
    float startAlpha,
    float endAlpha,
    float duration)
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;

        if (duration <= 0.0f)
        {
            canvasGroup.alpha = endAlpha;

            if (Mathf.Approximately(endAlpha, 0.0f))
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.gameObject.SetActive(false);
            }

            yield break;
        }

        canvasGroup.alpha = startAlpha;

        float timer = 0.0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float rate = Mathf.Clamp01(timer / duration);

            canvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, rate);

            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (Mathf.Approximately(endAlpha, 0.0f))
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);
        }
    }

    private float EaseOutBack(float rate)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1.0f;

        return 1.0f + c3 * Mathf.Pow(rate - 1.0f, 3.0f)
               + c1 * Mathf.Pow(rate - 1.0f, 2.0f);
    }
}