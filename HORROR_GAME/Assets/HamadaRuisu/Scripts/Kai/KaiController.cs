using UnityEngine;

public class KaiController : MonoBehaviour
{
    [SerializeField] KaiView m_kaiView;
    [SerializeField] PlayerController m_playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerController.GetCameraOnOffState() == PlayerController.CAMERA_ON_OFF_ATATE.ON) 
        {
            m_kaiView.Show();
        }
        else
        {
            m_kaiView.Hide();
        }
    }
}
