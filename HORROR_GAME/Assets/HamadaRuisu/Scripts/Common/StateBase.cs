//
//ステート基底クラス
//濱田ルイス
//

using UnityEngine;

[]
public class StateBase : ScriptableObject
{
    protected StateMachine m_stateMachine = null;
    protected GameObject m_stateOwner = null;
    protected string m_stateName;
    protected float m_stateTime = 0.0f;

    public void SetStateMachine(StateMachine stateMachine) { m_stateMachine = stateMachine; }
    public void SetStateOwner(GameObject gameObject) { m_stateOwner = gameObject; }
    public string GetStateName() { return m_stateName; }

    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}
