using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HideSpotController : MonoBehaviour, IPressable
{
    [SerializeField] HidePlaceType m_hidePlaceType;

    EnemyManager m_enemyManager;
    Animator m_animator;
    PlayerController m_player;

    float m_findRate = 0.0f;

    private void Start()
    {
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();

        m_animator = GameObject.Find("FadeImage").GetComponent<Animator>();

        m_enemyManager = FindFirstObjectByType<EnemyManager>();
    }

    public void OnPressed()
    {
        AudioManagerHamada.instance.Play("Hide");

        m_player.SetHideSpot(this);

        m_player.ChangeSate("HideState");

        m_animator.SetTrigger("StartFade");

        HidePlaceUIController.instance.SetCurrentHidePlaceType(m_hidePlaceType);

        m_player.SetFindRate(m_findRate);

        m_enemyManager.AutoMove();
    }

    public void OnReleased()
    {
    }

    public void IncreaseFindRate() 
    {
        Debug.Log("確率変動");
        FindRateChanger(0.2f);
    }

    public void FindRateChanger(float IncreaseRate) 
    {
        m_findRate += IncreaseRate;

        if (m_findRate > 1) 
        {
            m_findRate = 1;
        }
    }

    //見つかる確率取得
    public float GetFindRate() 
    {
        return m_findRate;
    }
}
