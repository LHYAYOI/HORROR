using CriWare;
using UnityEngine;

public class TesterController : MonoBehaviour
{
    public CriAudioController audioController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //Source‚ğÄ¶
            audioController.PlaySound(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Source‚ğÄ¶
            audioController.PlaySound(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Source‚ğÄ¶
            audioController.StopSound();
        }
    }

}
