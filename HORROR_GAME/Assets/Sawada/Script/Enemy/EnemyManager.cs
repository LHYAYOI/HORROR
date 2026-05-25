using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject m_EnemyPrefab;
    [SerializeField] List<DefaultEnemy> m_SpownEnemyList;
    List<GameObject> m_spownEnemyList = new();

    AreaManager m_areaManager;
    [SerializeField] PlayerController m_playerController;

    void Start()
    {
        foreach (var enemy in m_SpownEnemyList)
        {
            GameObject instantEnemy = Instantiate(m_EnemyPrefab);
            instantEnemy.GetComponent<EnemyObject>().Init(enemy);
            m_spownEnemyList.Add(instantEnemy);
        }

        m_areaManager = FindFirstObjectByType<AreaManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            MoveTarget(m_playerController.GetCurrentIndex());
        }
    }

    // Targetが移動したらEnemysも移動処理
    public void MoveTarget(Vector2Int target)
    {
        foreach (var enemy in m_spownEnemyList)
        {
            enemy.GetComponent<EnemyObject>().MoveTarget(target);
        }
    }

    // Playerを追いかけるか？
    private bool CanChasePlayer()
    {


        return true;
    }
}
