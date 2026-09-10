using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MenuFlag : MonoBehaviour
{
    private static string FilePath =>
        Path.Combine(Application.dataPath, "BoolSaveData.txt");

    // 保存
    public static void SetBool(string key, bool value)
    {
        Dictionary<string, bool> data = LoadAll();

        data[key] = value;

        SaveAll(data);
    }

    // 読み込み
    public static bool GetBool(string key, bool defaultValue = false)
    {
        Dictionary<string, bool> data = LoadAll();

        if (data.TryGetValue(key, out bool value))
        {
            return value;
        }

        return defaultValue;
    }

    // 全データ読み込み
    public static Dictionary<string, bool> LoadAll()
    {
        Dictionary<string, bool> data = new Dictionary<string, bool>();

        if (!File.Exists(FilePath))
            return data;

        string[] lines = File.ReadAllLines(FilePath);

        foreach (string line in lines)
        {
            string[] split = line.Split('=');

            if (split.Length != 2)
                continue;

            string key = split[0];

            if (bool.TryParse(split[1], out bool value))
            {
                data[key] = value;
            }
        }

        return data;
    }

    // 全データ保存
    private static void SaveAll(Dictionary<string, bool> data)
    {
        using StreamWriter writer = new StreamWriter(FilePath);

        foreach (var pair in data)
        {
            writer.WriteLine($"{pair.Key}={pair.Value}");
        }
    }

    // 保存場所確認用
    public static string GetFilePath()
    {
        return FilePath;
    }
}
