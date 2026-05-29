using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FadeEvents : MonoBehaviour
{
    [SerializeField] PlayerController m_player;
    [SerializeField] Volume m_volume;
    Vignette m_vignette;

    private void Start()
    {
        m_volume.profile.TryGet(out m_vignette);
    }

    public void EnableDisableVignette()
    {
        if (m_player.GetStateMachine().GetCurrentState().GetStateName() == "NormalState")
        {
            m_vignette.active = false;
        }
        else if (m_player.GetStateMachine().GetCurrentState().GetStateName() == "HideState")
        {
            m_vignette.active = true;
        }
    }
}
