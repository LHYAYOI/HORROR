using System.Collections;
using UnityEngine;

public class SkyboxTransition : MonoBehaviour
{
    [Header("Panorama Textures")]
    [SerializeField] private Texture panoramaA;
    [SerializeField] private Texture panoramaB;

    [Header("Blend Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float blend = 0f;

    [Range(0f, 8f)]
    [SerializeField] private float exposure = 1f;

    [Header("Target")]
    [SerializeField] private Material targetSkyboxMaterial;

    [Tooltip("ONにするとRenderSettings.skyboxにも設定します")]
    [SerializeField] private bool applyToSceneSkybox = true;

    private static readonly int Tex1ID = Shader.PropertyToID("_Tex1");
    private static readonly int Tex2ID = Shader.PropertyToID("_Tex2");
    private static readonly int BlendID = Shader.PropertyToID("_Blend");
    private static readonly int ExposureID = Shader.PropertyToID("_Exposure");

    private void OnValidate()
    {
        Apply();
    }

    private void Update()
    {
        //Apply();
    }

    private void Apply()
    {
        if (targetSkyboxMaterial == null)
            return;

        if (panoramaA != null)
            targetSkyboxMaterial.SetTexture(Tex1ID, panoramaA);

        if (panoramaB != null)
            targetSkyboxMaterial.SetTexture(Tex2ID, panoramaB);

        targetSkyboxMaterial.SetFloat(BlendID, blend);
        targetSkyboxMaterial.SetFloat(ExposureID, exposure);

        if (applyToSceneSkybox)
        {
            RenderSettings.skybox = targetSkyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }
}