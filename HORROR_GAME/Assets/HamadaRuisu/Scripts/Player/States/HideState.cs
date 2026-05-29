using UnityEngine;

[CreateAssetMenu(fileName = "newHideState", menuName = "States/PlayerStates/HideState")]
public class HideState:StateBase
{
    PlayerController m_controller;

    public override void Awake()
    {
        m_stateName = "HideState";
    }

    public override void Start()
    {
        m_controller = m_stateOwner.GetComponent<PlayerController>();
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
}
