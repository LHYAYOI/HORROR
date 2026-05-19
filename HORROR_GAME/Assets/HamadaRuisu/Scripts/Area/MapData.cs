using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] private int m_width = 1;
    [SerializeField] private int m_height = 1;

    [SerializeField] List<AreaController> m_areaData;

    public void Init()
    {
        m_areaData = new List<AreaController>(new AreaController[m_width * m_height]);
    }

    public AreaController GetArea(Vector2Int index)
    {
        if (index.x < 0 || index.x >= m_width || index.y < 0 || index.y >= m_height)
        {
            return null;
        }

        return m_areaData[index.y * m_width + index.x];
    }

    public int GetWidth()
    {
        return m_width;
    }

    public int GetHeight()
    {
        return m_height;
    }

    public void SetHeight(int height) 
    {
        m_height = height;
    }

    public void SetWidth(int width) 
    {
        m_width = width;
    }

    public void SetArea(Vector2Int index, AreaController data)
    {
        m_areaData[index.y * m_width + index.x] = data;
    }

    public void SetAreaData(List<AreaController> areaData) 
    {
        m_areaData = areaData;
    }
}
