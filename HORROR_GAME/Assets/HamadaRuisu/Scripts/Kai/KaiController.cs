using UnityEngine;

public class KaiControler : MonoBehaviour,IPressable
{
    [SerializeField] KaiView m_kaiView;
    PlayerController m_playerController;

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
            m_kaiView.Show();
        }
        else
        {
            m_kaiView.Hide();
        }
    }

    public void OnPressed()
    {
        Debug.Log("‰Ÿ‚³‚ê‚½");
    }

    public void OnReleased()
    {
        Debug.Log("—£‚µ‚½");
    }

}
