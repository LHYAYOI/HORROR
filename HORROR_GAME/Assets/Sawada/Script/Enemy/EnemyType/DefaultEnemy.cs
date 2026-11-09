using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
                    // 同じ縦横
                    if (currentPosi.x == targetPosi.x || currentPosi.y == targetPosi.y)
                    {
                        List<AreaController> areaData = mapData.GetAreaData();

                        // ターゲット方向
                        Vector2Int diff = targetPosi - currentPosi;

                        Vector2Int direction = new Vector2Int(Math.Sign(diff.x), Math.Sign(diff.y));

                        // 現在位置の1マス先から確認
                        Vector2Int checkPosi = currentPosi + direction;

                        while (checkPosi != targetPosi)
                        {
                            // マップ範囲外
                            if (checkPosi.x < 0 || checkPosi.x >= mapData.GetWidth() || checkPosi.y < 0 || checkPosi.y >= mapData.GetHeight()) return false;

                            int targetIndex = checkPosi.y * mapData.GetWidth() + checkPosi.x;

                            // 壁がある
                            if (areaData[targetIndex] == null) return false;

                            checkPosi += direction;
                        }

                        return true;
                    }

                    break;
                }
        }

        return false;
    }
}