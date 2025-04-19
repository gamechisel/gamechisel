using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }

    [Header("Data")]
    public SettingsData settingsData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadSettings();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if(settingsData == null)
        {
            LoadSettings();
        }
        // Set all Settings
        ApplyAllSettings();
    }

    public void OpenOptionsMenu()
    {
        UIManager.Instance.LoadMenu("options_menu");
    }

    public void ApplyAllSettings()
    {
        ApplyAudioSettings();
        ApplyVideoSettings();
        ApplyInputSettings();
        ApplyCameraSettings();
    }

    public void LoadSettings()
    {
        settingsData = SaveOptionsSettings.LoadSettings();
    }

    // Save Settings
    public void SaveSettings()
    {
        SaveOptionsSettings.SaveSettings(settingsData);
    }

    public void ApplyAudioSettings()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            AudioSettings sendAudioSettings = new AudioSettings
            {
                masterVolume = settingsData.audioSettings.masterVolume,
                soundVolume = settingsData.audioSettings.soundVolume,
                musicVolume = settingsData.audioSettings.musicVolume,
                uiVolume = settingsData.audioSettings.uiVolume
            };
            audioManager.UpdateSettings(sendAudioSettings);
        }
    }

    // Apply VideoManager Settings
    public void ApplyVideoSettings()
    {
        VideoManager videoManager = VideoManager.Instance;
        if (videoManager != null)
        {
            VideoSettings sendVideoSettings = new VideoSettings
            {
                limitFramerate = settingsData.videoSettings.limitFramerate,
                fullscreen = settingsData.videoSettings.fullscreen,
                vSync = settingsData.videoSettings.vSync,
                showStats = settingsData.videoSettings.showStats
            };
            videoManager.UpdateSettings(sendVideoSettings);
        }
    }

    // Apply InputManager Settings
    public void ApplyInputSettings()
    {
        InputManager inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            InputSettings sendInputSettings = new InputSettings
            {
                invertVertical = settingsData.inputSettings.invertVertical,
                invertHorizontal = settingsData.inputSettings.invertHorizontal,
                sensitivity = settingsData.inputSettings.sensitivity
            };
            inputManager.UpdateSettings(sendInputSettings);
        }
    }

    // Apply CameraManager Settings
    public void ApplyCameraSettings()
    {
        CameraManager cameraManager = CameraManager.Instance;
        if (cameraManager != null)
        {
            // Set Settings
            CameraManager.Instance.UpdateSettings();
        }
    }
}