using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("遷移先")]
    [SerializeField]
    private SceneReference m_nextSceneReference = new SceneReference();

    bool m_loadingFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene() 
    {
        if (m_loadingFlag)
        {
            return;
        }

        if (m_nextSceneReference == null ||
            !m_nextSceneReference.HasSceneFlag())
        {
            Debug.LogError(
                "TitleController：遷移先シーンが設定されていません。");

            return;
        }

        m_loadingFlag = true;

        SceneManager.LoadScene(m_nextSceneReference.GetScenePath());
    }
}
