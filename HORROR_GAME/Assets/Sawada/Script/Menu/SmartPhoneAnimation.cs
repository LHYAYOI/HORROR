using UnityEngine;
using UnityEngine.InputSystem;

public class SmartPhoneAnimation : MonoBehaviour
{
    [Header("Map&スマホ画面")]
    [SerializeField] GameObject m_uwasaSetUp;
    [SerializeField] Transform m_parent;
    [Header("Animator")]
    [SerializeField] private Animator m_animator;

    private bool m_isPulled = false;

    public void OnOffButton()
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

    public void OffButton()
    {
        m_animator.SetTrigger("RemoveSmartPhone");
        m_isPulled = false;
    }


    // SetUpの適応
    public void SetUp()
    {
        bool alreadyExists = false;

        for (int i = m_parent.childCount - 1; i >= 0; i--)
        {
            GameObject child = m_parent.GetChild(i).gameObject;

            // m_uwasaSetUp と同じPrefabなら消さない
            if (child.name == m_uwasaSetUp.name + "(Clone)")
            {
                alreadyExists = true;
                continue;
            }

            Destroy(child);
        }

        // すでに同じPrefabがあるなら生成しない
        if (!alreadyExists)
        {
            Instantiate(m_uwasaSetUp, m_parent);
            //Debug.Log(m_uwasaSetUp.name + " を生成しました");
        }
    }
}
