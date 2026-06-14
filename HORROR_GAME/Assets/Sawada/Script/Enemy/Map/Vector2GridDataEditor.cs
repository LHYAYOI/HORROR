using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Vector2GridData))]
public class Vector2GridDataEditor : Editor
{
    private const float CellWidth = 90f;

    public override void OnInspectorGUI()
    {
        Vector2GridData data = (Vector2GridData)target;

        Undo.RecordObject(data, "Edit Vector2 Grid");

        data.width = Mathf.Max(1, EditorGUILayout.IntField("Width", data.width));
        data.height = Mathf.Max(1, EditorGUILayout.IntField("Height", data.height));

        ResizeIfNeeded(data);

        GUILayout.Space(8);

        for (int y = 0; y < data.height; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < data.width; x++)
            {
                int index = y * data.width + x;
                Vector2 value = data.cells[index];

                string text = $"({value.x}, {value.y})";

                string newText = EditorGUILayout.TextField(
                    text,
                    GUILayout.Width(CellWidth)
                );

                if (newText != text && TryParseVector2(newText, out Vector2 parsed))
                {
                    data.cells[index] = parsed;
                    EditorUtility.SetDirty(data);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void ResizeIfNeeded(Vector2GridData data)
    {
        int size = data.width * data.height;

        if (data.cells != null && data.cells.Length == size)
            return;

        Vector2[] newCells = new Vector2[size];

        if (data.cells != null)
        {
            int copyCount = Mathf.Min(data.cells.Length, newCells.Length);
            for (int i = 0; i < copyCount; i++)
                newCells[i] = data.cells[i];
        }

        data.cells = newCells;
        EditorUtility.SetDirty(data);
    }

    private bool TryParseVector2(string text, out Vector2 value)
    {
        value = Vector2.zero;

        text = text.Replace("(", "")
                   .Replace(")", "")
                   .Replace(" ", "");

        string[] split = text.Split(',');

        if (split.Length != 2)
            return false;

        if (!float.TryParse(split[0], out float x))
            return false;

        if (!float.TryParse(split[1], out float y))
            return false;

        value = new Vector2(x, y);
        return true;
    }
}