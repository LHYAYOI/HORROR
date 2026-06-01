using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    private bool m_hideFlag;

    void Start()
    {
        m_hideFlag = false;
    }

    void Update()
    {
        
    }

    public bool GetHideFlag()
    {
        return m_hideFlag;
    }
    public void SetHideFlag(bool flag)
    {
        m_hideFlag = flag;
    }
}
