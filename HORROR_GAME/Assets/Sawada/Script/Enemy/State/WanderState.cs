using UnityEngine;
using static PlayerController;

/*
 * プレイヤーを視認していない場合
 * 適当に徘徊
 */
[CreateAssetMenu(fileName = "newWanderState", menuName = "States/EnemyStates/WanderState")]
public class WanderState : EnemyStateBase
{
    PlayerController m_controller;

    public override void Awake()
    {
        m_stateName = "WanderState";
    }

    public override void Start()
    {

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
        // 移動
        m_enemyObject.GetComponent<EnemyObject>().MoveEnemy_02();
    }
}
