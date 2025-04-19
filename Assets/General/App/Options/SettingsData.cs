using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public int slot = 1;
    public AudioSettings audioSettings;
    public InputSettings inputSettings;
    public VideoSettings videoSettings;
    public RenderingSettings renderSettings;
}