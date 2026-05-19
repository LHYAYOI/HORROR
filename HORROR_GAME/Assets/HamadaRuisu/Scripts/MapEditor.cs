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

        selectedType = (AreaController)EditorGUILayout.ObjectField("PaintArea", selectedType, typeof(AreaController), false);

        EditorGUILayout.Space();

        // グリッド描画
        for (int y = 0; y < map.GetHeight(); y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < map.GetWidth(); x++)
            {
                //AreaData area = map.GetArea(x, y);

                // 色分け(テクスチャの方がいいかも？)
                if (map.GetArea(new Vector2Int(x, y)) != null)  
                {
                    GUI.backgroundColor = Color.red;
                }

                if (GUILayout.Button("", GUILayout.Width(25), GUILayout.Height(25)))
                {
                    map.SetArea(new Vector2Int(x, y), selectedType);
                    EditorUtility.SetDirty(map);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        map.DebugArea(new Vector2Int(0, 0));
        map.DebugArea(new Vector2Int(1, 0));
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
}
#endif
