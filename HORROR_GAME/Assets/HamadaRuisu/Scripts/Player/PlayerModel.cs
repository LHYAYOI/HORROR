using UnityEngine;

public class PlayerModel : MonoBehaviour
{

    [SerializeField] private int m_maxBatteryValue;

    int m_currentBatteryValue;

    void Start()
    {
        m_currentBatteryValue = m_maxBatteryValue;
    }

    public int GetMaxBatteryValue() 
    {
        return m_maxBatteryValue;
    }

    public int GetCurrentBatteryValue()
    {
        return m_currentBatteryValue;
    }

    public void AddBattery(int amount)
    {
        m_currentBatteryValue += amount;

        if (m_currentBatteryValue > m_maxBatteryValue)
        {
            m_currentBatteryValue = m_maxBatteryValue;
        }
    }

    public void DrainBattery(int amount)
    {
        m_currentBatteryValue -= amount;

        if (m_currentBatteryValue < 0)
        {
            m_currentBatteryValue = 0;
        }
    }
}
