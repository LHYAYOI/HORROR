using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool m_isGameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetGameOverFlag(bool flag) 
    {
        m_isGameOver = flag;
    }

    public bool IsgameOver() 
    {
        return m_isGameOver;
    }
}
