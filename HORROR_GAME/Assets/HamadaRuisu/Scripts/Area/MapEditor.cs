#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{
    AreaController selectedType;

    public override void OnInspectorGUI()
    {
        MapData map = (MapData)target;

        if (GUILayout.Button("Init"))
        {
            map.Init();
            EditorUtility.SetDirty(map);
        }

        // サイズ変更
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        int newWidth = EditorGUILayout.IntField("Width", map.GetWidth());
        int newHeight = EditorGUILayout.IntField("Height", map.GetHeight());

        if (EditorGUI.EndChangeCheck())
        {
            Resize(map, newWidth, newHeight);
            EditorUtility.SetDirty(map);
        }

        // 選択タイプ
        EditorGUILayout.Space();

        selectedType = (AreaController)EditorGUILayout.ObjectField("PaintArea", selectedType, typeof(AreaController), true);

        EditorGUILayout.Space();

        // グリッド描画
        for (int y = 0; y < map.GetHeight(); y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < map.GetWidth(); x++)
            {
                // 色分け(テクスチャの方がいいかも？)
                if (map.GetArea(new Vector2Int(x, y)) != null)  
                {
                    GUI.backgroundColor = Color.red;
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                if (GUILayout.Button("", GUILayout.Width(25), GUILayout.Height(25)))
                {
                    map.SetArea(new Vector2Int(x, y), selectedType);
                    map.GetArea(new Vector2Int(x, y)).SetAreaPosition(new Vector2Int(x, y));
                    EditorUtility.SetDirty(map);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public void Resize(MapData map, int newWidth, int newHeight)
    {
        // 新しいサイズのリストを作成
        List<AreaController> newData = new List<AreaController>(new AreaController[newWidth * newHeight]);

        // 既にあるデータを新しいリストにコピー（既に作ってたものを消さないため）
        for (int y = 0; y < Mathf.Min(map.GetHeight(), newHeight); y++)
        {
            for (int x = 0; x < Mathf.Min(map.GetWidth(), newWidth); x++)
            {
                newData[y * newWidth + x] = map.GetArea(new Vector2Int(x, y)); ;
            }
        }

        // サイズ更新
        map.SetWidth(newWidth);
        map.SetHeight(newHeight);
        map.SetAreaData(newData);
    }

    public void DebugArea(MapData map, Vector2Int index)
    {
        AreaController data = map.GetArea(index);

        if (data == null)
        {
            Debug.Log("この要素には何もありません");
            return;
        }

        Debug.Log($"Area at ({index.x}, {index.y}): {data.GetAreaName()}");
    }
}
#endif
