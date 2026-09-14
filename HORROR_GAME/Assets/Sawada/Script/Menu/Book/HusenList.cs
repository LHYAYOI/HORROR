using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HusenList : MonoBehaviour
{
    [SerializeField] List<RectTransform> m_husenList;
    [SerializeField] PageChenge m_pageChenge;
    float m_moveMax = -647f;
    float m_moveMin = -624f;
    int m_currentNumber = 1;

    public void OnClickSelectHusen(int num)
    {
        if(!m_pageChenge.GetTurningFlag()) return;
        StartCoroutine(MovePositionX(m_husenList[num - 1], m_moveMin, m_moveMax, 0.2f));
        StartCoroutine(MovePositionX(m_husenList[m_currentNumber - 1], m_moveMax, m_moveMin, 0.2f));

        m_currentNumber = m_pageChenge.GetUwasaNumber();
    }

    private IEnumerator MovePositionX(RectTransform rect, float start, float end, float time)
    {
        float timer = 0.0f;

        while (timer < time)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / time);

            // 少し滑らかに
            t = Mathf.SmoothStep(0.0f, 1.0f, t);

            float x = Mathf.Lerp(start, end, t);

            Vector3 position = rect.anchoredPosition;
            position.x = x;
            rect.anchoredPosition = position;

            yield return null;
        }

        Vector3 finalPosition = rect.anchoredPosition;
        finalPosition.x = end;
        rect.anchoredPosition = finalPosition;
    }
}
