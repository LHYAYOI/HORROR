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

    //public void Resize(int newWidth, int newHeight)
    //{
    //    新しいサイズのリストを作成
    //    List<AreaController> newData = new List<AreaController>(new AreaController[newWidth * newHeight]);

    //    既にあるデータを新しいリストにコピー（既に作ってたものを消さないため）
    //    for (int y = 0; y < Mathf.Min(m_height, newHeight); y++)
    //    {
    //        for (int x = 0; x < Mathf.Min(m_width, newWidth); x++)
    //        {
    //            newData[y * newWidth + x] = m_areaData[y * m_width + x];
    //        }
    //    }

    //    サイズ更新
    //   m_width = newWidth;
    //    m_height = newHeight;
    //    m_areaData = newData;
    //}

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
        if (data == null)
        {
            return;
        }

        m_areaData[index.y * m_width + index.x] = data;
    }

    public void SetAreaData(List<AreaController> areaData) 
    {
        m_areaData = areaData;
    }

    public void DebugArea(Vector2Int index)
    {
        AreaController data = GetArea(index);

        if (data == null)
        {
            Debug.Log("この要素には何もありません");
            return;
        }

        Debug.Log($"Area at ({index.x}, {index.y}): {data.GetAreaName()}");
    }
}
