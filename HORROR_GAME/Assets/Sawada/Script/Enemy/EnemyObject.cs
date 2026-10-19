using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class EnemyObject : MonoBehaviour
{
    DefaultEnemy m_defaultEnemy;
    [SerializeField] Vector2Int m_currentAreaIndex;
    [SerializeField] Vector2Int m_lastTarget;
    [SerializeField] EnemyMove m_enemyMove;

    AreaManager m_areaManager;
    [SerializeField] Vector2Int m_targetPosition;
    StateMachine m_playerState;
    PlayerController m_playerController;
    bool m_checkHidePositionFlag = false;


    void Start()
    {
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("P : " + m_playerController.GetCurrentIndex());
            Debug.Log("E : " + m_currentAreaIndex);
        }
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            List<Vector2Int> path = m_enemyMove.FindPath(m_currentAreaIndex, m_targetPosition);
            foreach (var item in path)
            {
                Debug.Log($"{m_areaManager.GetMapData().GetArea(m_currentAreaIndex).name} {item}");
            }
        }
    }

    public void ChaseTarget()
    {
        m_defaultEnemy.CanChaseTarget(m_currentAreaIndex, m_playerController.GetCurrentIndex(), m_areaManager.GetMapData());
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
    }

    public Vector3 GetAdjustmentFromPlayer()
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
    public void SetPositionFromPlayer()
    {
        //Vector2Int distance = m_playerController.GetCurrentIndex() - m_currentAreaIndex;
        Vector2Int distance = m_currentAreaIndex - m_playerController.GetCurrentIndex();

        // 位置
        transform.position = new Vector3Int(distance.y, 0, distance.x) * 10;
        transform.position += GetAdjustmentFromPlayer();
        // 回転
        Vector3 direction = m_playerController.gameObject.transform.position - transform.position;
        if (direction.sqrMagnitude > 0.000001f) transform.rotation = Quaternion.LookRotation(direction);
        // 大きさ
        GetComponent<BoxCollider>().size = new(m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Size.x, m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Size.y, 1);
        // 画像
        //transform.Find("ghost").GetComponent<SpriteRenderer>().sprite = m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Sprite;

        //Debug.Log(m_currentAreaIndex + " - " + m_playerController.GetCurrentIndex() + " = " + distance + "  のポジション");
    }
    
    public void MoveEnemy_02()
    {
        /*
         * 新しい最短経路 A*
         */
        // 現在地の周囲のマスから、一番Goalまで近いマスを選ぶ
        List<Vector2Int> path = m_enemyMove.FindPath(m_currentAreaIndex, m_targetPosition);
        Vector2Int nextPosition = m_enemyMove.GetNextPosition(m_currentAreaIndex, path, m_defaultEnemy.GetSpeed());


        // ここに接敵判定
        Vector2Int playerIndex = m_playerController.GetCurrentIndex();
        Vector2Int diff = nextPosition - m_currentAreaIndex;
        Vector2Int direction = new(Math.Sign(diff.x), Math.Sign(diff.y));
        int distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
        bool isContact = false;
        // 移動途中のマスも確認
        for (int i = 1; i <= distance; i++)
        {
            Vector2Int checkPosi = m_currentAreaIndex + direction * i;

            if (checkPosi == playerIndex || checkPosi == m_targetPosition)
            {
                nextPosition = checkPosi;
                isContact = true;
                //Debug.Log("離脱開始");
                //Debug.Log("diff: " + diff);
                //Debug.Log("direction: " + direction);
                //Debug.Log("distance: " + distance);
                //Debug.Log("checkPosi: " + checkPosi);
                //Debug.Log("離脱終了");
                //Debug.Log("m_targetPosition: " + m_targetPosition);
                //Debug.Log("PlayerCurrentIndex: " + m_playerController.GetCurrentIndex());
                break;
            }
        }

        // 移動先を更新
        //Debug.Log($"E: {m_currentAreaIndex} -> {nextPosition}");
        m_currentAreaIndex = nextPosition;


        // 接敵・目的地到達時の処理
        if (isContact)
        {
            if (playerIndex == m_currentAreaIndex)
            {
                m_checkHidePositionFlag = true;

                if (m_playerState.GetCurrentState().name != "HideState")
                {
                    Debug.Log("hit: " + name);
                    m_playerController.KillPlayer();
                }
            }
            else
            {
                NextPositionDecide();
            }
        }


        Debug.Log($"E: {m_areaManager.GetMapData().GetArea(m_currentAreaIndex).name} {m_currentAreaIndex}");
        //Debug.Log("Pのposi:target: " + m_playerController.GetCurrentIndex() + " : " + m_targetPosition);
    }


    // Playerが視界にいるのか?
    public bool CheckPlayerInSight()
    {
        // 見えない
        if (!m_defaultEnemy.CanChaseTarget(m_currentAreaIndex, m_playerController.GetCurrentIndex(), m_areaManager.GetMapData()))
        {
            //ここ仮 GetComponent<EnemyView>().Hide();
            return false;
        }

        // Areaが行動可能か 未定
        if (CheckEnemyMovePosition(m_playerController.GetCurrentIndex()))
        {

        }
        m_targetPosition = m_playerController.GetCurrentIndex();

        return true;
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
        // Playerを視認するか?
        bool playerInSight = CheckPlayerInSight();
        Debug.Log("視認: " + playerInSight);

        // 見える      追う
        if (playerInSight)
        {
            // CheckPlayerInSight()内に書いてある。
        }
        // 見えない    ランダム座標へ
        else
        {
            MapData mapData = m_areaManager.GetMapData();
            Vector2Int nextPosition = new(UnityEngine.Random.Range(0, mapData.GetWidth()), UnityEngine.Random.Range(0, mapData.GetHeight()));
            int loopNum = 0;

            while (true)
            {
                // Areaが行動可能領域か?
                if (CheckEnemyMovePosition(nextPosition))
                {
                    m_targetPosition = nextPosition;
                    Debug.Log("NextPosi: " + m_targetPosition);
                    break;
                }

                // 横にずらす
                nextPosition = new(nextPosition.x % mapData.GetWidth(), nextPosition.x / mapData.GetWidth());

                loopNum++;
            }
        }
    }
}
