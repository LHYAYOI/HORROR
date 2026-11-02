using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;

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

            //別のエリアに移動

            m_enemyManager.GetPlayerController().ExitHideState();

            // 次の行先を決める
            m_stateOwner.GetComponent<EnemyObject>().NextPositionDecide();
            //m_enemyObject.GetComponent<EnemyObject>().ChaseTarget();

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

    public override void AutoMove()
    {

    }
}
