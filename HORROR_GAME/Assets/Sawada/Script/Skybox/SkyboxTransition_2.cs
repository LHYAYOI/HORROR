using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SkyboxTransition_2 : MonoBehaviour
{
    [Header("Panorama Textures")]
    [SerializeField] private Texture m_panoramaA;
    [SerializeField] private Texture panoramaB;

    [Header("Blend Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float blend = 0f;

    [Range(0f, 8f)]
    [SerializeField] private float exposure = 1f;

    [Header("Move Effect")]
    Camera m_targetCamera;

    [Range(0f, 0.2f)]
    [SerializeField] private float moveStrength = 0.05f;

    [Range(0f, 0.5f)]
    [SerializeField] private float zoomStrength = 0.15f;
    
    [Range(0f, 0.5f)]
    [SerializeField] private float tex2StartZoomOut = 0.12f;

    [Header("Target")]
    [SerializeField] private Material targetSkyboxMaterial;
    [SerializeField] private bool m_applyToSceneSkyboxFlag = true;

    private static readonly int m_tex1Id = Shader.PropertyToID("_Tex1");
    private static readonly int Tex2ID = Shader.PropertyToID("_Tex2");
    private static readonly int BlendID = Shader.PropertyToID("_Blend");
    private static readonly int ExposureID = Shader.PropertyToID("_Exposure");

    private static readonly int MoveStrengthID = Shader.PropertyToID("_MoveStrength");
    private static readonly int ZoomStrengthID = Shader.PropertyToID("_ZoomStrength");
    private static readonly int Tex2StartZoomOutID = Shader.PropertyToID("_Tex2StartZoomOut");

    private static readonly int FocusUID = Shader.PropertyToID("_FocusU");
    private static readonly int FocusVID = Shader.PropertyToID("_FocusV");

    private static readonly int SeamUID = Shader.PropertyToID("_SeamU");
    private static readonly int ViewCenterUID = Shader.PropertyToID("_ViewCenterU");
    private static readonly int ViewCenterVID = Shader.PropertyToID("_ViewCenterV");

    private void OnValidate()
    {
        ApplySkyboxTransition();
    }

    private void Start()
    {
        m_targetCamera = Camera.main;
    }
    private void Update()
    {
        //Apply();
        //ApplySkyboxTransition();
    }

    private void ApplySkyboxTransition()
    {
        if (targetSkyboxMaterial == null)
            return;

        if (m_targetCamera == null)
            m_targetCamera = Camera.main;

        targetSkyboxMaterial.SetTexture(m_tex1Id, m_panoramaA);
        targetSkyboxMaterial.SetTexture(Tex2ID, panoramaB);

        targetSkyboxMaterial.SetFloat(BlendID, blend);
        targetSkyboxMaterial.SetFloat(ExposureID, exposure);

        targetSkyboxMaterial.SetFloat(MoveStrengthID, moveStrength);
        targetSkyboxMaterial.SetFloat(ZoomStrengthID, zoomStrength);
        targetSkyboxMaterial.SetFloat(Tex2StartZoomOutID, tex2StartZoomOut);

        //SetSeamAndViewCenterFromCamera();
        SetMoveDirectionFromCamera();

        if (m_applyToSceneSkyboxFlag)
        {
            RenderSettings.skybox = targetSkyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }

    private void SetMoveDirectionFromCamera()
    {
        if (m_targetCamera == null)
            return;

        Vector3 forward = m_targetCamera.transform.forward.normalized;

        // カメラの向きをPanorama用UVに変換
        float longitude = Mathf.Atan2(forward.x, forward.z);
        float latitude = Mathf.Asin(forward.y);

        float u = longitude / (2f * Mathf.PI) + 0.5f;
        float v = latitude / Mathf.PI + 0.5f;

        targetSkyboxMaterial.SetFloat(FocusUID, u);
        targetSkyboxMaterial.SetFloat(FocusVID, v);

        // 既存の移動方向も一応残す
        Vector2 move = new Vector2(forward.x, forward.y);

        if (move.sqrMagnitude > 1f)
            move.Normalize();
    }

    public IEnumerator BlendRoutine(Texture from, Texture to)
    {
        m_panoramaA = from;

        panoramaB = to;

        float elapsedTime = 0.0f;

        while (elapsedTime < 0.5f) 
        {
            elapsedTime += Time.deltaTime;

            float rate = elapsedTime / 0.5f;

            blend = rate;

            yield return null;
        }

        m_panoramaA = to;

        panoramaB = null;

        //blend = 0;
    }

    public void SetBlend(float rate) 
    {
        blend = rate;
    }

    public void SetBlendingTextures(Texture from, Texture to) 
    {
        m_panoramaA = from;
        panoramaB = to;
    }

    // skyboxの向きを変更
    private void SetSeamAndViewCenterFromCamera()
    {
        if (m_targetCamera == null || targetSkyboxMaterial == null)
            return;

        // -----------------------------
        // seam は「カメラの真後ろ」に置く
        // ここは水平回転だけを使う
        // -----------------------------
        Vector3 flatForward = m_targetCamera.transform.forward;
        flatForward.y = 0f;

        if (flatForward.sqrMagnitude < 0.0001f)
            flatForward = Vector3.forward;

        flatForward.Normalize();

        float forwardUFlat = Mathf.Atan2(flatForward.x, flatForward.z) / (2f * Mathf.PI) + 0.5f;

        // forward の反対側 = seam
        float seamU = Mathf.Repeat(forwardUFlat + 0.5f, 1f);
        targetSkyboxMaterial.SetFloat(SeamUID, seamU);

        // -----------------------------
        // 現在見えている景色を変えないため、
        // カメラが今見ている中心UVも渡す
        // こちらは上下方向も含める
        // -----------------------------
        Vector3 fullForward = m_targetCamera.transform.forward.normalized;

        float viewCenterU = Mathf.Atan2(fullForward.x, fullForward.z) / (2f * Mathf.PI) + 0.5f;
        float viewCenterV = Mathf.Asin(Mathf.Clamp(fullForward.y, -1f, 1f)) / Mathf.PI + 0.5f;

        targetSkyboxMaterial.SetFloat(ViewCenterUID, viewCenterU);
        targetSkyboxMaterial.SetFloat(ViewCenterVID, viewCenterV);
    }
}