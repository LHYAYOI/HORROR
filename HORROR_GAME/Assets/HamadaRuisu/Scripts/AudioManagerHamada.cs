using UnityEngine;
using System.Collections.Generic;

public class AudioManagerHamada : MonoBehaviour
{
    public static AudioManagerHamada instance;

    [SerializeField] int m_poolNumber = 10;
    [SerializeField] List<AudioData> m_audioDataList = new List<AudioData>();

    List<AudioSource> m_sources = new List<AudioSource>();

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < m_poolNumber; i++)
        {
            CreateNewAudioSource();
        }
    }

    public AudioSource CreateNewAudioSource() 
    {
        GameObject child = new GameObject($"AudioChannel_{m_sources.Count}");
        child.transform.SetParent(this.transform);
        AudioSource source = child.AddComponent<AudioSource>();
        m_sources.Add(source);
        return source;
    }

    public void Play(string audioName) 
    {
        AudioData playAudio = null;

        //指定したオーディオ
        playAudio = GetAudioData(audioName);

        if (playAudio == null) 
        {
            return;
        }

        //使われてないオーディオソースを探す
        AudioSource availableSource = null;

        foreach (AudioSource source in m_sources) 
        {
            if (!source.isPlaying) 
            {
                availableSource = source;
                break;
            }
        }

        if (availableSource == null) 
        {
            return;
        }

        availableSource.volume = playAudio.GetVolume();
        availableSource.clip = playAudio.GetAudioClip();
        availableSource.Play();

        playAudio.BindAudioSource(availableSource);

    }

    public void Stop(string audioName)
    {
        AudioData stopAudio = null;

        //指定したオーディオ
        stopAudio = GetAudioData(audioName);

        if (stopAudio == null)
        {
            return;
        }

        foreach (AudioSource source in m_sources)
        {
            if (source.isPlaying && source.clip == stopAudio.GetAudioClip())
            {
                source.Stop();
                source.clip = null;
                break;
            }
        }

        stopAudio.UnBindAudioSource();
    }

    public void StopAll() 
    {
        foreach (AudioSource source in m_sources) 
        {
            if (source == null) 
            {
                continue;
            }

            source.Stop();
            source.clip = null;
        }

        foreach (AudioData audioData in m_audioDataList)
        {
            if (audioData != null)
            {
                audioData.UnBindAudioSource();
            }
        }
    }

    public void SetVolume(string audioName, float volume) 
    {
        AudioData audioData = GetAudioData(audioName);

        if (audioData != null) 
        {
            audioData.SetVolume(volume);
        }
    }

    private AudioData GetAudioData(string audioName) 
    {
        //指定したオーディオ
        foreach (AudioData audioData in m_audioDataList)
        {
            if (audioData == null) 
            {
                continue;
            }

            if (audioData.GetAudioName() == audioName)
            {
                return audioData;
            }
        }

        return null;
    }
}
