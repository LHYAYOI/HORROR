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

    private Dictionary<string, CriAtomExPlayback> playbackDict = new Dictionary<string, CriAtomExPlayback>();

    // 現在再生中のキュー情報（何も再生していない場合は ID: -1 / Name: "" とする）
    public int CurrentCueID { get; private set; } = -1;
    public string CurrentCueName { get; private set; } = string.Empty;

    // 現在再生中のものを追跡するための最新のPlayback
    private CriAtomExPlayback currentPlayback;

    [SerializeField]
    private Transform listener;

    [SerializeField, Range(0.0f, 1.0f)]
    private float[] volume = new float[6];

    [Header("Distance Attenuation Settings")]
    [Tooltip("最小距離（これより近いと音量最大）")]
    [SerializeField] private float minDistance = 1f;
    [Tooltip("最大距離（これより遠いと音量0）")]
    [SerializeField] private float maxDistance = 100f;
    [Tooltip("距離に対する音量の減衰カーブ（横軸:距離割合 0~1, 縦軸:音量 0~1）")]
    [SerializeField] private AnimationCurve volumeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private void Awake()
    {
        // アタッチされているCriAtomSourceを取得
        soundSource = GetComponent<CriAtomSourceForAsset>();
    }

    private void Update()
    {


        // リスナーとの距離に応じて音量を計算して変更する→何か変更が起きた場合にのみ音量を変更したいけど…('ω')
        ChangeVolume(CalculationVolume(listener));

        // 現在再生中のキューがあると記録されている場合のみ監視
        if (CurrentCueID != -1 || !string.IsNullOrEmpty(CurrentCueName))
        {
            CriAtomExPlayback.Status status = currentPlayback.GetStatus();

            // 再生が終了、または破棄された場合
            if ( status == CriAtomExPlayback.Status.Removed)
            {
                // 再生中ステータスをリセット
                CurrentCueID = -1;
                CurrentCueName = string.Empty;
            }
        }
    }



    /// <summary>
    /// キュー名(string)を指定して音声を再生します
    /// </summary>
    public void PlaySound(string cueName,bool loop=true)
    {
        if (soundSource == null || string.IsNullOrEmpty(cueName)) return;

        soundSource.loop = loop;

        CriAtomExPlayback playback = soundSource.Play(cueName);

        // キュー名をキーとして保存
        playbackDict[cueName] = playback;

        // 現在の再生情報を更新
        currentPlayback = playback;
        CurrentCueName = cueName;
        CurrentCueID = -1; // 名前再生時はIDが不明なため一旦リセット（※ACBからIDを引くことも可能ですが、簡易化のため）
    }

    /// <summary>
    /// キューID(int)を指定して音声を再生します
    /// </summary>
    public void PlaySound(int cueID,bool loop = true)
    {
        if (soundSource == null) return;
        
        soundSource.loop = loop;
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
        if (soundSource != null) return;

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
        }
    }

    /// <summary>
    /// 音源の3D位置を更新します (2D座標指定用)
    /// </summary>
    public void SetPosition(Vector2 newPosition)
    {
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);
    }

    /// <summary>
    /// 対象(プレイヤーやカメラなどリスナー)のTransformを受け取り、
    /// このオブジェクトとの距離に応じてCriAtomSourceの音量を変更します。
    /// </summary>
    public float CalculationVolume(Transform targetTransform)
    {
        if (soundSource == null || targetTransform == null) 
        { 
            Debug.LogWarning("CriAtomSource or targetTransform is null. Cannot calculate volume.");
            return -1.0f;
        }

        // 音源（このオブジェクト）と対象（targetTransform）の距離を計算
        float distance = Vector3.Distance(transform.position, targetTransform.position);

        // 距離を 0.0(minDistance) 〜 1.0(maxDistance) の割合に変換（自動クランプ）
        float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // AnimationCurve を使って、距離の割合から音量(0~1)を計算
        float calculatedVolume = volumeCurve.Evaluate(normalizedDistance);

        //// CriAtomSourceの音量に適用
        //atomSource.volume = calculatedVolume;

        return calculatedVolume;
    }
}