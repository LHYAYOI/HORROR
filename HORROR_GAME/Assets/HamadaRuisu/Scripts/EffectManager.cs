using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectManager : MonoBehaviour
{
    [Header("画面のエフェクト")]
    [SerializeField] FullScreenPassRendererFeature m_cameraPostEffectFeature;
    [SerializeField] Material m_materialPostEffectMaterial;
    [SerializeField] Volume m_volume;

    [Header("ドリーカメラエフェクト")]
    [SerializeField] CinemachineCamera m_camera;
    [SerializeField] float m_cameraMaxFrontDistance;
    [SerializeField] float m_maxCameraFOV;
    [SerializeField] float m_rate;

    Vector3 m_cameraStartPosition;
    float m_startFOV;

    Tonemapping m_tonemap;
    Bloom m_bloom;
    ColorAdjustments m_colorAdjustments;
    ChromaticAberration m_chromaticAberration;

    private void Awake()
    {
        m_cameraStartPosition = Camera.main.transform.position;
        m_startFOV = m_camera.Lens.FieldOfView;
        m_volume.profile.TryGet(out m_tonemap);
        m_volume.profile.TryGet(out m_bloom);
        m_volume.profile.TryGet(out m_colorAdjustments);
        m_volume.profile.TryGet(out m_chromaticAberration);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManagerHamada.instance.Play("SnowNoise");
    }

    // Update is called once per frame
    void Update()
    {
        SetDolyCameraEffectRate(0);
    }

    //カメラのポストエフェクト
    public void SetCameraPostEffectFlag(bool flag)
    {
        m_cameraPostEffectFeature.SetActive(flag);

        m_tonemap.active = flag;
        m_bloom.active = flag;
        m_colorAdjustments.active = flag;
        m_chromaticAberration.active = flag;
    }

    public void SetDolyCameraEffectRate(float rate) 
    {
        rate = Mathf.Clamp01(rate);

        Vector3 frontPosition = m_cameraStartPosition + m_camera.transform.forward * m_cameraMaxFrontDistance;
        m_camera.transform.position = Vector3.Lerp(m_cameraStartPosition, frontPosition, rate);
        m_camera.Lens.FieldOfView = Mathf.Lerp(m_startFOV, m_maxCameraFOV, rate);
        CameraShake.Instanse.CaptureShake(rate);

        SnowNoiseEffect(rate);
    }

    public void SnowNoiseEffect(float rate) 
    {
        float maskIntensity = Mathf.Abs(rate - 0.5f) * 2f * 1.1f;
        int isSnowActive = rate >= 0.5f ? 1 : 0;

        AudioManagerHamada.instance.SetVolume("SnowNoise", rate);

        m_materialPostEffectMaterial.SetFloat("_MaskIntensity", maskIntensity);

        m_materialPostEffectMaterial.SetInt("_SnowNoise_On_Off", isSnowActive);
    }

    public void ResetFromCaptureEffect()
    {
        m_camera.transform.position = m_cameraStartPosition;
        m_camera.Lens.FieldOfView = m_startFOV;
        CameraShake.Instanse.CaptureShake(0);
    }
}
