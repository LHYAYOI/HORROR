using CriWare;
using CriWare.Assets;
using System.Collections.Generic;
using UnityEngine;
using static CriWare.CriProfiler;


[RequireComponent(typeof(CriAtomSourceForAsset))]
public class CriAudioController : MonoBehaviour
{
    //Soundの内容。Editorで設定する
    public CriAtomCueReference soundCue;
    //Soundを再生するSource。Editorで設定する
    public CriAtomSourceForAsset soundSource;

    // 再生中の音声の情報を保持する辞書（キュー名またはIDをキーとして、再生インスタンスを保持）
    // 1つのキーに対して、複数のPlaybackを「リスト」で保持
    private Dictionary<string, List<CriAtomExPlayback>> playbackDict = new Dictionary<string, List<CriAtomExPlayback>>();    
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
    private float currentSourceVolumeMultiplier = 1.0f;
    private bool isDynamicVolumeMode = true;

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

        // 初期状態の全体音量として、InspectorのCriAtomSourceのvolumeをセット
        currentSourceVolumeMultiplier = soundSource.volume;

    }

    private void Update()
    {


        // リスナーとの距離に応じて音量を計算して変更する→何か変更が起きた場合にのみ音量を変更したい
        //UpdateVolumeByDistance();


        // すでに再生終了（Removed）した音声をplaybackDictから取り除く
        List<string> removeList = new List<string>();
        foreach (var kvp in playbackDict)
        {
            kvp.Value.RemoveAll(p => p.GetStatus() == CriAtomExPlayback.Status.Removed);

            if (kvp.Value.Count == 0)
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
        if (isDynamicVolumeMode)
        {
            currentSourceVolumeMultiplier = CalculationVolume(listener);
            soundSource.volume = currentSourceVolumeMultiplier; // Inspector表示も同期
        }
        soundSource.loop = loop;
        if (soundSource.player != null)
        {
            soundSource.player.Loop(loop);

            // 再生直前に 個別音量 × 全体音量 をセット
            float individualVol = savedVolumeDict.TryGetValue(cueName, out float saved) ? saved : 1.0f;
            soundSource.player.SetVolume(individualVol * currentSourceVolumeMultiplier);
        }
        CriAtomExPlayback playback = soundSource.Play(cueName);


        // 辞書にリストがない場合は作成し、そこに再生情報を追加する
        if (!playbackDict.ContainsKey(cueName))
        {
            playbackDict[cueName] = new List<CriAtomExPlayback>();
        }
        playbackDict[cueName].Add(playback);
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
        if (isDynamicVolumeMode)
        {
            currentSourceVolumeMultiplier = CalculationVolume(listener);
            soundSource.volume = currentSourceVolumeMultiplier; // Inspector表示も同期
        }
        soundSource.loop = loop;
        if (soundSource.player != null)
        {
            soundSource.player.Loop(loop);

            float individualVol = savedVolumeDict.TryGetValue(cueID.ToString(), out float saved) ? saved : 1.0f;
            soundSource.player.SetVolume(individualVol * currentSourceVolumeMultiplier);
        }
        CriAtomExPlayback playback = soundSource.Play(cueID);

        // 辞書にリストがない場合は作成し、そこに再生情報を追加する
        string key = cueID.ToString();
        if (!playbackDict.ContainsKey(key))
        {
            playbackDict[key] = new List<CriAtomExPlayback>();
        }
        playbackDict[key].Add(playback);

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
        if (playbackDict.TryGetValue(cueName, out List<CriAtomExPlayback> playbacks))
        {
            foreach (var playback in playbacks) InternalPause(playback);
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
        if (playbackDict.TryGetValue(cueID.ToString(), out List<CriAtomExPlayback> playbacks))
        {
            foreach (var playback in playbacks) InternalPause(playback);
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
        if (playbackDict.TryGetValue(cueName, out List<CriAtomExPlayback> playbacks))
        {
            foreach (var playback in playbacks) InternalResume(playback);
        }
    }

    /// <summary>
    /// キューID(int)を指定して、その音声のポーズを解除します
    /// </summary>
    public void ResumeSound(int cueID)
    {
        if (playbackDict.TryGetValue(cueID.ToString(), out List<CriAtomExPlayback> playbacks))
        {
            foreach (var playback in playbacks) InternalResume(playback);
        }
    }

    private void InternalResume(CriAtomExPlayback playback)
    {
        if (playback.GetStatus() != CriAtomExPlayback.Status.Removed)
        {
            playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
        }
    }


    /// <summary>
    /// すべての音声を停止します。
    /// </summary>
    public void StopAllSound()
    {
        if (soundSource == null) return;

        soundSource.Stop();

        // 再生情報を更新
        CurrentCueID = -1;
        CurrentCueName = string.Empty;


    }

    /// <summary>
    /// キュー名(string)を指定して、その音声だけを停止します。
    /// </summary>
    public void StopSound(string cueName)
    {
        if (playbackDict.TryGetValue(cueName, out List<CriAtomExPlayback> playbacks))
        {
            // 同じ名前の音をすべて止める
            foreach (var playback in playbacks) InternalStop(playback);
        }
        else
        {
            Debug.LogWarning($"Cue: {cueName} の再生情報が見つかりません。");
        }
    }

    /// <summary>
    /// 指定したCueIDの音声だけを停止します。
    /// </summary>
    public void StopSound(int cueID)
    {
        if (playbackDict.TryGetValue(cueID.ToString(), out List<CriAtomExPlayback> playbacks))
        {
            foreach (var playback in playbacks) InternalStop(playback);
        }
        else
        {
            Debug.LogWarning($"CueID: {cueID} の再生情報が見つかりません。");
        }
    }

    /// <summary>
    /// 停止処理の内部関数
    /// </summary>
    private void InternalStop(CriAtomExPlayback playback)
    {
        if (playback.GetStatus() != CriAtomExPlayback.Status.Removed)
        {
            playback.Stop();
        }
    }




    // =======================================================
    // 全体音量の制御（動的距離減衰 / 段階設定）
    // =======================================================

    /// <summary>
    /// 動的移動時用：リスナーとの距離を計算し、全体音量を更新する
    /// </summary>
    public void UpdateVolumeByDistance()
    {
        isDynamicVolumeMode = true; // 距離による動的モードをON

        // 距離から減衰割合を計算し、全体音量の乗数とする
        currentSourceVolumeMultiplier = CalculationVolume(listener);

        // Inspector上でも変化がわかるように同期
        if (soundSource != null) soundSource.volume = currentSourceVolumeMultiplier;

        // 再生中のすべての音に適用
        UpdateAllActivePlaybacks();

    }

    /// <summary>
    /// 全体の音量を直接数値（0-1）で固定します。
    /// </summary>
    public void ChangeSourceVolume(float vol)
    {      

        isDynamicVolumeMode = false; // 固定モードなので距離モードはOFF
        currentSourceVolumeMultiplier = vol;
        if (soundSource != null) soundSource.volume = vol;// Inspectorと同期

        UpdateAllActivePlaybacks();
    }

    /// <summary>
    /// 一定距離の設定など、あらかじめ設定した配列(channel)の音量で全体を固定します。
    /// </summary>
    public void ChangeSourceVolume(int channel)
    {
        

        isDynamicVolumeMode = false; // 固定モードなので距離モードはOFF
        if (channel >= 0 && channel < volume.Length)
        {
            currentSourceVolumeMultiplier = volume[channel];
            if (soundSource != null) soundSource.volume = currentSourceVolumeMultiplier;

            UpdateAllActivePlaybacks();
        }
    }

    // =======================================================
    // 個別音量の制御（キューごとの手動調整用）
    // =======================================================

    public void ChangeVolume(string cueName, float vol)
    {
        savedVolumeDict[cueName] = vol; // 個別音量を上書き
        if (playbackDict.TryGetValue(cueName, out List<CriAtomExPlayback> playbacks))
        {
            // 同じ名前で鳴っているすべての音量を変える
            foreach (var playback in playbacks) ApplyCurrentVolume(cueName, playback);
        }
    }

    public void ChangeVolume(int cueID, float vol)
    {
        savedVolumeDict[cueID.ToString()] = vol;
        if (playbackDict.TryGetValue(cueID.ToString(), out List<CriAtomExPlayback> playbacks))
        {
            foreach (var playback in playbacks) ApplyCurrentVolume(cueID.ToString(), playback);
        }
    }
    // =======================================================
    // 音量適用の内部処理
    // =======================================================

        /// <summary>
        /// 辞書に登録されているすべての再生中音声に対して、現在の設定を適用する
        /// </summary>
    private void UpdateAllActivePlaybacks()
    {
        foreach (var kvp in playbackDict)
        {
            foreach (var playback in kvp.Value)
            {
                ApplyCurrentVolume(kvp.Key, playback);
            }
        }
    }

    /// <summary>
    /// 個別音量 × 全体音量 を計算して特定のPlaybackに適用する
    /// </summary>
    private void ApplyCurrentVolume(string key, CriAtomExPlayback playback)
    {
        if (soundSource == null || playback.GetStatus() == CriAtomExPlayback.Status.Removed) return;

        float individualVol = savedVolumeDict.TryGetValue(key, out float saved) ? saved : 1.0f;
        float finalVolume = individualVol * currentSourceVolumeMultiplier;

        soundSource.player.SetVolume(finalVolume);
        soundSource.player.Update(playback);
    }
    /// <summary>
    /// リスナーとの距離から、減衰の「割合(0.0〜1.0)」だけを計算して返す
    /// </summary>
    public float CalculationVolume(Transform targetTransform)
    {
        if (soundSource == null || targetTransform == null) return 1.0f;

        float distance = Vector3.Distance(transform.position, targetTransform.position);
        float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);
        return volumeCurve.Evaluate(normalizedDistance);
    }
}