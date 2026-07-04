using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DefaultEnemy",
    menuName = "Scriptable Objects/Enemy/DefaultEnemy")]
public class DefaultEnemy : ScriptableObject
{
    [Serializable] public class DistanceImageData
    {
        [SerializeField] private Sprite m_sprite;
        [SerializeField] private int m_distance;
        [SerializeField] private Vector2Int m_size;
        [SerializeField] private Vector2 m_adjustment;

        public Sprite Sprite => m_sprite;
        public int Distance => m_distance;
        public Vector2Int Size => m_size;
        public Vector2 Adjustment => m_adjustment;
    }

    [SerializeField] List<DistanceImageData> m_images = new();

    [SerializeField] int m_speed;
    [SerializeField] Vector2Int m_spawnPosition;

    [SerializeField] float m_searchRange;

    public enum SearchMethod
    {
        circle = 0,
        line
    }

    [SerializeField] SearchMethod m_searchMethod;

    [SerializeField] List<EnemyStateBase> m_stateList = new();

    [SerializeField] List<AreaController> m_noAreaData = new();

    // Getter
    public int GetSpeed()
    {
        return m_speed;
    }

    public Vector2Int GetSpawnPosition()
    {
        return m_spawnPosition;
    }

    public List<EnemyStateBase> GetStateList()
    {
        return m_stateList;
    }

    public List<AreaController> GetNoAreaData()
    {
        return m_noAreaData;
    }

    public IReadOnlyList<DistanceImageData> GetImageDataList()
    {
        return m_images;
    }

    // 指定距離に対応するものを取得する。
    public DistanceImageData GetImage(Vector2Int A, Vector2Int B)    // 自動
    {
        DistanceImageData result = null;

        // 距離
        int distance = (int)Mathf.Abs(A.x - B.x) + Mathf.Abs(A.y - B.y);
        // 判別
        result = GetImage(distance);

        return result;
    }
    public DistanceImageData GetImage(int distance)    // 明示
    {
        DistanceImageData result = null;
        int nearestDistance = int.MinValue;

        foreach (DistanceImageData imageData in m_images)
        {
            if (imageData.Sprite == null)
            {
                continue;
            }

            if (imageData.Distance <= distance &&
                imageData.Distance > nearestDistance)
            {
                nearestDistance = imageData.Distance;
                result = imageData;
            }
        }

        // 条件に合うものがなければ先頭画像を返す
        if (result == null && m_images.Count > 0)
        {
            result = m_images[0];
        }

        return result;
    }

    // targetを追いかけるか判定
    public bool CanChaseTarget(Vector2Int current, Vector2Int target)
    {
        switch (m_searchMethod)
        {
            case SearchMethod.circle:
                {
                    float distance = Vector2Int.Distance(current, target);

                    if (distance <= m_searchRange)
                    {
                        Debug.Log("circle");
                        return true;
                    }

                    break;
                }

            case SearchMethod.line:
                {
                    int differenceX = Mathf.Abs(current.x - target.x);
                    int differenceY = Mathf.Abs(current.y - target.y);

                    bool sameColumn =
                        differenceX == 0 &&
                        differenceY <= m_searchRange;

                    bool sameRow =
                        differenceY == 0 &&
                        differenceX <= m_searchRange;

                    if (sameColumn || sameRow)
                    {
                        //Debug.Log("line");
                        return true;
                    }

                    break;
                }
        }

        return false;
    }
}