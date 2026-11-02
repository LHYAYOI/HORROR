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
        if (m_controller.IsDead())
        {
            return;
        }

        CursorManager.instance.EnableCrossHair();
        CursorManager.instance.ChangeCursorImage(CURSOR_TYPE.DEFAULT);

        if (Input.GetMouseButtonDown(0))
        {
            m_controller.ExitHideState();
        }
    }

    public override void ExitState()
    {

    }
}
