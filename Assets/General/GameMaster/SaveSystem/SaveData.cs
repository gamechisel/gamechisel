using UnityEngine;

[System.Serializable]
public class SaveData
{
    [Header("General")]
    public string date = "";

    [Header("World")]
    public EnvironmentSettings environmentSettings;

    [Header("SavePoint")]
    public SavePoint savePoint;

    [Header("Progress")]
    public string activeQuest = "";

    // [Header("PlayerData")]
    // public string playerData = "";


}