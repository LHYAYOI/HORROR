using UnityEngine;
using UnityEngine.UI;

public enum CURSOR_TYPE
{
    DEFAULT,
    INTERACTION,
    NONE
};

public enum CROSSHAIR_TYPE
{
    DEFAULT,
    INTERACTION,
    NONE
};

public class CursorManager : MonoBehaviour
{
    [Header("マウスカーソル用項目")]
    [SerializeField] Image m_cursorImage;
    [SerializeField] Sprite m_defaultImage;
    [SerializeField] Sprite m_interactImage;

    [Header("クロスヘア用項目")]
    [SerializeField] Image m_crossHairImage;
    [SerializeField] Sprite m_defaultCrossHairSprite;
    [SerializeField] Sprite m_interactCrossHairSprite;

    CURSOR_TYPE m_cursorType;
    CROSSHAIR_TYPE m_crosshair;

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
        //m_crossHairImage.enabled = true;
        m_cursorImage.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        //ChangeCrossHairImage(CROSSHAIR_TYPE.DEFAULT);
    }

    public void ChangeCrossHairImage(CROSSHAIR_TYPE type)
    {
        m_crosshair = type;

        switch (m_crosshair)
        {
            case CROSSHAIR_TYPE.DEFAULT:
                {
                    m_crossHairImage.sprite = m_defaultCrossHairSprite;
                    break;
                }

            case CROSSHAIR_TYPE.INTERACTION:
                {
                    m_crossHairImage.sprite = m_interactCrossHairSprite;
                    break;
                }

            case CROSSHAIR_TYPE:
                {
                    m_crossHairImage.sprite = null;
                    break;
                }
        }
    }

    public void EnableCursor()
    {
        m_cursorImage.enabled = true;
        m_crossHairImage.enabled = false;
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
                    m_cursorImage.sprite = m_defaultImage;
                    break;
                }
            case CURSOR_TYPE.INTERACTION:
                {
                    m_cursorImage.sprite = m_interactImage;
                    break;
                }
        }
    }
}
