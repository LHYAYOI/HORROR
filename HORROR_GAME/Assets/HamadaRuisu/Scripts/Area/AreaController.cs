using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaController : MonoBehaviour
{
    [SerializeField] AreaModel m_model;

    public string GetAreaName() => m_model.GetAreaName();

    public Texture GetAreaSkyBoxTexture() => m_model.GetAreaSkyBoxTexture();

    public AREA_TYPE GetAreaType() => m_model.GetAreaType();

    public Vector2Int GetAreaPosition()
    {
        return m_model.GetAreaPosition();
    }

    public void SetAreaPosition(Vector2Int position) 
    {
        m_model.SetAreaPosition(position);
    } 

    public float GetAreaRotation()
    {
        return m_model.GetAreaRotation();
    }
}
