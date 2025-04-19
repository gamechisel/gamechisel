using UnityEngine;
using System.IO;
using System;

public static class SaveOptionsSettings
{
    private const string settingsFileName = "settings.txt";

    public static void SaveSettings(SettingsData settingsData)
    {
        string dir = Application.persistentDataPath;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string json = JsonUtility.ToJson(settingsData);
        File.WriteAllText(Path.Combine(dir, settingsFileName), json);
        Debug.Log("Settings saved: " + Path.Combine(dir, settingsFileName));
    }

    public static SettingsData LoadSettings()
    {
        string fullPathSettingFile = Path.Combine(Application.persistentDataPath, settingsFileName);
        SettingsData settingsData = new SettingsData();

        if (File.Exists(fullPathSettingFile))
        {
            string json = File.ReadAllText(fullPathSettingFile);
            settingsData = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            // Initialize with default settings
            settingsData = GetDefaultSettings();
            SaveSettings(settingsData); // Save the default settings
        }

        return settingsData;
    }

    private static SettingsData GetDefaultSettings()
    {
        // Initialize default settings
        SettingsData defaultSettings = new SettingsData
        {
            slot = 1,
            audioSettings = new AudioSettings
            {
                // Set default audio settings here
            },
            inputSettings = new InputSettings
            {
                // Set default input settings here
            },
            videoSettings = new VideoSettings
            {
                // Set default video settings here
            },
            renderSettings = new RenderingSettings
            {
                // Set default rendering settings here
            }
        };

        return defaultSettings;
    }
}