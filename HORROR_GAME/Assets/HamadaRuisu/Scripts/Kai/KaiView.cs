using UnityEngine;

public class KaiView : MonoBehaviour
{
    [SerializeField] GameObject m_kaiVisual;

    public void Show()
    {
        m_kaiVisual.SetActive(true);
    }

    public void Hide()
    {
        m_kaiVisual.SetActive(false);
    }
}
