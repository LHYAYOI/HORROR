using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField] string m_audioName;
    [SerializeField] AudioClip m_audioClip;

    [SerializeField] bool m_loop;
    [SerializeField] float m_volume;

    AudioSource m_playingSource;

    public void SetVolume(float value)
    {
        m_volume = Mathf.Clamp01(value);

        if (m_playingSource != null && m_playingSource.isPlaying && m_playingSource.clip == m_audioClip) 
        {
            m_playingSource.volume = m_volume;
        }
    }

    public AudioClip GetAudioClip() 
    {
        return m_audioClip;
    }

    public string GetAudioName()
    {
        return m_audioName;
    }

    public float GetVolume() 
    {
        return m_volume; 
    }

    public bool IsLoop() 
    {
        return m_loop;
    }

    public void BindAudioSource(AudioSource audioSource) 
    {
        m_playingSource = audioSource;
    }

    public void UnBindAudioSource()
    {
        m_playingSource = null;
    }
}
