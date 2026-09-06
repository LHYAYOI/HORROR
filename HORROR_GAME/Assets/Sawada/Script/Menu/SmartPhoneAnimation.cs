using UnityEngine;
using UnityEngine.InputSystem;

public class SmartPhoneAnimation : MonoBehaviour
{
    [SerializeField] private Animator m_animator;

    private bool m_isPulled = false;

    public void OnButton()
    {
        // スマホを出す
        if (!m_isPulled)
        {
            m_animator.SetTrigger("PullSmartPhone");
            m_isPulled = true;
        }
        // スマホをしまう
        else
        {
            m_animator.SetTrigger("RemoveSmartPhone");
            m_isPulled = false;
        }
    }
}
