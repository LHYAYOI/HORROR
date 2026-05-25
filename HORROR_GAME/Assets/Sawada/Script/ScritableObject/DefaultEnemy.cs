using UnityEditor.Searcher;
using UnityEngine;
using static DefaultEnemy;

[CreateAssetMenu(fileName = "DefaultEnemy", menuName = "Scriptable Objects/Enemy/DefaultEnemy")]
public class DefaultEnemy : ScriptableObject
{
    public int m_speed;
    public Vector2 m_size;
    public Sprite m_image;
    public Vector2Int m_spawnPosition;

    [Header("索敵範囲")]
    public float m_searchRange;
    public enum SearchMethod
    {
        circle = 0,
        line
    }
    public SearchMethod m_searchMethod;


    // atargetを追いかけるか判定
    public bool CanChaseTarget(Vector2Int current, Vector2Int target)
    {
        switch (m_searchMethod)
        {
            case SearchMethod.circle:
                if (Mathf.Abs(Vector2Int.Distance(target, current)) <= m_searchRange)
                {
                    Debug.Log("circle");
                    return true;
                }
                break;
            case SearchMethod.line:
                if (current.x - target.x == 0 && current.y - target.y <= m_searchRange)
                {
                    Debug.Log("line");
                    return true;
                }
                if (current.x - target.x <= m_searchRange && current.y - target.y == 0)
                {
                    Debug.Log("line");
                    return true;
                }
                break;
        }

        return false;
    }
}
