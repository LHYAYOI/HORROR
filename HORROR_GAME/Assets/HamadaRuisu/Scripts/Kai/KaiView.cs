using UnityEngine;

public class KaiView : MonoBehaviour
{
    [SerializeField] GameObject m_enemyVisual;

    public void Show()
    {
        m_enemyVisual.SetActive(true);
    }

    public void Hide()
    {
        m_enemyVisual.SetActive(false);
    }
}
