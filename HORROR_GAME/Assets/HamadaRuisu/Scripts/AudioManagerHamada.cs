using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class AudioManagerHamada : MonoBehaviour
{
    [SerializeField] int m_poolNumber = 10;
    [SerializeField] List<AudioData> m_audioDataList = new List<AudioData>();

    List<AudioSource> m_sources = new List<AudioSource>();

    private void Awake()
    {
        for (int i = 0; i < m_poolNumber; i++)
        {
            CreateNewAudioSource();
        }
    }

    public AudioSource CreateNewAudioSource() 
    {
        GameObject child = new GameObject();
        child.transform.SetParent(this.transform);
        AudioSource source = child.AddComponent<AudioSource>();
        m_sources.Add(source);
        return source;
    }

    public void Play(string audioName) 
    {
        //指定したオーディオ
        foreach (AudioData audioData in m_audioDataList) 
        {
            if (audioData.GetAudioName() != audioName) 
            {
                continue;
            }
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


    }
}
