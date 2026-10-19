using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] SceneLoader m_sceneLoader;
    [SerializeField] kaiiManager m_kaiManager;
    bool m_isGameOver = false;

    private void Awake()
    {
        instance = this;
    }

    public void CheckIfClearedGame() 
    {
        if (m_kaiManager.IsAllCaptured())
        {
            m_sceneLoader.LoadGameClearSceneWithFade();
        }
    }

    public void SetGameOverFlag(bool flag) 
    {
        m_isGameOver = flag;
    }

    public bool IsGameOver() 
    {
        return m_isGameOver;
    }
}
