using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StoryMessageData
{
    [TextArea(3, 8)]
    [SerializeField] private string m_message = string.Empty;

    public string GetMessage()
    {
        return m_message;
    }
}

public class StorySequenceController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text m_storyText;
    [SerializeField] private GameObject m_nextHintObject;

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
    private Coroutine m_typingCoroutine;

    private void Start()
    {
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
        m_loadingFlag)
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

    public void AdvanceMessage()
    {
        if (m_loadingFlag)
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
        }

        string message =
            m_messageDataList[m_currentMessageIndex].GetMessage();

        m_typingCoroutine = StartCoroutine(TypeMessage(message));
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

        yield return FadeOut();

        SceneManager.LoadScene(
            m_nextSceneReference.GetScenePath());
    }

    private IEnumerator FadeOut()
    {
        if (m_fadeCanvasGroup == null ||
            m_fadeOutDuration <= 0.0f)
        {
            yield break;
        }

        m_fadeCanvasGroup.gameObject.SetActive(true);
        m_fadeCanvasGroup.blocksRaycasts = true;

        float timer = 0.0f;

        while (timer < m_fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;

            float rate = Mathf.Clamp01(timer / m_fadeOutDuration);
            m_fadeCanvasGroup.alpha = rate;

            yield return null;
        }

        m_fadeCanvasGroup.alpha = 1.0f;
    }

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

        ShowMessage(0);
    }
}