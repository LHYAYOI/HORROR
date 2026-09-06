using System;
using UnityEngine;
using UnityEngine.Events;

public class SmartPhoneDisplay : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private string m_stateName;


    [SerializeField]　GameObject m_displayObject;

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
            Debug.Log("指定アニメーション");
            m_displayObject.SetActive(true);
        }
        // オフ
        else
        {
            Debug.Log("指定アニメーション以外");
            m_displayObject.SetActive(false);
        }


        AnimatorClipInfo[] clipInfo = m_animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0) Debug.Log(clipInfo[0].clip.name);
    }
}
