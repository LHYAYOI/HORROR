using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class testAstar : MonoBehaviour
{
    class testCell
    {
        internal int step = -1;              // 進んだ回数
        internal int distance = -1;          // 現在地から目的地までの距離
        internal int weight = -1;            // 合計値
        internal Vector2Int previous = new(0, 0);   // 前のセルの位置
    }

    AreaManager m_areaManager;

    testCell[][] m_testAstar;


    private void Awake()
    {
        m_areaManager = FindFirstObjectByType<AreaManager>();
    }

    void Start()
    {
        if (!m_areaManager) return;

        // Mapの大きさ
        m_testAstar = new testCell[m_areaManager.GetMapData().GetWidth()][];
        for (int i = 0; i < m_areaManager.GetMapData().GetWidth(); i++)
        {
            m_testAstar[i] = new testCell[m_areaManager.GetMapData().GetHeight()];
        }
    }

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            CalculateShortestPath(new(0, 0), new(4, 3));
        }
    }

    // 初期化
    private void InitializeTestAstar()
    {
        for (int x = 0; x < m_areaManager.GetMapData().GetWidth(); x++)
        {
            for (int y = 0; y < m_areaManager.GetMapData().GetHeight(); y++)
            {
                m_testAstar[x][y] = new();
            }
        }
    }

    // 最短経路を計算する
    private List<Vector2Int> CalculateShortestPath(Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> result = new();
        InitializeTestAstar();

        //testAA(start);




        return result;
    }

    private void testAA(Vector2Int currentPosi, Vector2Int goal)
    {
        // 隣接セル
        List<testCell> neighbors = new();
        if(SetupCell(new(currentPosi.x + 1, currentPosi.y), goal, currentPosi)) neighbors.Add(m_testAstar[currentPosi.x + 1][currentPosi.y]);
        if(SetupCell(new(currentPosi.x - 1, currentPosi.y), goal, currentPosi)) neighbors.Add(m_testAstar[currentPosi.x - 1][currentPosi.y]);
        if(SetupCell(new(currentPosi.x, currentPosi.y + 1), goal, currentPosi)) neighbors.Add(m_testAstar[currentPosi.x][currentPosi.y + 1]);
        if(SetupCell(new(currentPosi.x, currentPosi.y - 1), goal, currentPosi)) neighbors.Add(m_testAstar[currentPosi.x][currentPosi.y - 1]);
        // 隣接するセルのweightが小さい順に並べる
        for (int i = 0; i < neighbors.Count - 1; i++)
        {
            if (neighbors[i + 1].weight < neighbors[i].weight)
            {
                testCell testCell = neighbors[i];// ここから
                neighbors[i] = neighbors[i + 1];
                neighbors[i + 1] = testCell;
                i = -1;
                continue;
            }
        }

    }


    // 指定セルをセットアップする
    private bool SetupCell(Vector2Int targetCell, Vector2Int goalCell, Vector2Int currentCell)
    {
        // 既にセットアップされていたら
        if (m_testAstar[targetCell.x][targetCell.y].step == -1) return false;

        m_testAstar[targetCell.x][targetCell.y].step = m_testAstar[currentCell.x][currentCell.y].step + 1;
        m_testAstar[targetCell.x][targetCell.y].distance = Mathf.Abs(targetCell.x - goalCell.x) + Mathf.Abs(targetCell.y - goalCell.y);
        m_testAstar[targetCell.x][targetCell.y].weight = m_testAstar[targetCell.x][targetCell.y].step + m_testAstar[targetCell.x][targetCell.y].distance;
        m_testAstar[targetCell.x][targetCell.y].previous = currentCell;

        return true;
    }
}
