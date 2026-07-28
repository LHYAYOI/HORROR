using CriWare;
using CriWare.Assets;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CriAtomSourceForAsset))]
public class CriAudioController : MonoBehaviour
{
    //Soundの内容。Editorで設定する
    public CriAtomCueReference soundCue;
    //Soundを再生するSource。Editorで設定する
    public CriAtomSourceForAsset soundSource;

    // 再生中の音声の情報を保持する辞書（キュー名またはIDをキーとして、再生インスタンスを保持）
    private Dictionary<string, CriAtomExPlayback> playbackDict = new Dictionary<string, CriAtomExPlayback>();
    // キュー名またはIDをキーとして、音量を保持する辞書（必要に応じて使用）
    private Dictionary<string, float> savedVolumeDict = new Dictionary<string, float>();

    // 現在再生中のキュー情報（何も再生していない場合は ID: -1 / Name: "" とする）
    public int CurrentCueID { get; private set; } = -1;
    public string CurrentCueName { get; private set; } = string.Empty;

    // 現在再生中のものを追跡するための最新のPlayback
    private CriAtomExPlayback currentPlayback;

    [SerializeField]
    private Transform listener;

    [SerializeField, Range(0.0f, 1.0f)]
    private float[] volume = new float[6];
    
    
    // もともとのデフォルト音量を記憶する変数
    private float defaultVolume = 1.0f;

    [Header("Distance Attenuation Settings")]
    [Tooltip("最小距離（これより近いと音量最大）")]
    [SerializeField] private float minDistance = 1f;
    [Tooltip("最大距離（これより遠いと音量0）")]
    [SerializeField] private float maxDistance = 100f;
    [Tooltip("距離に対する音量の減衰カーブ（横軸:距離割合, 縦軸:音量）")]
    [SerializeField] private AnimationCurve volumeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private void Awake()
    {
        // アタッチされているCriAtomSourceを取得
        soundSource = GetComponent<CriAtomSourceForAsset>();

    }

    private void Update()
    {


        // リスナーとの距離に応じて音量を計算して変更する→何か変更が起きた場合にのみ音量を変更したいけど…('ω')
        UpdateVolumeByDistance();


        // すでに再生終了（Removed）した音声をplaybackDictから取り除く
        List<string> removeList = new List<string>();
        foreach (var kvp in playbackDict)
        {
            if (kvp.Value.GetStatus() == CriAtomExPlayback.Status.Removed)
            {
                removeList.Add(kvp.Key);
            }
        }
        foreach (var key in removeList)
        {
            playbackDict.Remove(key);
            if (CurrentCueName == key) CurrentCueName = string.Empty;
            if (CurrentCueID.ToString() == key) CurrentCueID = -1;
        }
    }



    /// <summary>
    /// キュー名(string)を指定して音声を再生します
    /// </summary>
    public void PlaySound(string cueName,bool loop=true)
    {
        if (soundSource == null || string.IsNullOrEmpty(cueName)) return;

        soundSource.loop = loop;
        if (soundSource.player != null)
        {
            soundSource.player.Loop(loop);
            float baseVol = savedVolumeDict.TryGetValue(cueName, out float saved) ? saved : defaultVolume;
            float initAtt = CalculationVolume(listener);
            soundSource.player.SetVolume(baseVol * initAtt);
        }
        CriAtomExPlayback playback = soundSource.Play(cueName);


        // キュー名をキーとして保存
        playbackDict[cueName] = playback;

        // 現在の再生情報を更新
        currentPlayback = playback;
        CurrentCueName = cueName;
        CurrentCueID = -1; // 名前再生時はIDが不明なため一旦リセット
    }

    /// <summary>
    /// キューID(int)を指定して音声を再生します
    /// </summary>
    public void PlaySound(int cueID,bool loop = true)
    {
        if (soundSource == null) return;
        
        soundSource.loop = loop;
        if (soundSource.player != null)
        {
            soundSource.player.Loop(loop);
            float baseVol = savedVolumeDict.TryGetValue(cueID.ToString(), out float saved) ? saved : defaultVolume;
            float initAtt = CalculationVolume(listener);
            soundSource.player.SetVolume(baseVol * initAtt);
        }

        CriAtomExPlayback playback = soundSource.Play(cueID);

        // IDを文字列に変換してキーとして保存
        playbackDict[cueID.ToString()] = playback;

        // 現在の再生情報を更新
        currentPlayback = playback;
        CurrentCueID = cueID;
        CurrentCueName = string.Empty;
    }

    /// <summary>
    /// キュー名(string)を指定して、その音声だけをポーズします
    /// </summary>
    public void PauseSound(string cueName)
    {
        if (playbackDict.TryGetValue(cueName, out CriAtomExPlayback playback))
        {
            InternalPause(playback);
        }
        else
        {
            Debug.LogWarning($"Cue: {cueName} の再生情報が見つかりません。");
        }
    }

    /// <summary>
    /// 指定したCueIDの音声だけポーズ（一時停止）します
    /// </summary>
    public void PauseSound(int cueID)
    {
        // 辞書に指定されたCueIDの再生データが存在するか確認
        if (playbackDict.TryGetValue(cueID.ToString(), out CriAtomExPlayback playback))
        {
            InternalPause(playback);
        }
        else
        {
            Debug.LogWarning($"CueID: {cueID} の再生情報が見つかりません。");
        }
    }

    /// <summary>
    /// 現在のステータスを取得し、エラーや破棄済み（Removed）でなければポーズを実行
    /// </summary>
    private void InternalPause(CriAtomExPlayback playback)
    {
        if (playback.GetStatus() != CriAtomExPlayback.Status.Removed)
        {
            playback.Pause();
        }
    }


    /// <summary>
    /// キュー名(string)を指定して、その音声のポーズを解除します
    /// </summary>
    public void ResumeSound(string cueName)
    {
        if (playbackDict.TryGetValue(cueName, out CriAtomExPlayback playback))
        {
            InternalResume(playback);
        }
    }

    /// <summary>
    /// キューID(int)を指定して、その音声のポーズを解除します
    /// </summary>
    public void ResumeSound(int cueID)
    {
        if (playbackDict.TryGetValue(cueID.ToString(), out CriAtomExPlayback playback))
        {
            InternalResume(playback);
        }
    }

    private void InternalResume(CriAtomExPlayback playback)
    {
        if (playback.GetStatus() != CriAtomExPlayback.Status.Removed)
        {
            playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
        }
    }


    public void StopSound()
    {
        if (soundSource == null) return;

        soundSource.Stop();

        // 再生情報を更新
        CurrentCueID = -1;
        CurrentCueName = string.Empty;


    }

    /// <summary>
    /// 音量を調整します。（0-1）
    /// </summary>
    public void ChangeVolume(float vol)
    {
        if (soundSource != null)
        {
            soundSource.volume = vol;
            soundSource.player.SetVolume(vol);
        }
    }

    /// <summary>
    /// あらかじめ設定した音量に調整します。
    /// </summary>
    public void ChangeVolume(int channel)
    {
        if (soundSource != null)
        {
            soundSource.volume = volume[channel];
            soundSource.player.SetVolume(volume[channel]);
        }
    }

    public void ChangeVolume(string cueName, float vol)
    {
        savedVolumeDict[cueName] = vol; // まずベース音量を上書き
        if (playbackDict.TryGetValue(cueName, out CriAtomExPlayback playback))
        {
            ApplyVolumeWithDistance(cueName, playback); // 距離も加味して再適用
        }
    }

    public void ChangeVolume(int cueID, float vol)
    {
        savedVolumeDict[cueID.ToString()] = vol;
        if (playbackDict.TryGetValue(cueID.ToString(), out CriAtomExPlayback playback))
        {
            ApplyVolumeWithDistance(cueID.ToString(), playback);
        }
    }

    /// <summary>
    /// 特定のPlaybackに対して音量を適用する内部処理
    /// </summary>
    private void InternalChangeVolume(CriAtomExPlayback playback, float vol)
    {

        if (soundSource != null && playback.GetStatus() != CriAtomExPlayback.Status.Removed)
        {
            soundSource.player.SetVolume(vol);
            soundSource.player.Update(playback);
        }
    }

    /// <summary>
    /// 外部のスクリプト（移動時など）から呼び出し、鳴っているすべての音の距離減衰を更新する
    /// </summary>
    public void UpdateVolumeByDistance()
    {
        foreach (var kvp in playbackDict)
        {
            ApplyVolumeWithDistance(kvp.Key, kvp.Value);
        }
    }


    /// <summary>
    /// ベース音量 × 距離減衰を計算して特定のPlaybackに適用する
    /// </summary>
    private void ApplyVolumeWithDistance(string key, CriAtomExPlayback playback)
    {
        if (soundSource == null || playback.GetStatus() == CriAtomExPlayback.Status.Removed) return;

        // 1. ベース音量の決定（辞書にあればそれ、無ければデフォルト）
        float baseVol = savedVolumeDict.TryGetValue(key, out float saved) ? saved : defaultVolume;

        // 2. 距離減衰の割合(0~1)を取得
        float attenuation = CalculationVolume(listener);

        // 3. 掛け合わせた最終音量を適用
        soundSource.player.SetVolume(baseVol * attenuation);
        soundSource.player.Update(playback);
    }

    /// <summary>
    /// リスナーとの距離から、減衰の「割合(0.0〜1.0)」だけを計算して返す
    /// </summary>
    public float CalculationVolume(Transform targetTransform)
    {
        if (soundSource == null || targetTransform == null)
        {
            Debug.LogWarning("SoundSourceまたはターゲットTransformが設定されていません。");
            return 1.0f;
        }

        float distance = Vector3.Distance(transform.position, targetTransform.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);
        return volumeCurve.Evaluate(normalizedDistance);
    }
}