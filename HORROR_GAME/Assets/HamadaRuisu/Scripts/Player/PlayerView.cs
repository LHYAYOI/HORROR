using UnityEngine;
using System.Collections.Generic;

public class PlayerView : MonoBehaviour
{
    [SerializeField] GameObject m_batteryUI;
    [SerializeField] List<GameObject> m_batteryGages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowBatteryUI() 
    {
        m_batteryUI.SetActive(true);
    }

    public void HideBatteryUI() 
    {
        m_batteryUI.SetActive(false);
    }

    public void UpdateBatteryGages(int maxBattery, int currentBatteryAmount)
    {
        int updateBatteryAmount = maxBattery - currentBatteryAmount;

        for (int i = 0; i < maxBattery; i++)
        {
            if (i < updateBatteryAmount)
            {
                m_batteryGages[i].SetActive(false);
            }
            else
            {
                m_batteryGages[i].SetActive(true);
            }
        }
    }
}
