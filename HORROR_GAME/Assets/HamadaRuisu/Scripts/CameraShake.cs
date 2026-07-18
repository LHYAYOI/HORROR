using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instanse { get; private set; }
    private CinemachineBasicMultiChannelPerlin perlin;

    [SerializeField] float m_captureAmplitudeGain = 1f;
    [SerializeField] float m_captureFrequencyGain = 1f;

    private void Awake()
    {
        Instanse = this;
        perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void CaptureShake(float rate) 
    {
        rate = Mathf.Clamp01(rate);

        perlin.AmplitudeGain = Mathf.Lerp(0.7f, m_captureAmplitudeGain, rate);
        perlin.FrequencyGain = Mathf.Lerp(0.2f, m_captureFrequencyGain, rate);
    }
}
