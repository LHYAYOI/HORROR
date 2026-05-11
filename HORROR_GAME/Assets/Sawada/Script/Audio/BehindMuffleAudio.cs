using UnityEngine;

public class BehindMuffleAudio : MonoBehaviour
{
    Camera m_listenerCamera;
    AudioLowPassFilter m_lowPassFilter;

    [Header("前方 周波数上限")]
    [SerializeField] float m_frontCutoffFrequency = 22000.0f;

    [Header("後方 周波数上限")]
    [SerializeField] float m_backCutoffFrequency = 3500.0f;

    [Header("切り替わりの滑らかさ")]
    [SerializeField] float m_changeSpeed = 8.0f;

    private void Start()
    {
        m_lowPassFilter = GetComponent<AudioLowPassFilter>();
        m_listenerCamera = Camera.main;
    }

    private void Update()
    {
        UpdateMuffle();
    }

    private void UpdateMuffle()
    {
        if (m_listenerCamera == null || m_lowPassFilter == null)
        {
            return;
        }

        Vector3 listenerPosition = m_listenerCamera.transform.position;
        Vector3 soundPosition = transform.position;

        Vector3 soundDirection = soundPosition - listenerPosition;
        soundDirection.y = 0.0f;

        if (soundDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        soundDirection.Normalize();

        Vector3 listenerForward = m_listenerCamera.transform.forward;
        listenerForward.y = 0.0f;
        listenerForward.Normalize();

        float frontValue = Vector3.Dot(listenerForward, soundDirection);

        float backRate = Mathf.InverseLerp(0.0f, -1.0f, frontValue);

        float targetCutoffFrequency = Mathf.Lerp(
            m_frontCutoffFrequency,
            m_backCutoffFrequency,
            backRate
        );

        m_lowPassFilter.cutoffFrequency = Mathf.Lerp(
            m_lowPassFilter.cutoffFrequency,
            targetCutoffFrequency,
            Time.deltaTime * m_changeSpeed
        );
    }
}
