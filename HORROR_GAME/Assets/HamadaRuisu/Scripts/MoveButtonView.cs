using UnityEngine;
using UnityEngine.UI;


public class MoveButtonView : MonoBehaviour
{
    [SerializeField] Image m_buttonImage;

    public void UpdateGauge(float fillAmount) 
    {
        m_buttonImage.fillAmount = fillAmount;
    }
}
