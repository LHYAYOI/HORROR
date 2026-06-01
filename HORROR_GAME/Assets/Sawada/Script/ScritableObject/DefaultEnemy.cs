using UnityEditor.Searcher;
using UnityEngine;
using static DefaultEnemy;

[CreateAssetMenu(fileName = "DefaultEnemy", menuName = "Scriptable Objects/Enemy/DefaultEnemy")]
public class DefaultEnemy : ScriptableObject
{
    [SerializeField] int m_speed;
    [SerializeField] Vector2 m_size;
    [SerializeField] Sprite m_image;
    [SerializeField] Vector2Int m_spawnPosition;

    [Header("索敵範囲")]
    [SerializeField] float m_searchRange;
    public enum SearchMethod
    {
        circle = 0,
        line
    }
    public SearchMethod m_searchMethod;


    // ゲット
    public int GetSpeed() { return m_speed; }
    public Vector2 GetSize() { return m_size; }
    public Sprite GetImage() { return m_image; }
    public Vector2Int GetSpawnPosition() { return m_spawnPosition; }


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
