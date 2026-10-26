using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private AreaManager m_areaManager;
    [SerializeField] private Vector2Int m_start;
    [SerializeField] private Vector2Int m_goal;
    [SerializeField] private Vector2Int m_current;
    private List<AreaController> m_areaData;

    private void Start()
    {
        m_areaData = m_areaManager.GetMapData().GetAreaData();
    }

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
        }
    }

    public void TestDebugLog(Vector2Int start, Vector2Int goal)
    {
        Debug.Log($"E: {m_areaManager.GetMapData().GetArea(start).name} {start}");
    }

    // 上下左右
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private class Node
    {
        public Vector2Int Position;

        public int m_startGoalDistance;
        public int m_currentDistance;

        public int m_finfPriority => m_startGoalDistance + m_currentDistance;

        public Node Parent;

        public Node(Vector2Int position)
        {
            Position = position;
            m_startGoalDistance = int.MaxValue;
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        List<Node> openList = new List<Node>();

        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

        Node startNode = new Node(start);

        startNode.m_startGoalDistance = 0;
        startNode.m_currentDistance = GetDistance(start, goal);

        openList.Add(startNode);
        nodes.Add(start, startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            // 優先度が最も高いNodeを取得
            foreach (Node node in openList)
            {
                if (node.m_finfPriority < currentNode.m_finfPriority ||
                    (node.m_finfPriority == currentNode.m_finfPriority &&
                     node.m_currentDistance < currentNode.m_currentDistance))
                {
                    currentNode = node;
                }
            }

            // ゴールに到達
            if (currentNode.Position == goal)
            {
                return CreatePath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.Position);

            // 上下左右の1マスだけ探索
            foreach (Vector2Int direction in Directions)
            {
                Vector2Int nextPosition = currentNode.Position + direction;

                // nextPositionが範囲外
                if (nextPosition.x < 0 ||
                    nextPosition.y < 0 ||
                    m_areaManager.GetMapData().GetWidth() <= nextPosition.x ||
                    m_areaManager.GetMapData().GetHeight() <= nextPosition.y)
                {
                    continue;
                }

                // 移動可能か(壁)
                if (!CanMove(nextPosition))
                {
                    continue;
                }

                // すでに探索済み
                if (closedList.Contains(nextPosition))
                {
                    continue;
                }

                if (!nodes.TryGetValue(nextPosition, out Node nextNode))
                {
                    nextNode = new Node(nextPosition);
                    nodes.Add(nextPosition, nextNode);
                }

                // 1マス分のコストを加算
                int newG = currentNode.m_startGoalDistance + 1;

                if (newG < nextNode.m_startGoalDistance)
                {
                    nextNode.m_startGoalDistance = newG;

                    nextNode.m_currentDistance = GetDistance(nextPosition, goal);

                    nextNode.Parent = currentNode;

                    if (!openList.Contains(nextNode))
                    {
                        openList.Add(nextNode);
                    }
                }

                // 目的地に到達した場合、経路を復元して返す
                if (nextPosition == goal)
                {
                    return CreatePath(nextNode);
                }
            }
        }

        return new List<Vector2Int>();
    }

    // ゴールまでの推定距離
    private int GetDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) +
               Mathf.Abs(from.y - to.y);
    }

    // 移動可能判定
    private bool CanMove(Vector2Int position)
    {
        // 壁ではないか
        int targetIndex = position.y/*y*/ * m_areaManager.GetMapData().GetWidth() + position.x/*x*/;
        if (m_areaData[targetIndex] == null)
        {
            return false;
        }

        return true;
    }

    // 経路を復元
    private List<Vector2Int> CreatePath(Node goalNode)
    {
        List<Vector2Int> path =
            new List<Vector2Int>();

        Node currentNode = goalNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();

        return path;
    }

    // 現在位置と経路から、次の移動先を取得
    public Vector2Int GetNextPosition(Vector2Int currentPosition, List<Vector2Int> path, int moveSpeed)
    {
        if (path == null || path.Count == 0 || moveSpeed <= 0)
        {
            return currentPosition;
        }

        // 現在位置が経路の何番目か取得
        int currentIndex = path.IndexOf(currentPosition);

        if (currentIndex == -1)
        {
            return currentPosition;
        }

        // 次のマスが存在しない
        if (currentIndex + 1 >= path.Count)
        {
            return currentPosition;
        }

        // 最初の移動方向
        Vector2Int direction = path[currentIndex + 1] - currentPosition;

        Vector2Int nextPosition = currentPosition;

        // 指定された移動距離まで確認
        for (int i = 1; i <= moveSpeed; i++)
        {
            int nextIndex = currentIndex + i;

            // 経路の終端
            if (nextIndex >= path.Count)
            {
                break;
            }

            Vector2Int targetPosition = path[nextIndex];

            // 同じ方向に進んでいるか
            if (targetPosition - nextPosition != direction)
            {
                break;
            }

            // 移動可能か
            if (!CanMove(targetPosition))
            {
                break;
            }

            nextPosition = targetPosition;
        }

        return nextPosition;
    }
}