using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
   private int width = 1;
   private int height = 1;

    List<AreaData> m_areaData;

    public void Init()
    {
        m_areaData = new List<AreaData>(new AreaData[width * height]);
    }

    public void Resize(int newWidth, int newHeight)
    {
        // 新しいサイズのリストを作成
        List<AreaData> newData = new List<AreaData>(new AreaData[newWidth * newHeight]);

        // 既にあるデータを新しいリストにコピー（既に作ってたものを消さないため）
        for (int y = 0; y < Mathf.Min(height, newHeight); y++)
        {
            for (int x = 0; x < Mathf.Min(width, newWidth); x++)
            {
                newData[y * newWidth + x] = m_areaData[y * width + x];
            }
        }

        // サイズ更新
        width = newWidth;
        height = newHeight;
        m_areaData = newData;
    }

    public AreaData GetArea(int x, int y)
    {
        return m_areaData[y * width + x];
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void SetArea(int x, int y, AreaData data)
    {
        if (data == null)
        {
            return;
        }

        m_areaData[y * width + x] = data;
    }

    //横幅増やす
    public void AddWidth()
    {
        width++;
    }

    //立幅増やす
    public void AddHeight()
    {
        height++;
    }

}
