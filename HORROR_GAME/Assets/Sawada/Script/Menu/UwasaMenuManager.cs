using UnityEngine;
using UnityEngine.InputSystem;

public class UwasaMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject m_objects;
    private bool m_viewMode = false;
    [SerializeField] private Animator m_animator;

    private void Start()
    {
        SetView();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetView();
        }
    }

    // 表示非表示
    public void SetView()
    {
        // m_objectsの表示非表示
        m_viewMode = !m_viewMode;
        m_objects.SetActive(m_viewMode);

        // m_animatorの実行
        if (m_viewMode) m_animator.SetTrigger("PullSmartPhone");
        else m_animator.SetTrigger("RemoveSmartPhone");
    }
}
