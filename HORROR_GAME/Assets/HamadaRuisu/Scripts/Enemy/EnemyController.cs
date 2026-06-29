using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyView m_enemyView;
    [SerializeField] PlayerController m_playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerController.GetCameraOnOffState() == PlayerController.CAMERA_ON_OFF_ATATE.ON) 
        {
            m_enemyView.Show();
        }
        else
        {
            m_enemyView.Hide();
        }
    }
}
