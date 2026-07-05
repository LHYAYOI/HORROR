using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CaptureAreaController : MonoBehaviour
{
    [SerializeField] List<KaiControler> m_capturableKaiList;
    [SerializeField] GameObject m_visual;
    CameraController m_cameraController;

    float m_captureDistance;

    bool m_captured = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_cameraController = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraController>();

        m_captureDistance = m_visual.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOnCamera() && !m_captured)
        {
            m_visual.SetActive(true);
        }
        else
        {
            m_visual.SetActive(false);
        }
    }

    public float GetCaptureDistance()
    {
        return m_captureDistance;
    }

    public void ConcludeCapture() 
    {
        foreach (KaiControler kai in m_capturableKaiList) 
        {
            if (kai == null) 
            {
                continue;
            }

            kai.Captured();
        }

        m_captured = true;

        gameObject.SetActive(false);
    }

    public bool IsCaptured()
    {
        return m_captured;
    }

    public bool IsOnCamera()
    {
        return m_cameraController.GetCameraOnOffState() == CAMERA_ON_OFF_STATE.ON;
    }
}
