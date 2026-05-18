using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [SerializeField] MapData m_MapData;

    private Vector2Int m_currentAreaIndex = Vector2Int.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
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

            Vector2Int leftIndex = new Vector2Int(m_currentAreaIndex.x - 1, m_currentAreaIndex.y);

            AreaController area = m_MapData.GetArea(leftIndex);

            if (area != null)
            {
                RenderSettings.skybox = area.GetAreaSkyBoxMaterial();
                m_currentAreaIndex = leftIndex;
            }
        }
    }

    public MapData GetMapData()
    {
        return m_MapData;
    }
}
