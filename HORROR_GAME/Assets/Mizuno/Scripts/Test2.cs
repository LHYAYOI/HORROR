using UnityEngine;

public class Test2 : MonoBehaviour
{
    public CriAudioController audioController;
    private float vol = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Sourceを再生
            audioController.ChangeVolume("カメラをしまう", vol);

            audioController.PlaySound("カメラをしまう", false);
        }
        // ==================================
        // 個別音量調整
        // ==================================
        if (Input.GetKeyDown(KeyCode.X))
        {
            vol += 0.05f;
            if (vol > 1.0f) vol = 1.0f;
            audioController.ChangeVolume("カメラをしまう", vol);
            Debug.Log("音量変更 vol=" + vol);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            vol -= 0.05f;
            if (vol < 0.0f) vol = 0.0f;
            audioController.ChangeVolume("カメラをしまう", vol);
            Debug.Log("音量変更 vol=" + vol);

        }

    }
}
