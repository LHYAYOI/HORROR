using UnityEditor;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    [SerializeField] MapData m_MapData;
    [SerializeField] PlayerController m_PlayerController;

    private void Start()
    {
        for (int y = 0; y < m_MapData.GetHeight(); y++)
        {
            for (int x = 0; x < m_MapData.GetWidth(); x++)
            {
                AreaController area = m_MapData.GetArea(new Vector2Int(x, y));

                if (area == null) 
                {
                    continue;
                }

                if (area.GetAreaPosition() != m_PlayerController.GetCurrentIndex()) 
                {
                    
                    area.gameObject.SetActive(false);
                }
            }
        }
    }

    public MapData GetMapData()
    {
        return m_MapData;
    }
}
