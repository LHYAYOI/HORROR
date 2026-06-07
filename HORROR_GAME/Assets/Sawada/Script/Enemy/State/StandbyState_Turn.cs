using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;

/*
 * プレイヤーを視認していない場合
 * 数秒間前を見る
 */
[CreateAssetMenu(fileName = "newStandbyState_Turn", menuName = "States/EnemyStates/StandbyState_Turn")]
public class StandbyState_Turn : EnemyStateBase
{
    [SerializeField] int m_turn;
    int m_timer;

    public override void Awake()
    {
        m_stateName = "StandbyState_Turn";
    }

    public override void Start()
    {
        m_timer = 0;
    }

    public override void EnterState()
    {

    }

    public override void UpdateState()
    {

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
        m_timer++;
        if (m_turn <= m_timer)
        {
            // 次の行先を決める
            m_enemyObject.GetComponent<EnemyObject>().NextPositionDecide();

            // state切り替え
            m_enemyObject.GetComponent<StateMachine>().ChangeState("WanderState");
        }
    }
}
