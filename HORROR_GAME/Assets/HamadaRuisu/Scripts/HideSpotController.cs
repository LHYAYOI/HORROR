using UnityEngine;

public class HideSpotController : MonoBehaviour,IIntaractable
{

    Animator m_animator;
    PlayerController m_player;

    private void Start()
    {
        m_player = GameObject.Find("Player").GetComponent<PlayerController>();

        m_animator = GameObject.Find("FadeImage").GetComponent<Animator>();
    }

    public void OnInteracted() 
    {
        m_player.ChangePlayerState(PlayerController.PLAYER_STATE.HIDE);

        m_animator.SetTrigger("StartFade");
    }
}
