using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    [Header("遷移先")]
    [SerializeField]
    private SceneReference m_nextSceneReference = new SceneReference();

    [Header("入力設定")]
    [SerializeField]
    private bool m_enableKeyboardStartFlag = true;

    private bool m_loadingFlag;

    private void Update()
    {
        if (!m_enableKeyboardStartFlag || m_loadingFlag)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
    }

    public void StartGame()
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

        SceneManager.LoadScene(
            m_nextSceneReference.GetScenePath());
    }
}