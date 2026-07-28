using UnityEngine;

public class TesterController : MonoBehaviour
{
    
    public CriAudioController audioController;

    bool isPause = false;

    private float vol=0.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //Sourceを再生
            audioController.PlaySound(10);

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Sourceを再生
            audioController.ChangeVolume("カメラをしまう", vol);

            audioController.PlaySound("カメラをしまう",true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            vol += 0.05f;
            if(vol > 1.0f) vol = 1.0f;
            audioController.ChangeVolume("カメラをしまう", vol);
            Debug.Log("音量変更 vol=" + vol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            vol -= 0.05f;
            if(vol < 0.0f) vol = 0.0f;
            audioController.ChangeVolume("カメラをしまう", vol);
        }


        // 音源との距離による音量調整のテスト
        if (Input.GetKeyDown(KeyCode.W))
        {
           transform.position += new Vector3(0, 0, 1);
            audioController.UpdateVolumeByDistance();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += new Vector3(0, 0, -1);
            audioController.UpdateVolumeByDistance();
        }




        // Pause/Resumeの切り替え
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isPause == false) {
                //Sourceを一時停止
                audioController.PauseSound(10);
                audioController.PauseSound(0);
                isPause = true;
            }
            else
            {
                //Sourceを再生
                audioController.ResumeSound(10);
                isPause = false;
            }
        }




    }

}
