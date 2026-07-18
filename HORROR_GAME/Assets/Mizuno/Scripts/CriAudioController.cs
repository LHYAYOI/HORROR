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

    private Dictionary<int, CriAtomExPlayback> playbackDict = new Dictionary<int, CriAtomExPlayback>();

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
        // 毎フレーム、リスナーとの距離に応じて音量を計算して変更する
        ChangeVolume(CalculationVolume(listener));
    }





    /// <summary>
    /// 音声を再生し、そのPlayback参照を辞書に保存します
    /// </summary>
    public void PlaySound(int cueID,bool loop = false)
    {
        if (soundSource != null)
        {
            soundSource.loop = loop;
            CriAtomExPlayback playback = soundSource.Play(cueID);

            // 辞書に保存する
            playbackDict[cueID] = playback;
        }
    }

    /// <summary>
    /// 指定したCueIDの音声だけポーズ（一時停止）します
    /// </summary>
    public void PauseSound(int cueID)
    {
        // 辞書に指定されたCueIDの再生データが存在するか確認
        if (playbackDict.TryGetValue(cueID, out CriAtomExPlayback playback))
        {
            // 現在のステータスを取得し、エラーや破棄済み（Removed）でなければポーズを実行
            if (playback.GetStatus() != CriAtomExPlayback.Status.Removed)
            {
                playback.Pause();
            }
        }
        else
        {
            Debug.LogWarning($"CueID: {cueID} の再生情報が見つかりません。");
        }
    }
    /// <summary>
    /// 指定したCueIDのポーズを解除（再開）します
    /// </summary>
    public void ResumeSound(int cueID)
    {
        if (playbackDict.TryGetValue(cueID, out CriAtomExPlayback playback))
        {
            if (playback.GetStatus() != CriAtomExPlayback.Status.Removed)
            {
                // ポーズ専用のモード指定でレジューム（再開）する
                playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
            }
        }
    }

    public void StopSound()
    {
        if (soundSource != null)
        {
            soundSource.Stop();
        }
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