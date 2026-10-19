using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}

public class MoveButtonController : MonoBehaviour, IPressable
{

    [SerializeField] MoveDirection m_moveDirectionType;
    [SerializeField] MoveButtonView m_moveButtonView;
    PlayerController m_playerController;

    private void Start()
    {
        m_playerController = FindAnyObjectByType<PlayerController>();
    }

    private void MakePlayerMove()
    {
        switch (m_moveDirectionType)
        {
            case MoveDirection.Up:
                m_playerController.MoveUp();
                break;

            case MoveDirection.Down:
                m_playerController.MoveDown();
                break;

            case MoveDirection.Right:
                m_playerController.MoveRight();
                break;

            case MoveDirection.Left:
                m_playerController.MoveLeft();
                break;
        }
    }

    public void OnPressed()
    {
        MakePlayerMove();
    }

    public void OnReleased()
    {
    }
}
