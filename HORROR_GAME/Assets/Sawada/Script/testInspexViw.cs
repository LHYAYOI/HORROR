using UnityEngine;

public class testInspexViw : MonoBehaviour
{
    public SettingType m_settingType;

    public int m_aValue;
    public float m_aSpeed;

    public int m_bValue;
    public float m_bSpeed;
}

public enum SettingType
{
    A,
    B
}