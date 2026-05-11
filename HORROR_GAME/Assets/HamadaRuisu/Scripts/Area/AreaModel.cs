using UnityEngine;

public class AreaModel : MonoBehaviour
{
    [SerializeField] string m_areaName;
    [SerializeField] Material m_skyboxMaterial;

    public string GetAreaName() => m_areaName;
    public Material GetAreaSkyBoxMaterial() => m_skyboxMaterial;
}
