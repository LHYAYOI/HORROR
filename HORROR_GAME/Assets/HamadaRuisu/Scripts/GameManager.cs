using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] SceneLoader m_sceneLoader;
    [SerializeField] kaiiManager m_kaiManager;
    bool m_isGameOver = false;

    bool m_isGameCleared = false;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void CheckIfClearedGame() 
    {
        if (m_kaiManager.IsAllCaptured())
        {
            //m_sceneLoader.LoadGameClearScene();
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
