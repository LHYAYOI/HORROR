using UnityEngine;

public enum AREA_TYPE
{
    PASSABLE,
    WALL
}

public class AreaModel : MonoBehaviour
{
    [SerializeField] AREA_TYPE m_areaType;
    [SerializeField] string m_areaName;
    [SerializeField] Texture m_skyboxTexture;

    private Vector2Int m_areaPosition;

    public string GetAreaName() => m_areaName;
    public Texture GetAreaSkyBoxTexture() => m_skyboxTexture;
    public AREA_TYPE GetAreaType() => m_areaType;

    public Vector2Int GetAreaPosition() 
    {
        return m_areaPosition;
    }

    public void SetAreaPosition(Vector2Int position) 
    {
        m_areaPosition = position;
    }
}
