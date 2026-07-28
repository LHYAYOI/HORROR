using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TesterController : MonoBehaviour
{
    bool isPause = false;

    public CriAudioController audioController;
    
    private float vol=0.5f;


    private void Start()
    {
        // audioController.ChangeSourceVolume(5);
        audioController.UpdateVolumeByDistance();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //SourceÇçƒê∂
            audioController.PlaySound(10);

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //SourceÇçƒê∂
            audioController.ChangeVolume(2, vol);

            audioController.PlaySound(2,false);
        }
        // ==================================
        // å¬ï âπó í≤êÆ
        // ==================================
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            vol += 0.05f;
            if(vol > 1.0f) vol = 1.0f;
            audioController.ChangeVolume(2, vol);
            Debug.Log("âπó ïœçX vol=" + vol);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            vol -= 0.05f;
            if(vol < 0.0f) vol = 0.0f;
            audioController.ChangeVolume(2, vol);
        }
        // ==================================
        // âπåπÇ∆ÇÃãóó£Ç…ÇÊÇÈâπó í≤êÆÇÃÉeÉXÉg
        // ==================================

        // âπåπÇ™ìÆÇ´âÒÇÈVer
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

        // âπåπÇ™Ç®Ç∆Ç»ÇµÇ≠ÅAãóó£ÇíiäKÇ≈ïœÇ¶ÇÈVer
        if (Input.GetKeyDown(KeyCode.T))
        {
            audioController.ChangeSourceVolume(0);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            audioController.ChangeSourceVolume(1);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            audioController.ChangeSourceVolume(2);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            audioController.ChangeSourceVolume(4);
        }








        // Pause/ResumeÇÃêÿÇËë÷Ç¶
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isPause == false) {
                //SourceÇàÍéûí‚é~
                audioController.PauseSound(10);
                // çƒê∂í‚é~
                audioController.StopSound(2);
                isPause = true;
                Debug.Log("Pause");
            }
            else
            {
                //SourceÇçƒê∂
                audioController.ResumeSound(10);
                isPause = false;
                Debug.Log("Resume");
            }
        }

        // çƒê∂í‚é~
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            audioController.StopAllSound();
        }




    }

}
