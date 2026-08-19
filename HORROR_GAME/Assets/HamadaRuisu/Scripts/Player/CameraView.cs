using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CameraView : MonoBehaviour
{
    [SerializeField] GameObject m_batteryUI;
    [SerializeField] Slider m_batterySlider;

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
        m_batterySlider.value = currentBatteryAmount / maxBattery;
    }
}
