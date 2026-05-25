using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyObject : MonoBehaviour
{
    DefaultEnemy m_defaultEnemy;
    Vector2Int m_currentAreaIndex;

    AreaManager m_areaManager;

    void Start()
    {
        transform.position = new Vector3Int(m_currentAreaIndex.x, 0, m_currentAreaIndex.y) * 10;
        m_areaManager = FindFirstObjectByType<AreaManager>();
    }

    public void Init(DefaultEnemy date)
    {
        m_defaultEnemy = date;
        name = date.name;
        GetComponent<BoxCollider>().size = new(date.m_size.x, date.m_size.y, 1);
        transform.Find("Visual").GetComponent<SpriteRenderer>().sprite = date.m_image;
        m_currentAreaIndex = date.m_spawnPosition;
    }

    public void MoveTarget(Vector2Int target)
    {
        MapData m_MapData = m_areaManager.GetMapData();

        AreaController area = m_MapData.GetArea(target);

        if (area == null){
            return; }


        if (!m_defaultEnemy.CanChaseTarget(m_currentAreaIndex, target)) return;

        // 最短距離で移動先
        Vector2Int move = new(0, 0);
        Vector2Int diff = target - m_currentAreaIndex;
        for (int i = 0; i < m_defaultEnemy.m_speed; i++)
        {
            Vector2Int diff_ = diff - move;
            if (diff_ == new Vector2Int(0, 0))
            {
                Debug.Log("hit : " + name);
                break;
            }

            Debug.Log("A");
            //if (Mathf.Abs(diff_.y) <= Mathf.Abs(diff_.x))
            //{
            //    if (m_MapData.GetArea(new(m_currentAreaIndex.x + 1, m_currentAreaIndex.y)).GetAreaName() != "syougai")
            //    {
            //        move += new Vector2Int(1, 0);   // 最後ここ
            //    }
            //}
            move += Mathf.Abs(diff_.y) <= Mathf.Abs(diff_.x) ? new Vector2Int(1, 0) : new Vector2Int(0, 1);
            Debug.Log("B");
        }

        // x方向に近づく
        m_currentAreaIndex.x += Math.Sign(diff.x) * move.x;

        // y方向に近づく
        m_currentAreaIndex.y += Math.Sign(diff.y) * move.y;

        transform.position = new Vector3Int(m_currentAreaIndex.x, 0, m_currentAreaIndex.y) * 10;
    }
}
