using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName ="newNormalState",menuName="States/PlayerStates/NormalState")]
public class NormalState:StateBase
{
    PlayerController m_controller;

    public override void Awake()
    {
        m_stateName = "NormalState";
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
        m_controller.MoveAreaInputHandler();
        m_controller.CameraInputHandler();
        m_controller.RotateCameraInputHandler();
        m_controller.InteractionHandler();
    }

    public override void ExitState()
    {
      
    }
}
