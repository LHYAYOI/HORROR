using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyObject : MonoBehaviour
{
    DefaultEnemy m_defaultEnemy;
    Vector2Int m_currentAreaIndex;
    [SerializeField] Vector2Int m_lastTarget;

    AreaManager m_areaManager;
    [SerializeField] Vector2Int m_targetPosition;
    StateMachine m_playerState;
    PlayerController m_playerController;
    bool m_checkHidePositionFlag = false;


    void Start()
    {
        transform.position = new Vector3Int(m_currentAreaIndex.x, 0, m_currentAreaIndex.y) * 10;
        m_areaManager = FindFirstObjectByType<AreaManager>();
        m_playerState = GameObject.Find("Player").GetComponent<StateMachine>();
        m_playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void Init(DefaultEnemy date)
    {
        m_defaultEnemy = date;
        name = date.name;
        GetComponent<BoxCollider>().size = new(date.GetSize().x, date.GetSize().y, 1);
        transform.Find("Visual").GetComponent<SpriteRenderer>().sprite = date.GetImage();
        m_currentAreaIndex = date.GetSpawnPosition();
    }

    public void MoveEnemy(Vector2Int target)
    {
        // 最短距離で移動先
        Vector2Int move = new(0, 0);
        Vector2Int diff = target - m_currentAreaIndex;
        for (int i = 0; i < m_defaultEnemy.GetSpeed(); i++)
        {
            Vector2Int diff_ = diff - move;
            if (diff_ == new Vector2Int(0, 0))
            {
                m_checkHidePositionFlag = true;
                // 当たってるのか
                if (m_playerState.GetCurrentState().name != "HideState")
                    Debug.Log("hit : " + name);
                break;
            }

            move += Mathf.Abs(diff_.y) <= Mathf.Abs(diff_.x) ? new Vector2Int(1, 0) : new Vector2Int(0, 1);
        }

        // x方向に近づく
        m_currentAreaIndex.x += Math.Sign(diff.x) * move.x;

        // y方向に近づく
        m_currentAreaIndex.y += Math.Sign(diff.y) * move.y;

        transform.position = new Vector3Int(m_currentAreaIndex.x, 0, m_currentAreaIndex.y) * 10;
    }

    public void MoveEnemy_02()
    {
        // 最短距離で移動先
        Vector2Int move = new(0, 0);
        Vector2Int diff = m_targetPosition - m_currentAreaIndex;
        for (int i = 0; i < m_defaultEnemy.GetSpeed(); i++)
        {
            Vector2Int diff_ = diff - move;
            if (diff_ == new Vector2Int(0, 0))
            {
                m_checkHidePositionFlag = true;
                // 当たってるのか
                if (m_playerState.GetCurrentState().name != "HideState")
                    Debug.Log("hit : " + name);
                break;
            }

            move += Mathf.Abs(diff_.y) <= Mathf.Abs(diff_.x) ? new Vector2Int(1, 0) : new Vector2Int(0, 1);
        }

        // x方向に近づく
        m_currentAreaIndex.x += Math.Sign(diff.x) * move.x;

        // y方向に近づく
        m_currentAreaIndex.y += Math.Sign(diff.y) * move.y;

        transform.position = new Vector3Int(m_currentAreaIndex.x, 0, m_currentAreaIndex.y) * 10;
    }


    // 自動で判定
    public void AutoMove()
    {
        Debug.Log("動いた");
        MapData mapData = m_areaManager.GetMapData();

        // m_targetPositionの更新
        if (m_playerState.GetCurrentState().name == "HideState")
        {
            // まだ最後の位置に到達していない
            if (!m_checkHidePositionFlag)
            {
                if (true)
                {
                    m_checkHidePositionFlag = true;
                }
            }
            // 到達済み
            else
            {
                // 次の位置決め
                //NextPositionDecide();
            }
        }
        else
        {
            // 次の位置決め
            NextPositionDecide();
        }

        AreaController area = mapData.GetArea(m_targetPosition);

        if (area == null) return;

        if (!m_defaultEnemy.CanChaseTarget(m_currentAreaIndex, m_targetPosition)) return;
        
        // 移動
        MoveEnemy(m_targetPosition);
    }

    // Playerが視界にいるのか?
    private bool CheckPlayerInSight()
    {
        return m_defaultEnemy.CanChaseTarget(m_currentAreaIndex, m_playerController.GetCurrentIndex());
    }
    // posiが侵入可能か否か
    private bool CheckEnemyMovePosition(Vector2Int posi)
    {
        MapData mapData = m_areaManager.GetMapData();
        foreach (AreaController area in m_defaultEnemy.GetNoAreaData())
        {
            if (area == mapData.GetArea(posi))
            {
                return false;
            }
        }

        return true;
    }
    // 位置決め
    public void NextPositionDecide()
    {
        Debug.Log("視認 : " + CheckPlayerInSight());
        // Playerを視認している
        if (CheckPlayerInSight())
        {
            // Areaが行動可能か 未定
            if (CheckEnemyMovePosition(m_playerController.GetCurrentIndex()))
            {

            }
            m_targetPosition = m_playerController.GetCurrentIndex();
        }
        // ノット視認
        else
        {
            // ランダムな位置に設定
            MapData mapData = m_areaManager.GetMapData();
            Vector2Int nextPosition = new(UnityEngine.Random.Range(0, mapData.GetWidth()), UnityEngine.Random.Range(0, mapData.GetHeight()));
            int loopNum = 0;
            while (true)
            {
                // Areaが行動可能か
                if (CheckEnemyMovePosition(nextPosition))
                {
                    m_targetPosition = nextPosition;
                    break;
                }
                else
                {
                    // 探したけどない場合 未定
                    if (loopNum >= mapData.GetWidth() * mapData.GetWidth())
                    {
                        
                    }
                }

                // 横にずらす
                nextPosition = new(nextPosition.x % mapData.GetWidth(), nextPosition.x / mapData.GetWidth());

                loopNum++;
            }

        }
        Debug.Log("移動先 : " + m_targetPosition);
    }
}
