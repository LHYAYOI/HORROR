using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;
using System.Collections.Generic;

/*
 * プレイヤーを視認していない場合
 * 数秒間前を見る
 */
[CreateAssetMenu(fileName = "newStandbyState_Time", menuName = "States/EnemyStates/StandbyState_Time")]
public class StandbyState_Time : EnemyStateBase
{
    [SerializeField] float m_time;
    float m_timer;

    EnemyManager m_enemyManager;

    public override void Awake()
    {
        m_stateName = "StandbyState_Time";
    }

    public override void Start()
    {
        m_timer = 0.0f;
        m_enemyManager = m_stateOwner.GetComponent<EnemyController>().GetEnemyManager();
    }

    public override void EnterState()
    {
        m_timer = 0.0f;
        Debug.Log(m_enemyManager);
    }

    public override void UpdateState()
    {
        m_timer += Time.deltaTime;

        if (m_time <= m_timer)
        {
            m_enemyManager.GetPlayerController().ExitHideState();

            AudioManagerHamada.instance.Stop("HeartSound");

            //別のエリアに移動


            // 次の行先を決める
            m_stateOwner.GetComponent<EnemyObject>().NextPositionDecide();

            // state切り替え
            m_stateOwner.GetComponent<StateMachine>().ChangeState("WanderState");

            m_timer = 0.0f;
        }
    }

    public override void ExitState()
    {

    }

    public override void InitEnemyState(GameObject enemyObject)
    {
        m_enemyObject = enemyObject;
        m_enemyManager = m_stateOwner.GetComponent<EnemyManager>();
        Debug.Log(m_enemyManager);
    }

    private void DecideWarpPoint() 
    {
        Vector2 playerIndex = m_enemyManager.GetPlayerController().GetCurrentIndex();

        List<AreaController> list = m_stateOwner.GetComponent<EnemyController>().GetWarpPointList();

        float maxDistance = 0;

        Vector2Int warpPoint = m_stateOwner.GetComponent<EnemyObject>().GetCurrentAreaIndex(); 

        foreach (AreaController area in list) 
        {
            float distance = (area.GetAreaPosition().x + area.GetAreaPosition().y);

            //最長距離更新
            if (maxDistance > distance) 
            {
                maxDistance = distance;
                warpPoint = area.GetAreaPosition();
            }
        }

        //実際に移動

    }

    public override void AutoMove()
    {

    }
}
