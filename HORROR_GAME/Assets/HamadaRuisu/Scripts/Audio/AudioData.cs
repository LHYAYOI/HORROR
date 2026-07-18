using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField] string m_audioName;
    [SerializeField] AudioClip m_audioClip;

    [SerializeField] bool m_loop;
    [SerializeField] float m_volume;

    public void SetVolume(float value)
    {
        m_volume = value;
    }

    public string GetAudioName()
    {
        return m_audioName;
    }

    public bool IsLoop() 
    {
        return m_loop;
    }
}
