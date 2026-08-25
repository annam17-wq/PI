using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true = formatado, fácil de debugar
        File.WriteAllText(SavePath, json);
        Debug.Log($"Jogo salvo em: {SavePath}");
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("Nenhum save encontrado, criando novo.");
            return new SaveData();
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave() => File.Exists(SavePath);

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}