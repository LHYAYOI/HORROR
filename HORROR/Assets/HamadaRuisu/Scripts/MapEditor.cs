#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{
    AreaData selectedType;

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
            map.Resize(newWidth, newHeight);
            EditorUtility.SetDirty(map);
        }

        // 選択タイプ
        EditorGUILayout.Space();

        selectedType = (AreaData)EditorGUILayout.ObjectField("PaintArea", selectedType, typeof(AreaData), false);

        EditorGUILayout.Space();

        // グリッド描画
        for (int y = 0; y < map.GetHeight(); y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < map.GetWidth(); x++)
            {
                //AreaData area = map.GetArea(x, y);

                // 色分け(テクスチャの方がいいかも？)
                //GUI.backgroundColor = GetColor(value);

                if (GUILayout.Button("", GUILayout.Width(25), GUILayout.Height(25)))
                {
                    map.SetArea(x, y, selectedType);
                    EditorUtility.SetDirty(map);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
