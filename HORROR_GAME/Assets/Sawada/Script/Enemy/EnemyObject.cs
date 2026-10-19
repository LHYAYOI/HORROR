using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyObject : MonoBehaviour
{
    DefaultEnemy m_defaultEnemy;
    [SerializeField] Vector2Int m_currentAreaIndex;
    [SerializeField] Vector2Int m_lastTarget;

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
        Vector2Int distance = m_playerController.GetCurrentIndex() - m_currentAreaIndex;

        // 位置
        transform.position = new Vector3Int(distance.x, 0, distance.y) * 10;
        transform.position += GetAdjustmentFromPlayer();
        // 回転
        Vector3 direction = m_playerController.gameObject.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        // 大きさ
        GetComponent<BoxCollider>().size = new(m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Size.x, m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Size.y, 1);
        // 画像
        //transform.Find("ghost").GetComponent<SpriteRenderer>().sprite = m_defaultEnemy.GetImage(m_playerController.GetCurrentIndex(), m_currentAreaIndex).Sprite;

        Debug.Log(m_playerController.GetCurrentIndex() + " - " + m_currentAreaIndex + " = " + distance + "  のポジション");
    }
    
    public void MoveEnemy_02()
    {
        // 最短距離で移動先
        Vector2Int move = new(0, 0);
        Vector2Int diff = m_targetPosition - m_currentAreaIndex;
        Vector2Int playerIndex = m_playerController.GetCurrentIndex();
        Debug.Log("diff : " + diff);
        for (int i = 0; i < m_defaultEnemy.GetSpeed(); i++)
        {
            // 移動
            //move += Mathf.Abs(diff.x) >= Mathf.Abs(diff.y) ? new Vector2Int(1 * Math.Sign(diff.x), 0) : new Vector2Int(0, 1 * Math.Sign(diff.y));
            move += Mathf.Abs(diff.x) >= Mathf.Abs(diff.y) ? new Vector2Int(1 * Math.Sign(diff.x), 0) : new Vector2Int(0, 1 * Math.Sign(diff.y));

            if (m_currentAreaIndex + move == playerIndex || m_currentAreaIndex + move == m_targetPosition)
            {
                Debug.Log("重なり");
                // m_targetPositionは現在のPlayerの位置なのか
                if (playerIndex == m_targetPosition)
                {
                    m_checkHidePositionFlag = true;
                    // 当たってるのか
                    if (m_playerState.GetCurrentState().name != "HideState")
                    {
                        Debug.Log("hit : " + name);
                        m_playerController.KillPlayer();
                    }
                    break;
                }
                else
                {
                    // 更新
                    NextPositionDecide();
                }
            }
        }

        m_currentAreaIndex += move;

        /*
         * 新しい最短経路 A*
         */
        // 現在地の周囲のマスから、一番Goalまで近いマスを選ぶ

        // 
        Debug.Log("EnemyMove");

        //Vector2Int.up
    }

    // 現在地から上下左右のマスを順番に探索して


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
        Debug.Log("視認 : " + CheckPlayerInSight());
        // Playerを視認していない
        if (!CheckPlayerInSight())
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
                    Debug.Log("NextPosi : " + m_targetPosition);
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
