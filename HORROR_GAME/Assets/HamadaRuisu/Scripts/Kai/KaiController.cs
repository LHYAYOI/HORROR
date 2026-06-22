using UnityEngine;

public class KaiControler : MonoBehaviour,IPressable
{
    [SerializeField] KaiView m_kaiView;
    PlayerController m_playerController;

    bool m_wasShoted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOnCamera())
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
        m_wasShoted = true;

        Debug.Log("‰Ÿ‚³‚ê‚½");
    }

    public void OnReleased()
    {
        Debug.Log("—£‚µ‚½");
    }

    public bool WasShot() 
    {
        return m_wasShoted;
    }

    public bool IsOnCamera() 
    {
        return m_playerController.GetCameraOnOffState() == PlayerController.CAMERA_ON_OFF_ATATE.ON;
    }

}
