using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] PlayerController m_playerController;
    [SerializeField] StateMachine m_playerState;
    Vector2Int m_lastPlayerPosition;

    [Header("Enemy")]
    [SerializeField] GameObject m_enemyPrefab;
    [SerializeField] GameObject m_enemyParent;
    [SerializeField] List<DefaultEnemy> m_spawnEnemyList;
    [SerializeField] List<GameObject> m_spawnedEnemyList = new();

    [Header("Area")]
    [SerializeField] AreaManager m_areaManager;

    private void Update()
    {
    }

    public void AutoMove()
    {
        // 自動
        foreach (GameObject enemy in m_spawnedEnemyList)
        {
            //enemy.GetComponent<EnemyObject>().AutoMove();
            EnemyStateBase currentState = (EnemyStateBase)enemy.GetComponent<StateMachine>().GetCurrentState();
            currentState.AutoMove();
            enemy.GetComponent<EnemyObject>().SetPositionFromPlayer();
        }
    }

    // 始めて怪異と触れた時
    public void FarstEncount()
    {
        // スポーン
        foreach (DefaultEnemy enemy in m_spawnEnemyList)
        {
            // prefabからInstantiate
            GameObject instantEnemy = Instantiate(m_enemyPrefab);

            instantEnemy.transform.SetParent(m_enemyParent.transform);

            // 初期化
            instantEnemy.GetComponent<EnemyObject>().Init(enemy);
            StateMachine stateMachine = instantEnemy.GetComponent<StateMachine>();
            foreach (EnemyStateBase state in enemy.GetStateList())
            {
                state.InitEnemyState(instantEnemy);
                stateMachine.AddState(state);
            }
            stateMachine.ChangeState(0);

            // 追加
            m_spawnedEnemyList.Add(instantEnemy);
        }

        SetView();
    }

    // 可視 不可視
    public void SetView()
    {
        foreach (var enemy in m_spawnedEnemyList)
        {
            enemy.GetComponent<EnemyController>().ViewSet();
        }
    }
    public void SetView(bool viewFlag)
    {
        foreach (var enemy in m_spawnedEnemyList)
        {
            enemy.GetComponent<EnemyController>().ViewSet(viewFlag);
        }
    }
}
