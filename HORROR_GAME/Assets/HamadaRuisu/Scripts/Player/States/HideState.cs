using UnityEngine;
using static PlayerController;

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
        if (Input.GetMouseButtonDown(0))
        {
            AudioManagerHamada.instance.Play("Hide");

            m_controller.GetHideAnimator().SetTrigger("StartFade");
            m_controller.GetStateMachine().ChangeState("NormalState");
        }
    }

    public override void ExitState()
    {

    }
}
