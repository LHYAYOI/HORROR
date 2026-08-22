using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CURSOR_TYPE
{
    DEFAULT,
    INTERACTION_HIDE_SPOT,
    EXIT_HIDE,
    NONE
};

public class CursorManager : MonoBehaviour
{
    [Header("マウスカーソル用項目")]
    [SerializeField] Image m_cursorImage;
    [SerializeField] Animator m_cursorAnimator;
    [SerializeField] TextMeshProUGUI m_cursorText;
    [SerializeField] Vector3 m_defaultCursorScale;
    [SerializeField] Vector3 m_interactCursorScale;

    CURSOR_TYPE m_cursorType;

    public static CursorManager instance;

    private void Awake()
    {
        instance = this;
        Cursor.visible = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_cursorImage.rectTransform.position = Input.mousePosition;
    }

    public void EnableCrossHair()
    {
        m_cursorImage.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EnableCursor()
    {
        m_cursorImage.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        ChangeCursorImage(CURSOR_TYPE.DEFAULT);
    }

    public void ChangeCursorImage(CURSOR_TYPE type)
    {
        m_cursorType = type;

        switch (m_cursorType)
        {
            case CURSOR_TYPE.DEFAULT:
                {
                    m_cursorImage.rectTransform.localScale = m_defaultCursorScale;
                    m_cursorAnimator.SetTrigger("UnInteract");
                    break;
                }
            case CURSOR_TYPE.INTERACTION_HIDE_SPOT:
                {
                    m_cursorImage.rectTransform.localScale = m_interactCursorScale;
                    m_cursorAnimator.SetTrigger("Interact");
                    m_cursorText.text = "隠れる";
                    break;
                }

            case CURSOR_TYPE.EXIT_HIDE:
                {
                    m_cursorImage.rectTransform.localScale = m_interactCursorScale;
                    m_cursorAnimator.SetTrigger("Interact");
                    m_cursorText.text = "出る";
                    break;
                }
        }
    }
}
