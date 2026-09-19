using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HidePlaceType
{
    Bicycle,
    Car,
    GomiB,
    GomiT,
    Kago,
    Tire
}

public class HidePlaceUIController : MonoBehaviour
{

    public static HidePlaceUIController instance;

    [SerializeField] private List<Sprite> m_hideImageList;

    [SerializeField] private Image m_hideUI;

    HidePlaceType m_currentHidePlaceType;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_hideUI.enabled = false;
    }

    public void SetCurrentHidePlaceType(HidePlaceType hidePlaceType) 
    {
        m_currentHidePlaceType = hidePlaceType;
    }

    public void SetHideUIActive() 
    {
        m_hideUI.enabled = true;
        m_hideUI.sprite = m_hideImageList[(int)m_currentHidePlaceType];
    }

    public void SetHideUIInactive() 
    {
        m_hideUI.enabled = false;
    }
}
