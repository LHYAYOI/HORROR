using UnityEngine;

public class KaiControler : MonoBehaviour
{
    [SerializeField] KaiView m_kaiView;
    [SerializeField] Collider m_collider;
    CameraController m_cameraController;

    bool m_wasShoted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_cameraController = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOnCamera() && !m_wasShoted)
        {
            m_kaiView.Show();
            m_collider.enabled = true;
        }
        else
        {
            m_kaiView.Hide();
            m_collider.enabled = false;
        }
    }

    public bool WasShot() 
    {
        return m_wasShoted;
    }

    public bool IsOnCamera() 
    {
        return m_cameraController.GetCameraOnOffState() == CAMERA_ON_OFF_STATE.ON;
    }

    public void Captured() 
    {
        m_wasShoted = true;

        m_collider.enabled = false;

        m_kaiView.Hide();

        GameManager.instance.CheckIfClearedGame();
    }

}
