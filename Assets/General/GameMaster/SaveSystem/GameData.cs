using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Data")]
    public SaveData data;

    // Start
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Functions
    public SaveData GetGameData()
    {
        return data;
    }

    public void SetSlot(int s)
    {
        OptionsManager.Instance.settingsData.slot = s;
        OptionsManager.Instance.SaveSettings();
        Load();
    }

    public void Load()
    {
        data = SaveGameSystem.LoadGame(OptionsManager.Instance.settingsData.slot);
    }

    public void Save()
    {
        SaveGameSystem.SaveGame(data, OptionsManager.Instance.settingsData.slot);
        // if (OptionsManager.Instance.settingsData.slot != 0)
        // {
        // }
    }

    public void Reset()
    {
        SaveGameSystem.ResetGame(OptionsManager.Instance.settingsData.slot);
    }
}
