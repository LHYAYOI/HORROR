using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("使用Audio一覧")]
    [SerializeField] List<AudioClip> bgm = new List<AudioClip>();
    [SerializeField] List<AudioClip> se = new List<AudioClip>();

    [Header("Prefab")]
    [SerializeField] GameObject audioSourcePrefab;
    List<GameObject> audioSources = new List<GameObject>();


    [Header("仮")]
    [SerializeField] float w;
    [SerializeField] float h;
    [SerializeField] float size;
    [SerializeField] GameObject cam;

    void Start()
    {
        // AudioSource初期化
        GameObject AudioSorceParent = GameObject.Find("AudioSources");
        float harfSize = size / 2;
        // 横
        for (int j = 0; j < h + 1; j++)
        {
            for (int i = 0; i < w; i++)
            {
                // インスタンス作製
                GameObject instancePrefab = Instantiate(audioSourcePrefab);
                audioSources.Add(instancePrefab);
                instancePrefab.transform.parent = AudioSorceParent.transform;
                // 位置調整
                instancePrefab.transform.position = new(harfSize + i * size, 0.0f, j * size);
            }
        }
        // 縦
        for (int i = 0; i < w + 1; i++)
        {
            for (int j = 0; j < h; j++)
            {
                // インスタンス作製
                GameObject instancePrefab = Instantiate(audioSourcePrefab);
                audioSources.Add(instancePrefab);
                instancePrefab.transform.parent = AudioSorceParent.transform;
                // 位置調整
                instancePrefab.transform.position = new(i * size, 0.0f, harfSize + j * size);
            }
        }

        cam.transform.position = new(harfSize, 0.0f, harfSize); // 適当おき
    }

    void Update()
    {
        
    }

    // nameと同じ名前のBGMを探す
    public AudioClip SearchBGM(string name)
    {
        return bgm.Find(clip => clip != null && clip.name == name);
    }
    // nameと同じ名前のSEを探す
    public AudioClip SearchSE(string name)
    {
        return se.Find(clip => clip != null && clip.name == name);
    }
}
