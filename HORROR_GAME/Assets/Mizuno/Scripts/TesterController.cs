using UnityEngine;

public class TesterController : MonoBehaviour
{
    
    public CriAudioController audioController;


    bool isPause = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //Source‚ğÄ¶
            audioController.PlaySound(0,true);

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Source‚ğÄ¶
            audioController.PlaySound("‘«‰¹",false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            audioController.ChangeVolume(0);
        }



        // Pause/Resume‚ÌØ‚è‘Ö‚¦
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isPause == false) { 
                //Source‚ğˆê’â~
                audioController.PauseSound(0);
                isPause = true;
            }
            else
            {
                //Source‚ğÄ¶
                audioController.ResumeSound(0);
                isPause = false;
            }
        }




    }

}
