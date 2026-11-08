using UnityEngine;
using UnityEngine.InputSystem;

public class SmartPhoneAnimation : MonoBehaviour
{
    [Header("Map&スマホ画面")]
    [SerializeField] GameObject m_uwasaSetUp;
    [SerializeField] Transform m_parent;

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
