using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SmartPhoneDisplay : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private string m_stateName;

    private void Update()
    {
        AnimatorStateInfo stateInfo =
            m_animator.GetCurrentAnimatorStateInfo(0);

        bool isTargetState =
            stateInfo.IsName(m_stateName) &&
            !m_animator.IsInTransition(0);

        // オン
        if (isTargetState)
        {
            //Debug.Log("指定アニメーション");
            for (int i = transform.childCount - 1; i >= 0; i--) transform.GetChild(i).gameObject.SetActive(true);
        }
        // オフ
        else
        {
            //Debug.Log("指定アニメーション以外");
            for (int i = transform.childCount - 1; i >= 0; i--) transform.GetChild(i).gameObject.SetActive(false);
        }

        AnimatorClipInfo[] clipInfo = m_animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0) Debug.Log(clipInfo[0].clip.name);
    }
}
