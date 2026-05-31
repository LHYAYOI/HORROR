using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

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
        // kaiに触れてたら
        if (m_playerController.InteractedWithKai())
        {
            /*
             * PlayerControllerのInteractionHandler()のif (interactedObject.tag == "Kai")の中に追加
             * if (!m_interactedKai) m_nemyManager.FarstEncount();
             */
            if (Input.GetKeyDown(KeyCode.R) && m_spawnedEnemyList.Count == 0) FarstEncount();     // 削除
        }

        // 進む
        /*
         * PlayerControllerのMoveArea()の中に追加
         * m_nemyManager.AutoMove();
         */
        if (Input.GetKeyDown(KeyCode.E)) AutoMove();                               // 削除
    }

    // Targetが移動したらEnemysも移動処理
    public void MoveTarget(Vector2Int target)
    {
        // 自動
        foreach (GameObject enemy in m_spawnedEnemyList)
        {
            enemy.GetComponent<EnemyObject>().AutoMove();
        }
    }
    public void AutoMove()
    {
        // 自動
        foreach (GameObject enemy in m_spawnedEnemyList)
        {
            enemy.GetComponent<EnemyObject>().AutoMove();
        }
    }

    // 始めて怪異と触れた時
    private void FarstEncount()
    {
        // スポーン
        foreach (DefaultEnemy enemy in m_spawnEnemyList)
        {
            GameObject instantEnemy = Instantiate(m_enemyPrefab);
            instantEnemy.transform.SetParent(m_enemyParent.transform);
            instantEnemy.GetComponent<EnemyObject>().Init(enemy);
            m_spawnedEnemyList.Add(instantEnemy);
        }
    }
}
