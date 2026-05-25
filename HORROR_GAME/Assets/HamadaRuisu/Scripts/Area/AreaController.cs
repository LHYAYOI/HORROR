using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaController : MonoBehaviour
{
    [SerializeField] AreaModel m_model;

    public string GetAreaName() => m_model.GetAreaName();

    public Texture GetAreaSkyBoxTexture() => m_model.GetAreaSkyBoxTexture();
}
