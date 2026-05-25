using UnityEngine;

public class AreaModel : MonoBehaviour
{
    [SerializeField] string m_areaName;
    [SerializeField] Texture m_skyboxTexture;

    public string GetAreaName() => m_areaName;
    public Texture GetAreaSkyBoxTexture() => m_skyboxTexture;
}
