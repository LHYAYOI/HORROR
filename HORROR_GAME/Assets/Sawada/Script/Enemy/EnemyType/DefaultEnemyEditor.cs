using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultEnemy))]
public class DefaultEnemyEditor : Editor
{
    // 各列の固定幅
    private const float DistanceWidth = 55f;
    private const float SizeWidth = 120f;
    private const float AdjustmentWidth = 120f;
    private const float DeleteButtonWidth = 28f;

    // 列同士の余白
    private const float ColumnSpace = 5f;

    // 縦線の太さ
    private const float SeparatorWidth = 1f;

    // SerializedProperty
    private SerializedProperty m_imagesProperty;

    private SerializedProperty m_speedProperty;
    private SerializedProperty m_sizeProperty;
    private SerializedProperty m_spawnPositionProperty;

    private SerializedProperty m_searchRangeProperty;
    private SerializedProperty m_searchMethodProperty;

    private SerializedProperty m_stateListProperty;
    private SerializedProperty m_noAreaDataProperty;

    private void OnEnable()
    {
        m_imagesProperty =
            serializedObject.FindProperty("m_images");

        m_speedProperty =
            serializedObject.FindProperty("m_speed");

        m_spawnPositionProperty =
            serializedObject.FindProperty("m_spawnPosition");

        m_searchRangeProperty =
            serializedObject.FindProperty("m_searchRange");

        m_searchMethodProperty =
            serializedObject.FindProperty("m_searchMethod");

        m_stateListProperty =
            serializedObject.FindProperty("m_stateList");

        m_noAreaDataProperty =
            serializedObject.FindProperty("m_noAreaData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDistanceImages();

        EditorGUILayout.Space(10);

        DrawNumberSettings();

        EditorGUILayout.Space(10);

        DrawSearchSettings();

        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(
            m_stateListProperty,
            new GUIContent("ステート一覧"),
            true);

        EditorGUILayout.PropertyField(
            m_noAreaDataProperty,
            new GUIContent("侵入不可エリア"),
            true);

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 距離別立ち絵
    /// </summary>
    private void DrawDistanceImages()
    {
        EditorGUILayout.LabelField(
            "距離別立ち絵",
            EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            DrawImageHeader();

            for (int i = 0; i < m_imagesProperty.arraySize; i++)
            {
                bool deleted = DrawImageElement(i);

                if (deleted)
                {
                    break;
                }
            }

            if (m_imagesProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox(
                    "立ち絵が登録されていません。",
                    MessageType.Info);
            }

            EditorGUILayout.Space(2);

            DrawAddButton();
        }
    }

    /// <summary>
    /// ヘッダー
    /// </summary>
    private void DrawImageHeader()
    {
        Rect rowRect = EditorGUILayout.GetControlRect(
            false,
            EditorGUIUtility.singleLineHeight);

        GetColumnRects(
            rowRect,
            out Rect spriteRect,
            out Rect distanceRect,
            out Rect sizeRect,
            out Rect adjustmentRect,
            out Rect deleteRect,
            out Rect separator1,
            out Rect separator2,
            out Rect separator3,
            out Rect separator4);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(0, 0, 0, 0)
        };

        EditorGUI.LabelField(
            spriteRect,
            "立ち絵",
            headerStyle);

        EditorGUI.LabelField(
            distanceRect,
            "距離",
            headerStyle);

        EditorGUI.LabelField(
            sizeRect,
            "大きさ",
            headerStyle);

        EditorGUI.LabelField(
            adjustmentRect,
            "位置微調整",
            headerStyle);

        DrawSeparator(separator1);
        DrawSeparator(separator2);
        DrawSeparator(separator3);
        DrawSeparator(separator4);
    }

    /// <summary>
    /// 各データ行
    /// </summary>
    private bool DrawImageElement(int index)
    {
        SerializedProperty element =
            m_imagesProperty.GetArrayElementAtIndex(index);

        SerializedProperty spriteProperty =
            element.FindPropertyRelative("m_sprite");

        SerializedProperty distanceProperty =
            element.FindPropertyRelative("m_distance");

        SerializedProperty sizeProperty =
            element.FindPropertyRelative("m_size");

        SerializedProperty adjustmentProperty =
            element.FindPropertyRelative("m_adjustment");

        Rect rowRect = EditorGUILayout.GetControlRect(
            false,
            EditorGUIUtility.singleLineHeight);

        GetColumnRects(
            rowRect,
            out Rect spriteRect,
            out Rect distanceRect,
            out Rect sizeRect,
            out Rect adjustmentRect,
            out Rect deleteRect,
            out Rect separator1,
            out Rect separator2,
            out Rect separator3,
            out Rect separator4);

        EditorGUI.PropertyField(
            spriteRect,
            spriteProperty,
            GUIContent.none);

        EditorGUI.PropertyField(
            distanceRect,
            distanceProperty,
            GUIContent.none);

        EditorGUI.PropertyField(
            sizeRect,
            sizeProperty,
            GUIContent.none);

        EditorGUI.PropertyField(
            adjustmentRect,
            adjustmentProperty,
            GUIContent.none);

        DrawSeparator(separator1);
        DrawSeparator(separator2);
        DrawSeparator(separator3);
        DrawSeparator(separator4);

        if (GUI.Button(deleteRect, "－"))
        {
            m_imagesProperty.DeleteArrayElementAtIndex(index);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 全行で共通の列位置を計算する
    /// </summary>
    private void GetColumnRects(
        Rect rowRect,
        out Rect spriteRect,
        out Rect distanceRect,
        out Rect sizeRect,
        out Rect adjustmentRect,
        out Rect deleteRect,
        out Rect separator1,
        out Rect separator2,
        out Rect separator3,
        out Rect separator4)
    {
        // 縦線の左右に余白があるため、1区切りにつき余白2個
        float totalSpacing =
            ColumnSpace * 8f;

        float totalSeparatorWidth =
            SeparatorWidth * 4f;

        float fixedWidth =
            DistanceWidth +
            SizeWidth +
            AdjustmentWidth +
            DeleteButtonWidth +
            totalSpacing +
            totalSeparatorWidth;

        float spriteWidth =
            Mathf.Max(80f, rowRect.width - fixedWidth);

        float x = rowRect.x;

        // Sprite
        spriteRect = new Rect(
            x,
            rowRect.y,
            spriteWidth,
            rowRect.height);

        x += spriteWidth;
        x += ColumnSpace;

        // Spriteと距離の間
        separator1 = new Rect(
            x,
            rowRect.y,
            SeparatorWidth,
            rowRect.height);

        x += SeparatorWidth;
        x += ColumnSpace;

        // 距離
        distanceRect = new Rect(
            x,
            rowRect.y,
            DistanceWidth,
            rowRect.height);

        x += DistanceWidth;
        x += ColumnSpace;

        // 距離とSizeの間
        separator2 = new Rect(
            x,
            rowRect.y,
            SeparatorWidth,
            rowRect.height);

        x += SeparatorWidth;
        x += ColumnSpace;

        // Size
        sizeRect = new Rect(
            x,
            rowRect.y,
            SizeWidth,
            rowRect.height);

        x += SizeWidth;
        x += ColumnSpace;

        // SizeとAdjustmentの間
        separator3 = new Rect(
            x,
            rowRect.y,
            SeparatorWidth,
            rowRect.height);

        x += SeparatorWidth;
        x += ColumnSpace;

        // Adjustment
        adjustmentRect = new Rect(
            x,
            rowRect.y,
            AdjustmentWidth,
            rowRect.height);

        x += AdjustmentWidth;
        x += ColumnSpace;

        // Adjustmentと削除ボタンの間
        separator4 = new Rect(
            x,
            rowRect.y,
            SeparatorWidth,
            rowRect.height);

        x += SeparatorWidth;
        x += ColumnSpace;

        // 削除ボタン
        deleteRect = new Rect(
            x,
            rowRect.y,
            DeleteButtonWidth,
            rowRect.height);
    }

    /// <summary>
    /// 縦線を描画
    /// </summary>
    private void DrawSeparator(Rect rect)
    {
        Color separatorColor;

        if (EditorGUIUtility.isProSkin)
        {
            separatorColor =
                new Color(0.38f, 0.38f, 0.38f, 1f);
        }
        else
        {
            separatorColor =
                new Color(0.55f, 0.55f, 0.55f, 1f);
        }

        EditorGUI.DrawRect(
            rect,
            separatorColor);
    }

    /// <summary>
    /// 追加ボタン
    /// </summary>
    private void DrawAddButton()
    {
        Rect rowRect = EditorGUILayout.GetControlRect(
            false,
            EditorGUIUtility.singleLineHeight);

        GetColumnRects(
            rowRect,
            out Rect spriteRect,
            out Rect distanceRect,
            out Rect sizeRect,
            out Rect adjustmentRect,
            out Rect deleteRect,
            out Rect separator1,
            out Rect separator2,
            out Rect separator3,
            out Rect separator4);

        // ＋ボタンを距離列の位置に表示
        Rect addButtonRect = new Rect(
            distanceRect.x,
            rowRect.y,
            DistanceWidth,
            rowRect.height);

        if (GUI.Button(addButtonRect, "＋"))
        {
            AddImageElement();
        }
    }

    /// <summary>
    /// 新しい距離別立ち絵を追加
    /// </summary>
    private void AddImageElement()
    {
        int newIndex =
            m_imagesProperty.arraySize;

        m_imagesProperty.InsertArrayElementAtIndex(newIndex);

        SerializedProperty newElement =
            m_imagesProperty.GetArrayElementAtIndex(newIndex);

        SerializedProperty spriteProperty =
            newElement.FindPropertyRelative("m_sprite");

        SerializedProperty distanceProperty =
            newElement.FindPropertyRelative("m_distance");

        SerializedProperty sizeProperty =
            newElement.FindPropertyRelative("m_size");

        SerializedProperty adjustmentProperty =
            newElement.FindPropertyRelative("m_adjustment");

        spriteProperty.objectReferenceValue = null;
        distanceProperty.intValue = 0;
        sizeProperty.vector2IntValue = Vector2Int.zero;
        adjustmentProperty.vector2IntValue = Vector2Int.zero;
    }

    /// <summary>
    /// 数値設定
    /// </summary>
    private void DrawNumberSettings()
    {
        EditorGUILayout.LabelField(
            "数値",
            EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.PropertyField(
                m_speedProperty,
                new GUIContent("速度"));

            EditorGUILayout.PropertyField(
                m_spawnPositionProperty,
                new GUIContent("出現位置"));
        }
    }

    /// <summary>
    /// 索敵設定
    /// </summary>
    private void DrawSearchSettings()
    {
        EditorGUILayout.LabelField(
            "索敵範囲",
            EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.PropertyField(
                m_searchRangeProperty,
                new GUIContent("索敵距離"));

            EditorGUILayout.PropertyField(
                m_searchMethodProperty,
                new GUIContent("索敵方法"));
        }
    }
}