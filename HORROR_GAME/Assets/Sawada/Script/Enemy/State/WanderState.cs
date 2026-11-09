using UnityEngine;

/*
 * プレイヤーを視認していない場合
 * 適当に徘徊
 */
[CreateAssetMenu(fileName = "newWanderState", menuName = "States/EnemyStates/WanderState")]
public class WanderState : EnemyStateBase
{
    PlayerController m_controller;
    //[SerializeField] AudioData m_horrorAudio;

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
        // Playerを視認しているか確認
        m_enemyObject.GetComponent<EnemyObject>().CheckPlayerInSight();
        // 移動
        m_enemyObject.GetComponent<EnemyObject>().MoveEnemy_02();
        // Playerを視認しているか確認
        m_enemyObject.GetComponent<EnemyObject>().CheckPlayerInSight();

        /*Vector3 adjust = m_enemyObject.GetComponent<EnemyObject>().GetAdjustmentFromPlayer();
        if (adjust.x + adjust.z != 0)
        {
            
        }
        Debug.Log($"WanderState: adjust = {adjust}");*/
    }
}
