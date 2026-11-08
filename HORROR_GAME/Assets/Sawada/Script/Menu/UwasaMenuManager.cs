using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UwasaMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject m_objects;
    private bool m_viewMode = false;
    [SerializeField] private Animator m_animator;

    [SerializeField] private List<TextMeshProUGUI> m_texts;
    private float m_timer = 0f;

    private void Start()
    {
        m_objects.SetActive(false);

        foreach (var item in m_texts)
        {
            item.color = new(1f, 1f, 1f, 0f);
        }
        m_timer = 0f;
        Debug.Log("reset");
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SetView();
        }

        // 再生終了後の処理
        if (IsAnimationFinished("Base Layer.PullSmartPhone"))
        {
            foreach (var item in m_texts)
            {
                float alpha = m_timer * 8;
                alpha = alpha > 1f ? 1f : alpha;
                item.color = new(1f, 1f, 1f, alpha);
            }
            Debug.Log("加算中");
            m_timer += Time.deltaTime;
        }
    }

    // 表示非表示
    public void SetView()
    {
        // m_objectsの表示非表示
        m_viewMode = !m_viewMode;
        m_objects.SetActive(m_viewMode);

        // m_animatorの実行
        if (m_viewMode)
            m_animator.SetTrigger("PullSmartPhone");
        else
        {
            m_animator.SetTrigger("RemoveSmartPhone");

            foreach (var item in m_texts)
            {
                item.color = new(1f, 1f, 1f, 0f);
            }
            m_timer = 0f;
            Debug.Log("reset");
        }
    }

    // 再生中かどうか
    private bool IsAnimationFinished(string stateName, int layer = 0)
    {
        AnimatorStateInfo state = m_animator.GetCurrentAnimatorStateInfo(layer);

        return state.IsName(stateName) && state.normalizedTime >= 0.8f;
    }
}
