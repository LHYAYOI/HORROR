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
        m_player.ChangeSate("HideState");

        m_animator.SetTrigger("StartFade");


    }

    public void OnReleased()
    {
    }

    public void IncreaseFindRate(float IncreaseRate) 
    {
        m_findRate += IncreaseRate;
    }
}
