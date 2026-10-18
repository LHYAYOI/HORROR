using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DefaultEnemy",
    menuName = "Scriptable Objects/Enemy/DefaultEnemy")]
public class DefaultEnemy : ScriptableObject
{
    [Serializable]
    public class DistanceImageData
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
    public bool CanChaseTarget(Vector2Int currentPosi, Vector2Int targetPosi, MapData mapData)
    {
        switch (m_searchMethod)
        {
            case SearchMethod.circle:
                {
                    float distance = Vector2Int.Distance(currentPosi, targetPosi);

                    if (distance <= m_searchRange)
                    {
                        Debug.Log("circle");
                        return true;
                    }

                    break;
                }

            case SearchMethod.line:
                {
                    //Vector2Int difference = currentPosi - targetPosi;
                    //Vector2Int differenceAbs = new(Math.Abs(difference.x), Math.Abs(difference.y));
                    //int diff = 0;

                    //bool sameColumn =
                    //    differenceAbs.x == 0 &&
                    //    differenceAbs.y <= m_searchRange;

                    //bool sameRow =
                    //    differenceAbs.y == 0 &&
                    //    differenceAbs.x <= m_searchRange;

                    //if (!sameColumn && !sameRow) return false;

                    //if (sameColumn)
                    //{
                    //    diff = differenceAbs.y;
                    //}
                    //else if (sameRow)
                    //{
                    //    diff = differenceAbs.x;
                    //}

                    //// 一マスずつ確認
                    //for (int i = 0; i < differenceAbs.x; i++)
                    //{
                    //    for (int j = 0; j < m_noAreaData.Count; j++)
                    //    {
                    //        if (mapData.GetArea(currentPosi + new Vector2Int(i, 0)).GetAreaName() == m_noAreaData[j].GetAreaName())
                    //        {
                    //            Debug.Log("障害物あり x");
                    //            return false;
                    //        }
                    //    }
                    //}
                    //for (int i = 0; i < differenceAbs.y; i++)
                    //{
                    //    for (int j = 0; j < m_noAreaData.Count; j++)
                    //    {
                    //        if (mapData.GetArea(currentPosi + new Vector2Int(0, i)).GetAreaName() == m_noAreaData[j].GetAreaName())
                    //        {
                    //            Debug.Log("障害物あり y");
                    //            return false;
                    //        }
                    //    }
                    //}

                    //return true;

                    // 対象への方向
                    Vector2Int difference = targetPosi - currentPosi;

                    // 距離
                    Vector2Int differenceAbs = new(
                        Math.Abs(difference.x),
                        Math.Abs(difference.y)
                    );

                    // 同じ列
                    bool sameColumn =
                        differenceAbs.x == 0 &&
                        differenceAbs.y <= m_searchRange;

                    // 同じ行
                    bool sameRow =
                        differenceAbs.y == 0 &&
                        differenceAbs.x <= m_searchRange;

                    if (!sameColumn && !sameRow)
                    {
                        return false;
                    }

                    // 対象までのマス数
                    int distance = differenceAbs.x + differenceAbs.y;

                    // 確認する方向
                    Vector2Int direction = new(
                        Math.Sign(difference.x),
                        Math.Sign(difference.y)
                    );

                    // 敵自身と対象のマスは確認しない
                    for (int i = 1; i < distance; i++)
                    {
                        Vector2Int checkPos = currentPosi + direction * i;

                        AreaController area = mapData.GetArea(checkPos);

                        if (area == null)
                        {
                            Debug.LogWarning($"マスが存在しません: {checkPos}");
                            return false;
                        }

                        // 障害物確認
                        foreach (AreaController noArea in m_noAreaData)
                        {
                            if (noArea == null)
                            {
                                continue;
                            }

                            if (area.GetAreaName() == noArea.GetAreaName())
                            {
                                Debug.Log($"障害物あり: {checkPos}");
                                return false;
                            }
                        }
                    }

                    return true;
                }
        }

        return false;
    }
}