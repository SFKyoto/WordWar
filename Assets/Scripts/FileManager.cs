using UnityEngine;
using System.IO;

public static class FileManager
{
    public static bool FileExists(string path)
    {
        return File.Exists(Application.persistentDataPath + "/" + path);
    }

    public static string ReadFile(string path)
    {
        StreamReader file = new StreamReader(Application.persistentDataPath + "/" + path);
        string _text = file.ReadToEnd();
        file.Close();

        return _text;
    }

    public static void WriteFile(string path, string text)
    {
        StreamWriter file = new StreamWriter(Application.persistentDataPath + "/" + path, false);
        file.Write(text);
        file.Close();
    }

    public static void CreateFile(string path)
    {
        File.Create(Application.persistentDataPath + "/" + path);
    }
}
