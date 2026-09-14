using System;
using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("遷移先")]

    [SerializeField] private SceneReference m_GameOverSceneReference = new SceneReference();
    [SerializeField] private SceneReference m_GameClearSceneReference = new SceneReference();
    [SerializeField] private Image m_fadeImage;
    [SerializeField] float fadeTime = 1.0f;

    bool m_loadingFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) 
        {
            LoadGameClearSceneWithFade();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            LoadGameOverSceneWithFade();
        }
    }

    public void LoadGameClearSceneWithFade() 
    {
        StartCoroutine(LoadNextSceneWithFadeCoroutine(m_GameClearSceneReference, fadeTime));
    }

    public void LoadGameOverSceneWithFade()
    {
        StartCoroutine(LoadNextSceneWithFadeCoroutine(m_GameOverSceneReference, fadeTime));
    }

    private IEnumerator LoadNextSceneWithFadeCoroutine(SceneReference scene, float fadeTime = 1.0f) 
    {
        if (m_loadingFlag)
        {
            yield break;
        }

        m_loadingFlag = true;

        float deltaTime = 0.0f;

        while (deltaTime < fadeTime) 
        {
            deltaTime += Time.deltaTime;

            float rate = Mathf.Clamp01(deltaTime / fadeTime);

            Color startColor = new Color(0, 0, 0, 0);
            Color endColor = new Color(0, 0, 0, 1);

            m_fadeImage.color = Color.Lerp(startColor, endColor, rate);

            yield return null;
        }

        SceneManager.LoadScene(scene.GetScenePath());
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
