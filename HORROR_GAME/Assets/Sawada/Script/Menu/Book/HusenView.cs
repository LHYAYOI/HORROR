using UnityEngine;

public class HusenView : MonoBehaviour
{
    [SerializeField] GameObject m_complete;

    public void ShowComplete()
    {
        m_complete.SetActive(true);
    }
}
