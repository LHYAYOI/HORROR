using UnityEngine;
using UnityEngine.Audio;

public class EnemyAudio : MonoBehaviour
{
    [SerializeField] AudioSource m_audioSource;
    [SerializeField] AudioData m_horrorAudio;

    [Header("距離減衰")]
    [SerializeField] float m_distanceDecay;

    private void Start()
    {
        if (!m_audioSource || !m_horrorAudio) return;

        m_audioSource.clip = m_horrorAudio.GetAudioClip();
        m_audioSource.loop = m_horrorAudio.IsLoop();
        m_audioSource.volume = m_horrorAudio.GetVolume();

        m_audioSource.Play();
    }
}
