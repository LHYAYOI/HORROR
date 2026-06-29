using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyView m_enemyView;
    [SerializeField] CameraController m_cameraController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_cameraController.GetCameraOnOffState() == CAMERA_ON_OFF_STATE.ON) 
        {
            m_enemyView.Show();
        }
        else
        {
            m_enemyView.Hide();
        }
        m_enemyView.Show();
    }
}
