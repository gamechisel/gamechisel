using UnityEngine;
using System.IO;
using System;

public static class SaveGameSystem
{
    private const string saveFileName = "save.txt";

    public static void SaveGame(SaveData saveData, int slot)
    {
        string currentDate = DateTime.Now.ToString();
        saveData.date = currentDate;

        string dir = Application.persistentDataPath;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Path.Combine(dir, saveFileName + slot.ToString()), json);
        Debug.Log("Game Saved: " + Path.Combine(dir, saveFileName));
    }

    public static SaveData LoadGame(int slot)
    {
        string fullPathSaveFile = Path.Combine(Application.persistentDataPath, saveFileName + slot.ToString());
        SaveData saveData = new SaveData();
        if (File.Exists(fullPathSaveFile))
        {
            string json = File.ReadAllText(fullPathSaveFile);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        return saveData;
    }

    public static void ResetGame(int slot)
    {
        File.Delete(Path.Combine(Application.persistentDataPath, saveFileName + slot.ToString()));
    }
}
