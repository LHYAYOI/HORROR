using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class RumorMenuData
{
    [Header("ボタン名")]
    [SerializeField] private string m_buttonName = "噂";

    [Header("噂の画像")]
    [SerializeField] private Sprite m_rumorSprite;

    [Header("噂の説明文")]
    [TextArea(3, 8)]
    [SerializeField] private string m_description = string.Empty;

    public string ButtonName => m_buttonName;
    public Sprite RumorSprite => m_rumorSprite;
    public string Description => m_description;
}

public class PauseRumorMenuController : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [Header("メニュー全体")]
    [SerializeField] private GameObject m_menuRoot;

    [Header("噂ボタン")]
    [SerializeField]
    private List<Button> m_rumorButtonList =
        new List<Button>();

    [Header("選択中のボタン色")]
    [SerializeField] private Color m_normalButtonColor = Color.white;

    [SerializeField]
    private Color m_selectedButtonColor =
        new Color(0.7f, 0.15f, 0.15f, 1.0f);

    private int m_selectedRumorIndex = -1;

    [Header("噂データ")]
    [SerializeField]
    private List<RumorMenuData> m_rumorDataList =
        new List<RumorMenuData>();

    [Header("表示UI")]
    [SerializeField] private Image m_rumorImage;
    [SerializeField] private TMP_Text m_descriptionText;

    [Header("タイトルへ戻る")]
    [SerializeField] private Button m_titleButton;
    [SerializeField] private string m_titleSceneName = "Title_Test";

    [Header("カーソル制御")]
    [SerializeField] private bool m_controlCursor = true;

    private void Start()
    {
        SetupButtons();
        CloseMenu();

        if (m_rumorDataList.Count > 0)
        {
            ShowRumor(0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    private void SetupButtons()
    {
        int count = Mathf.Min(
            m_rumorButtonList.Count,
            m_rumorDataList.Count);

        for (int i = 0; i < count; i++)
        {
            int index = i;

            Button button = m_rumorButtonList[index];

            if (button == null)
            {
                continue;
            }

            TMP_Text buttonText =
                button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = m_rumorDataList[index].ButtonName;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ShowRumor(index));
        }

        if (m_titleButton != null)
        {
            m_titleButton.onClick.RemoveAllListeners();
            m_titleButton.onClick.AddListener(BackToTitle);
        }
    }

    public void OpenMenu()
    {
        IsPaused = true;

        if (m_menuRoot != null)
        {
            m_menuRoot.SetActive(true);
        }

        Time.timeScale = 0.0f;

        if (m_controlCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void CloseMenu()
    {
        IsPaused = false;

        if (m_menuRoot != null)
        {
            m_menuRoot.SetActive(false);
        }

        Time.timeScale = 1.0f;

        if (m_controlCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void ShowRumor(int index)
    {
        if (index < 0 || index >= m_rumorDataList.Count)
        {
            return;
        }

        RumorMenuData data = m_rumorDataList[index];

        if (m_rumorImage != null)
        {
            m_rumorImage.sprite = data.RumorSprite;
            m_rumorImage.enabled = data.RumorSprite != null;
        }

        if (m_descriptionText != null)
        {
            m_descriptionText.text = data.Description;
        }

        ApplyButtonColor(index);
    }

    private void ApplyButtonColor(int selectedIndex)
    {
        m_selectedRumorIndex = selectedIndex;

        for (int i = 0; i < m_rumorButtonList.Count; i++)
        {
            Button button = m_rumorButtonList[i];

            if (button == null)
            {
                continue;
            }

            bool selectedFlag = i == m_selectedRumorIndex;

            Color targetColor =
                selectedFlag ? m_selectedButtonColor : m_normalButtonColor;

            Image buttonImage = button.GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImage.color = targetColor;
            }

            // ButtonのColor Tintにも反映して、マウスを乗せた時に変に戻らないようにする
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = targetColor;
            colorBlock.highlightedColor = targetColor;
            colorBlock.pressedColor = targetColor;
            colorBlock.selectedColor = targetColor;
            button.colors = colorBlock;
        }
    }

    private void BackToTitle()
    {
        Time.timeScale = 1.0f;
        IsPaused = false;

        SceneManager.LoadScene(m_titleSceneName);
    }
}