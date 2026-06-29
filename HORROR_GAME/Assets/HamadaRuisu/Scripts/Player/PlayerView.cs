using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerView : MonoBehaviour
{
    [SerializeField] GameObject m_batteryUI;
    [SerializeField] TextMeshProUGUI m_batteryPercentage;

    public void ShowBatteryUI() 
    {
        m_batteryUI.SetActive(true);
    }

    public void HideBatteryUI() 
    {
        m_batteryUI.SetActive(false);
    }

    public void UpdateBatteryPercentage(float maxBattery, float currentBatteryAmount)
    {
        m_batteryPercentage.text = currentBatteryAmount.ToString() + "%";
    }
}
