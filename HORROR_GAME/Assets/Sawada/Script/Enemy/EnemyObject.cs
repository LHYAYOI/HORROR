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
    }

    public void Init(DefaultEnemy date)
    {
        m_playerState = GameObject.Find("Player").GetComponent<StateMachine>();
        m_playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        m_areaManager = FindFirstObjectByType<AreaManager>();

        m_defaultEnemy = date;
        name = date.name;
        m_currentAreaIndex = date.GetSpawnPosition();

        SetPositionFromPlayer();

        transform.Find("Visual").GetComponent<SpriteRenderer>().sprite = date.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Sprite;
    }

    Vector3 GetAdjustmentFromPlayer()
    {
        Vector3 difference = transform.position - m_playerController.gameObject.transform.position;
        difference.y = 0f;

        if (difference.sqrMagnitude <= Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        Vector3 forward;

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.z))
        {
            forward = difference.x > 0f
                ? Vector3.right
                : Vector3.left;
        }
        else
        {
            forward = difference.z > 0f
                ? Vector3.forward
                : Vector3.back;
        }

        Vector3 right = new Vector3(
            forward.z,
            0f,
            -forward.x
        );

        return right * m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Adjustment.x
             + forward * m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Adjustment.y;
    }
    // Playerから見た出現場所
    private void SetPositionFromPlayer()
    {
        Vector2Int distance = new(Mathf.Abs(m_playerController.GetCurrentIndex().x - m_currentAreaIndex.x), Mathf.Abs(m_playerController.GetCurrentIndex().y - m_currentAreaIndex.y));

        // 位置
        transform.position = new Vector3Int(distance.x, 0, distance.y) * 10;
        transform.position += GetAdjustmentFromPlayer();
        // 回転
        Vector3 direction = m_playerController.gameObject.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        // 大きさ
        GetComponent<BoxCollider>().size = new(m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Size.x, m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Size.y, 1);
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

        SetPositionFromPlayer();
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
                // m_targetPositionは現在のPlayerの位置なのか
                if (m_playerController.GetCurrentIndex() == m_targetPosition)
                {
                    m_checkHidePositionFlag = true;
                    // 当たってるのか
                    if (m_playerState.GetCurrentState().name != "HideState")
                        Debug.Log("hit : " + name);
                    break;
                }
                else
                {
                    // 更新
                    NextPositionDecide();
                }
            }

            move += Mathf.Abs(diff_.y) <= Mathf.Abs(diff_.x) ? new Vector2Int(1, 0) : new Vector2Int(0, 1);
        }

        // x方向に近づく
        m_currentAreaIndex.x += Math.Sign(diff.x) * move.x;

        // y方向に近づく
        m_currentAreaIndex.y += Math.Sign(diff.y) * move.y;

        SetPositionFromPlayer();


        transform.Find("Visual").GetComponent<SpriteRenderer>().sprite = m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Sprite;

        /*
         * 新しい最短経路 A*
         */
        // 現在地の周囲のマスから、一番Goalまで近いマスを選ぶ

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
