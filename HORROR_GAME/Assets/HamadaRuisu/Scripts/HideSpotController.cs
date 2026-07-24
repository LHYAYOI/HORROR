using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HideSpotController : MonoBehaviour, IPressable
{
    Animator m_animator;
    PlayerController m_player;

    float m_findRate = 0.0f;

    private void Start()
    {
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();

        m_animator = GameObject.Find("FadeImage").GetComponent<Animator>();

    }

    public void OnPressed()
    {
        AudioManagerHamada.instance.Play("Hide");

        m_player.ChangeSate("HideState");

        m_animator.SetTrigger("StartFade");

        IncreaseFindRate(0.2f);

        m_player.SetFindRate(m_findRate);
    }

    public void OnReleased()
    {
    }

    public void IncreaseFindRate(float IncreaseRate) 
    {
        m_findRate += IncreaseRate;

        if (m_findRate > 1) 
        {
            m_findRate = 1;
        }
    }

    //Œ©‚Â‚©‚éŠm—¦Žæ“¾
    public float GetFindRate() 
    {
        return m_findRate;
    }
}
