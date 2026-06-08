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
    [SerializeField] PlayerController m_playerController;
    [SerializeField] float m_concludeChargeTime = 0.5f;

    float m_currentConcludeChargeTime = 0.0f;

    bool m_canCharge = false;

    void Start()
    {

    }

    void Update()
    {
        ConcludeChargeProcess();
    }

    private void ConcludeChargeProcess()
    {
        if (m_canCharge == false)
        {
            if (m_currentConcludeChargeTime > 0)
            {
                //ゲージを減衰
                m_currentConcludeChargeTime -= Time.deltaTime;

                float rate = m_currentConcludeChargeTime / m_concludeChargeTime;

                rate = Mathf.Clamp01(rate);

                m_moveButtonView.UpdateGauge(Easing.OutCubic(rate));
            }
        }
        else
        {
            m_currentConcludeChargeTime += Time.deltaTime;

            // ゲージを減衰を増幅
            float rate = m_currentConcludeChargeTime / m_concludeChargeTime;

            rate = Mathf.Clamp01(rate);

            m_moveButtonView.UpdateGauge(Easing.OutCubic(rate));

            // 移動の実行
            if (m_currentConcludeChargeTime >= m_concludeChargeTime)
            {
                MakePlayerMove();
                ResetValues();
            }
        }
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

    private void ResetValues()
    {
        m_canCharge = false;
        m_moveButtonView.UpdateGauge(0);
    }

    public void OnPressed()
    {
        m_canCharge = true;
    }

    public void OnReleased()
    {
        ResetValues();
    }
}
