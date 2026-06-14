using UnityEngine;

public class EnemyStateBase : StateBase
{
    protected GameObject m_enemyObject;
    public virtual void InitEnemyState(GameObject enemyObject) { }

    public virtual void AutoMove() { }

    public virtual void QuickMove() { }
}
