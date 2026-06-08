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

    public override void Awake()
    {
        m_stateName = "StandbyState_Time";
    }

    public override void Start()
    {
        m_timer = 0.0f;
    }

    public override void EnterState()
    {

    }

    public override void UpdateState()
    {
        m_timer += Time.deltaTime;
        if (m_time <= m_timer)
        {
            // 次の行先を決める
            m_enemyObject.GetComponent<EnemyObject>().NextPositionDecide();

            // state切り替え
            m_enemyObject.GetComponent<StateMachine>().ChangeState("WanderState");
        }
    }

    public override void ExitState()
    {

    }

    public override void InitEnemyState(GameObject enemyObject)
    {
        m_enemyObject = enemyObject;
    }

    public override void AutoMove()
    {

    }
}
