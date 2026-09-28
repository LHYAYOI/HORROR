using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StoryMessageData
{
    [TextArea(3, 8)]
    [SerializeField] private string m_message = string.Empty;

    [SerializeField] private Sprite m_backgroundSprite;

    public string GetMessage()
    {
        return m_message;
    }

    public Sprite GetBackgroundSprite()
    {
        return m_backgroundSprite;
    }
}

public class StorySequenceController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text m_storyText;
    [SerializeField] private GameObject m_nextHintObject;

    [Header("背景")]
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private CanvasGroup m_backgroundFadeCanvasGroup;
    [SerializeField, Min(0.0f)]
    private float m_backgroundFadeDuration = 0.5f;

    [Header("文章")]
    [SerializeField]
    private List<StoryMessageData> m_messageDataList =
        new List<StoryMessageData>();

    [SerializeField, Min(0.01f)]
    private float m_characterInterval = 0.03f;

    [Header("遷移先")]
    [SerializeField]
    private SceneReference m_nextSceneReference =
        new SceneReference();

    [Header("フェード")]
    [SerializeField] private CanvasGroup m_fadeCanvasGroup;
    [SerializeField, Min(0.0f)] private float m_fadeOutDuration = 1.0f;

    [Header("入力設定")]
    [SerializeField] private bool m_enableInputFlag = true;

    [Header("開始設定")]
    [SerializeField] private bool m_playOnStartFlag = true;

    private int m_currentMessageIndex;
    private bool m_typingFlag;
    private bool m_loadingFlag;
    private bool m_sequenceStartedFlag;
    private bool m_messageTransitionFlag;

    private Coroutine m_typingCoroutine;
    private Coroutine m_showMessageCoroutine;

    private Sprite m_currentBackgroundSprite;

    private void Start()
    {
        InitializeFadeCanvas();

        if (!m_playOnStartFlag)
        {
            return;
        }

        BeginSequence();
    }

    private void Update()
    {
        if (!m_sequenceStartedFlag ||
        !m_enableInputFlag ||
        m_loadingFlag ||
         m_messageTransitionFlag)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            AdvanceMessage();
        }
    }

    private void InitializeFadeCanvas()
    {
        if (m_fadeCanvasGroup == null)
        {
            return;
        }

        m_fadeCanvasGroup.alpha = 0.0f;
        m_fadeCanvasGroup.blocksRaycasts = false;
        m_fadeCanvasGroup.gameObject.SetActive(false);
    }


    public void AdvanceMessage()
    {
        if (m_loadingFlag || m_messageTransitionFlag)
        {
            return;
        }

        // 文字送り中なら全文を表示する
        if (m_typingFlag)
        {
            CompleteTyping();
            return;
        }

        int nextMessageIndex = m_currentMessageIndex + 1;

        // 次の文章へ進む
        if (nextMessageIndex < m_messageDataList.Count)
        {
            ShowMessage(nextMessageIndex);
            return;
        }

        // 最後の文章まで終わったら次のシーンへ
        LoadNextScene();
    }

    private void ShowMessage(int messageIndex)
    {
        m_currentMessageIndex = messageIndex;

        if (m_typingCoroutine != null)
        {
            StopCoroutine(m_typingCoroutine);
            m_typingCoroutine = null;
        }

        //if (m_showMessageCoroutine != null)
        //{
        //    StopCoroutine(m_showMessageCoroutine);
        //    m_showMessageCoroutine = null;
        //}


        //string message =
        //    m_messageDataList[m_currentMessageIndex].GetMessage();

        //m_typingCoroutine = StartCoroutine(TypeMessage(message));

        m_showMessageCoroutine = StartCoroutine(
           ShowMessageCoroutine(m_messageDataList[m_currentMessageIndex]));
    }

    private IEnumerator ShowMessageCoroutine(StoryMessageData messageData)
    {
        SetNextHintActive(false);

        yield return ChangeBackgroundIfNeeded(
            messageData.GetBackgroundSprite());

        string message = messageData.GetMessage();
        m_typingCoroutine = StartCoroutine(TypeMessage(message));

        m_showMessageCoroutine = null;
    }

    private IEnumerator ChangeBackgroundIfNeeded(Sprite nextBackgroundSprite)
    {
        if (m_backgroundImage == null)
        {
            yield break;
        }

        // 背景画像が設定されていない場合は変更しない
        if (nextBackgroundSprite == null)
        {
            yield break;
        }

        // 同じ背景なら何もしない
        if (m_currentBackgroundSprite == nextBackgroundSprite)
        {
            yield break;
        }

        m_messageTransitionFlag = true;

        yield return FadeCanvasGroup(
            m_backgroundFadeCanvasGroup,
            0.0f,
            1.0f,
            m_backgroundFadeDuration);

        m_backgroundImage.sprite = nextBackgroundSprite;
        m_currentBackgroundSprite = nextBackgroundSprite;

        yield return FadeCanvasGroup(
            m_backgroundFadeCanvasGroup,
            1.0f,
            0.0f,
            m_backgroundFadeDuration);

        m_messageTransitionFlag = false;
    }

    private IEnumerator TypeMessage(string message)
    {
        m_typingFlag = true;
        SetNextHintActive(false);

        m_storyText.text = message ?? string.Empty;

        // TextMeshProに文字情報を即座に作らせる
        m_storyText.maxVisibleCharacters = int.MaxValue;
        m_storyText.ForceMeshUpdate();

        int characterCount = m_storyText.textInfo.characterCount;
        m_storyText.maxVisibleCharacters = 0;

        for (int visibleCharacterCount = 1;
             visibleCharacterCount <= characterCount;
             visibleCharacterCount++)
        {
            m_storyText.maxVisibleCharacters = visibleCharacterCount;

            yield return new WaitForSecondsRealtime(
                m_characterInterval);
        }

        m_storyText.maxVisibleCharacters = int.MaxValue;

        m_typingFlag = false;
        m_typingCoroutine = null;

        SetNextHintActive(true);
    }

    private void CompleteTyping()
    {
        if (m_typingCoroutine != null)
        {
            StopCoroutine(m_typingCoroutine);
            m_typingCoroutine = null;
        }

        m_storyText.maxVisibleCharacters = int.MaxValue;

        m_typingFlag = false;

        SetNextHintActive(true);
    }

    private void LoadNextScene()
    {
        if (m_nextSceneReference == null ||
            !m_nextSceneReference.HasSceneFlag())
        {
            Debug.LogError(
                "StorySequenceController：遷移先シーンが設定されていません。");

            return;
        }

        if (m_loadingFlag)
        {
            return;
        }


        StartCoroutine(LoadNextSceneCoroutine());
    }

    private IEnumerator LoadNextSceneCoroutine()
    {
        m_loadingFlag = true;
        SetNextHintActive(false);

        //yield return FadeOut();

        yield return FadeCanvasGroup(
            m_fadeCanvasGroup,
            0.0f,
            1.0f,
            m_fadeOutDuration);

        SceneManager.LoadScene(
            m_nextSceneReference.GetScenePath());
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
        canvasGroup.alpha = startAlpha;

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

        float timer = 0.0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float rate = Mathf.Clamp01(timer / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, rate);

            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (Mathf.Approximately(endAlpha, 0.0f))
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);
        }
    }

    //private IEnumerator FadeOut()
    //{
    //    if (m_fadeCanvasGroup == null ||
    //        m_fadeOutDuration <= 0.0f)
    //    {
    //        yield break;
    //    }

    //    m_fadeCanvasGroup.gameObject.SetActive(true);
    //    m_fadeCanvasGroup.blocksRaycasts = true;

    //    float timer = 0.0f;

    //    while (timer < m_fadeOutDuration)
    //    {
    //        timer += Time.unscaledDeltaTime;

    //        float rate = Mathf.Clamp01(timer / m_fadeOutDuration);
    //        m_fadeCanvasGroup.alpha = rate;

    //        yield return null;
    //    }

    //    m_fadeCanvasGroup.alpha = 1.0f;
    //}

    private void SetNextHintActive(bool activeFlag)
    {
        if (m_nextHintObject == null)
        {
            return;
        }

        m_nextHintObject.SetActive(activeFlag);
    }

    private bool ValidateSettings()
    {
        bool validFlag = true;

        if (m_storyText == null)
        {
            Debug.LogError(
                "StorySequenceController：StoryTextが設定されていません。");

            validFlag = false;
        }

        if (m_messageDataList == null ||
            m_messageDataList.Count == 0)
        {
            Debug.LogError(
                "StorySequenceController：文章が設定されていません。");

            validFlag = false;
        }

        return validFlag;
    }



    public void BeginSequence()
    {
        if (!ValidateSettings())
        {
            enabled = false;
            return;
        }

        m_sequenceStartedFlag = true;
        m_currentMessageIndex = 0;
        m_typingFlag = false;
        m_loadingFlag = false;
        m_messageTransitionFlag = false;
        m_currentBackgroundSprite = null;

        ShowMessage(0);
    }
}