using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] PlayerController m_PlayerController;
    [SerializeField] AreaManager m_AreaManager;

    Vector2Int m_currentAreaIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MapData m_MapData = m_AreaManager.GetMapData();

            Vector2Int upIndex = new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y - 1);

            AreaController area = m_MapData.GetArea(upIndex);

            if (area != null)
            {
                RenderSettings.skybox = area.GetAreaSkyBoxMaterial();
                m_currentAreaIndex = upIndex;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MapData m_MapData = m_AreaManager.GetMapData();

            Vector2Int downIndex = new Vector2Int(m_currentAreaIndex.x, m_currentAreaIndex.y + 1);

            AreaController area = m_MapData.GetArea(downIndex);

            if (area != null)
            {
                RenderSettings.skybox = area.GetAreaSkyBoxMaterial();
                m_currentAreaIndex = downIndex;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MapData m_MapData = m_AreaManager.GetMapData();

            Vector2Int rightIndex = new Vector2Int(m_currentAreaIndex.x + 1, m_currentAreaIndex.y);

            AreaController area = m_MapData.GetArea(rightIndex);

            if (area != null)
            {
                RenderSettings.skybox = area.GetAreaSkyBoxMaterial();
                m_currentAreaIndex = rightIndex;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MapData m_MapData = m_AreaManager.GetMapData();

            Vector2Int leftIndex = new Vector2Int(m_currentAreaIndex.x - 1, m_currentAreaIndex.y);

            AreaController area = m_MapData.GetArea(leftIndex);

            if (area != null)
            {
                RenderSettings.skybox = area.GetAreaSkyBoxMaterial();
                m_currentAreaIndex = leftIndex;
            }
        }
    }
}
