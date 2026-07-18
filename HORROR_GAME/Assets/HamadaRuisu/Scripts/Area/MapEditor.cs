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
            // サイズ変更後はすぐにインスペクターを再描画させてバグを防ぐ
            AssetDatabase.SaveAssets();
            return;
        }

        // 選択タイプ
        EditorGUILayout.Space();
        selectedType = (AreaController)EditorGUILayout.ObjectField("PaintArea", selectedType, typeof(AreaController), true);
        EditorGUILayout.Space();

        // ★デフォルトの色を保存しておく（これがないと他のUIまで全部変な色になる）
        Color defaultColor = GUI.backgroundColor;

        // グリッド描画
        // 現在のマップの最新のサイズを変数に入れておく
        int currentHeight = map.GetHeight();
        int currentWidth = map.GetWidth();

        for (int y = 0; y < currentHeight; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < currentWidth; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                AreaController area = map.GetArea(pos);

                // もしデータが空（null）なら白にする
                if (area == null)
                {
                    GUI.backgroundColor = Color.white;
                }
                else
                {
                    // タイプによって色分け
                    switch (area.GetAreaType())
                    {
                        case AREA_TYPE.PASSABLE: GUI.backgroundColor = Color.red; break;
                        case AREA_TYPE.SKIPPABLE: GUI.backgroundColor = Color.blue; break;
                        case AREA_TYPE.WALL: GUI.backgroundColor = Color.purple; break;
                        default: GUI.backgroundColor = Color.white; break;
                    }
                }

                // ボタンのテキストに「空なら●、あれば頭文字」などを出すと分かりやすいよ
                string btnText = area != null ? area.GetAreaName() : "?";

                if (GUILayout.Button(btnText, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    if (selectedType != null)
                    {
                        map.SetArea(pos, selectedType);
                        // インスタンス化されたコンポーネントがある場合のみ位置をセット
                        if (map.GetArea(pos) != null)
                        {
                            map.GetArea(pos).SetAreaPosition(pos);
                        }
                        EditorUtility.SetDirty(map);
                    }
                    else
                    {
                        Debug.LogWarning("PaintArea に AreaController がセットされていません！");
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // ★変更したボタンの色を元に戻す
        GUI.backgroundColor = defaultColor;
    }

    public void Resize(MapData map, int newWidth, int newHeight)
    {
        if (map == null) return;
        if (newHeight < 0 || newWidth < 0) return;

        // 新しいサイズのリストを作成
        List<AreaController> newData = new List<AreaController>(newWidth * newHeight);

        // 1次元配列（リスト）を正しく初期化（nullで埋める）
        for (int i = 0; i < newWidth * newHeight; i++)
        {
            newData.Add(null);
        }

        // 既にあるデータを新しいリストにコピー（インデックス計算を厳密に！）
        int oldWidth = map.GetWidth();
        int oldHeight = map.GetHeight();

        for (int y = 0; y < Mathf.Min(oldHeight, newHeight); y++)
        {
            for (int x = 0; x < Mathf.Min(oldWidth, newWidth); x++)
            {
                newData[y * newWidth + x] = map.GetArea(new Vector2Int(x, y));
            }
        }

        // サイズ更新
        map.SetWidth(newWidth);
        map.SetHeight(newHeight);
        map.SetAreaData(newData);
    }
}
#endif