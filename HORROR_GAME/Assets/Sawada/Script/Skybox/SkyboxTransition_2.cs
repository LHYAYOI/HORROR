using System.Collections;
using UnityEngine;

public class SkyboxTransition_2 : MonoBehaviour
{
    [Header("Panorama Textures")]
    [SerializeField] private Texture m_currentTexture;
    [SerializeField] private Texture m_nextTexture;

    [Header("Blend Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float m_blend = 0f;

    [Range(0f, 8f)]
    [SerializeField] private float m_exposure = 1f;

    private bool m_updateSkyBoxFlag;
    [Range(0f, 1f)]
    [SerializeField] private float m_transitionSpeed;

    [Header("Move Effect")]

    [Range(0f, 0.2f)]
    [SerializeField] private float m_moveStrength = 0.05f;

    [Range(0f, 0.5f)]
    [SerializeField] private float m_zoomStrength = 0.15f;
    
    [Range(0f, 0.5f)]
    [SerializeField] private float m_tex2StartZoomOut = 0.12f;

    [Header("Target")]
    [SerializeField] private Material m_targetSkyboxMaterial;
    [SerializeField] private bool m_applyToSceneSkyboxFlag = true;
    private Camera m_targetCamera;


    private static readonly int m_tex1Id = Shader.PropertyToID("_Tex1");
    private static readonly int m_tex2ID = Shader.PropertyToID("_Tex2");
    private static readonly int m_blendID = Shader.PropertyToID("_Blend");
    private static readonly int m_exposureID = Shader.PropertyToID("_Exposure");

    private static readonly int m_moveStrengthID = Shader.PropertyToID("_MoveStrength");
    private static readonly int m_zoomStrengthID = Shader.PropertyToID("_ZoomStrength");
    private static readonly int m_tex2StartZoomOutID = Shader.PropertyToID("_Tex2StartZoomOut");

    private static readonly int m_focusUID = Shader.PropertyToID("_FocusU");
    private static readonly int m_focusVID = Shader.PropertyToID("_FocusV");

    private static readonly int m_seamUID = Shader.PropertyToID("_SeamU");
    private static readonly int m_viewCenterUID = Shader.PropertyToID("_ViewCenterU");
    private static readonly int m_viewCenterVID = Shader.PropertyToID("_ViewCenterV");

    private void OnValidate()
    {
        ApplySkyboxTransition();
    }

    private void Start()
    {
        m_targetCamera = Camera.main;

        m_updateSkyBoxFlag = false;
    }
    private void Update()
    {
        if (m_updateSkyBoxFlag)
        {
            // blend値更新
            m_blend += m_transitionSpeed;
            if (1.0f < m_blend)
            {
                m_currentTexture = m_nextTexture;
                m_nextTexture = null;
                m_blend = 0.0f;
                m_updateSkyBoxFlag = false;
            }

            // 反映
            ApplySkyboxTransition();
        }
    }

    public void SetNextTexture(Texture next)
    {
        Debug.Log(next);
        if (m_currentTexture != null) m_nextTexture = next;
        else m_nextTexture = m_currentTexture;
        m_updateSkyBoxFlag = true;
        ApplySkyboxTransition();
    }

    private void ApplySkyboxTransition()
    {
        if (m_targetSkyboxMaterial == null)
            return;

        if (m_targetCamera == null)
            m_targetCamera = Camera.main;

        m_targetSkyboxMaterial.SetTexture(m_tex1Id, m_currentTexture);
        m_targetSkyboxMaterial.SetTexture(m_tex2ID, m_nextTexture);

        m_targetSkyboxMaterial.SetFloat(m_blendID, m_blend);
        m_targetSkyboxMaterial.SetFloat(m_exposureID, m_exposure);

        m_targetSkyboxMaterial.SetFloat(m_moveStrengthID, m_moveStrength);
        m_targetSkyboxMaterial.SetFloat(m_zoomStrengthID, m_zoomStrength);
        m_targetSkyboxMaterial.SetFloat(m_tex2StartZoomOutID, m_tex2StartZoomOut);

        //SetSeamAndViewCenterFromCamera();
        SetMoveDirectionFromCamera();

        if (m_applyToSceneSkyboxFlag)
        {
            RenderSettings.skybox = m_targetSkyboxMaterial;
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

        m_targetSkyboxMaterial.SetFloat(m_focusUID, u);
        m_targetSkyboxMaterial.SetFloat(m_focusVID, v);

        // 既存の移動方向も一応残す
        Vector2 move = new Vector2(forward.x, forward.y);

        if (move.sqrMagnitude > 1f)
            move.Normalize();
    }

    // skyboxの向きを変更
    private void SetSeamAndViewCenterFromCamera()
    {
        if (m_targetCamera == null || m_targetSkyboxMaterial == null)
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
        m_targetSkyboxMaterial.SetFloat(m_seamUID, seamU);

        // -----------------------------
        // 現在見えている景色を変えないため、
        // カメラが今見ている中心UVも渡す
        // こちらは上下方向も含める
        // -----------------------------
        Vector3 fullForward = m_targetCamera.transform.forward.normalized;

        float viewCenterU = Mathf.Atan2(fullForward.x, fullForward.z) / (2f * Mathf.PI) + 0.5f;
        float viewCenterV = Mathf.Asin(Mathf.Clamp(fullForward.y, -1f, 1f)) / Mathf.PI + 0.5f;

        m_targetSkyboxMaterial.SetFloat(m_viewCenterUID, viewCenterU);
        m_targetSkyboxMaterial.SetFloat(m_viewCenterVID, viewCenterV);
    }
}