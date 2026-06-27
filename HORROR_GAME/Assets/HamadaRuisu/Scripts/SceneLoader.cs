using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("遷移先")]

    [SerializeField] private SceneReference m_GameOverSceneReference = new SceneReference();
    [SerializeField] private SceneReference m_GameClearSceneReference = new SceneReference();

    bool m_loadingFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameOverScene() 
    {
        if (m_loadingFlag)
        {
            return;
        }

        if (m_GameOverSceneReference == null ||
            !m_GameOverSceneReference.HasSceneFlag())
        {
            Debug.LogError(
                "TitleController：遷移先シーンが設定されていません。");

            return;
        }

        m_loadingFlag = true;

        SceneManager.LoadScene(m_GameOverSceneReference.GetScenePath());
    }

    public void LoadGameClearScene()
    {
        if (m_loadingFlag)
        {
            return;
        }

        if (m_GameClearSceneReference == null ||
            !m_GameClearSceneReference.HasSceneFlag())
        {
            Debug.LogError(
                "TitleController：遷移先シーンが設定されていません。");

            return;
        }

        m_loadingFlag = true;

        SceneManager.LoadScene(m_GameClearSceneReference.GetScenePath());
    }
}
