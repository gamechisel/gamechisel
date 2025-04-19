using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsChanger : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TMP_Text masterText;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private TMP_Text soundText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private TMP_Text uiText;
    [SerializeField] private Button applyAudioButton;

    [Header("Video")]
    [SerializeField] private Slider resolutionSlider;
    [SerializeField] private TMP_Text resolutionText;
    private Resolution[] resolutions;
    private int currentResolution;
    [SerializeField] private Slider limitFramerateSlider;
    [SerializeField] private TMP_Text limitFramerateText;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle showStatsToggle;
    [SerializeField] private Button applyVideoButton;

    private void Start()
    {
        // Load Settings
        if (OptionsManager.Instance != null)
        {
            LoadSettings();

            // Add listeners to audio sliders
            masterSlider.onValueChanged.AddListener(AdjustMasterVolume);
            musicSlider.onValueChanged.AddListener(AdjustMusicVolume);
            soundSlider.onValueChanged.AddListener(AdjustSoundVolume);
            uiSlider.onValueChanged.AddListener(AdjustUIVolume);
            applyAudioButton.onClick.AddListener(() => SaveAndApplyAudio());

            // Add listeners to video
            resolutionSlider.onValueChanged.AddListener(ChangeResolution);
            limitFramerateSlider.onValueChanged.AddListener(AdjustLimitFramerate);
            fullscreenToggle.onValueChanged.AddListener((bool value) => ToggleFullscreen());
            vSyncToggle.onValueChanged.AddListener((bool value) => ToggleVSync());
            showStatsToggle.onValueChanged.AddListener((bool value) => ToggleShowStats());
            applyVideoButton.onClick.AddListener(() => SaveAndApplyVideo());
        }
        else
        {
            Debug.LogWarning("OptionsManager not found!");
        }
    }

    public void SaveAndApplyVideo()
    {
        if(OptionsManager.Instance)
        {
            OptionsManager.Instance.ApplyVideoSettings();
            OptionsManager.Instance.SaveSettings();
        }
    }

    public void SaveAndApplyAudio()
    {
        if(OptionsManager.Instance)
        {
            OptionsManager.Instance.ApplyAudioSettings();
            OptionsManager.Instance.SaveSettings();
        }
    }

    public void LoadSettings()
    {
        // Audio settings
        float maV = Mathf.Sqrt(OptionsManager.Instance.settingsData.audioSettings.masterVolume / -0.008f);
        maV = 100f - maV;
        masterSlider.value = maV;
        masterText.text = maV.ToString() + "%";

        float muV = Mathf.Sqrt(OptionsManager.Instance.settingsData.audioSettings.musicVolume / -0.008f);
        muV = 100f - muV;
        musicSlider.value = muV;
        musicText.text = muV.ToString() + "%";

        float sV = Mathf.Sqrt(OptionsManager.Instance.settingsData.audioSettings.soundVolume / -0.008f);
        sV = 100f - sV;
        soundSlider.value = sV;
        soundText.text = sV.ToString() + "%";

        float meV = Mathf.Sqrt(OptionsManager.Instance.settingsData.audioSettings.uiVolume / -0.008f);
        meV = 100f - meV;
        uiSlider.value = meV;
        uiText.text = meV.ToString() + "%";

        // video settings
        RefreshResolutions();

        limitFramerateSlider.value = OptionsManager.Instance.settingsData.videoSettings.limitFramerate;
        if (OptionsManager.Instance.settingsData.videoSettings.limitFramerate >= 245)
        {
            limitFramerateText.text = "-";
        }
        else if (OptionsManager.Instance.settingsData.videoSettings.limitFramerate <= 244 && OptionsManager.Instance.settingsData.videoSettings.limitFramerate >= 30)
        {
            limitFramerateText.text = OptionsManager.Instance.settingsData.videoSettings.limitFramerate.ToString();
        }

        vSyncToggle.isOn = OptionsManager.Instance.settingsData.videoSettings.vSync;

        fullscreenToggle.isOn = OptionsManager.Instance.settingsData.videoSettings.fullscreen;

        showStatsToggle.isOn = OptionsManager.Instance.settingsData.videoSettings.showStats;
    }

    public void AdjustMasterVolume(float amount)
    {
        float masterV = Mathf.RoundToInt(amount - 100f);
        OptionsManager.Instance.settingsData.audioSettings.masterVolume = masterV * masterV * -0.008f;
        masterText.text = amount.ToString() + "%";
    }

    public void AdjustMusicVolume(float amount)
    {
        float musicV = Mathf.RoundToInt(amount - 100f);
        OptionsManager.Instance.settingsData.audioSettings.musicVolume = musicV * musicV * -0.008f;
        musicText.text = amount.ToString() + "%";
    }

    public void AdjustSoundVolume(float amount)
    {
        float soundV = Mathf.RoundToInt(amount - 100f);
        OptionsManager.Instance.settingsData.audioSettings.soundVolume = soundV * soundV * -0.008f;
        soundText.text = amount.ToString() + "%";
    }

    public void AdjustUIVolume(float amount)
    {
        float uiV = Mathf.RoundToInt(amount - 100f);
        OptionsManager.Instance.settingsData.audioSettings.uiVolume = uiV * uiV * -0.008f;
        uiText.text = amount.ToString() + "%";
    }

    public void RefreshResolutions()
    {
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            //string option = resolutions[i].width + "x" + resolutions[i].height;
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            { 
                currentResolutionIndex = i; 
            }
        }
        currentResolution = currentResolutionIndex;
        resolutionSlider.maxValue = resolutions.Length - 1;
        resolutionSlider.value = currentResolution;
        resolutionText.text = resolutions[currentResolution].ToString();
    }

    public void AdjustLimitFramerate(float amount)
    {
        OptionsManager.Instance.settingsData.videoSettings.limitFramerate = Mathf.RoundToInt(amount);
        if (OptionsManager.Instance.settingsData.videoSettings.limitFramerate >= 245)
        {
            Application.targetFrameRate = -1;
            limitFramerateText.text = "-";
        }
        else if (OptionsManager.Instance.settingsData.videoSettings.limitFramerate <= 244 && OptionsManager.Instance.settingsData.videoSettings.limitFramerate >= 30)
        {
            limitFramerateText.text = OptionsManager.Instance.settingsData.videoSettings.limitFramerate.ToString();
            Application.targetFrameRate = OptionsManager.Instance.settingsData.videoSettings.limitFramerate;
        }
    }

    public void ToggleFullscreen()
    {
        OptionsManager.Instance.settingsData.videoSettings.fullscreen = !OptionsManager.Instance.settingsData.videoSettings.fullscreen;
        Screen.fullScreen = OptionsManager.Instance.settingsData.videoSettings.fullscreen;
        fullscreenToggle.isOn = OptionsManager.Instance.settingsData.videoSettings.fullscreen;
    }

    public void ToggleVSync()
    {
        OptionsManager.Instance.settingsData.videoSettings.vSync = !OptionsManager.Instance.settingsData.videoSettings.vSync;
        if (OptionsManager.Instance.settingsData.videoSettings.vSync) 
        {
            QualitySettings.vSyncCount = 1;
        } 
        else 
        { 
            QualitySettings.vSyncCount = 0; 
        }
        vSyncToggle.isOn = OptionsManager.Instance.settingsData.videoSettings.vSync;
    }

    public void ToggleShowStats()
    {
        OptionsManager.Instance.settingsData.videoSettings.showStats = !OptionsManager.Instance.settingsData.videoSettings.showStats;
        showStatsToggle.isOn = OptionsManager.Instance.settingsData.videoSettings.showStats;
        // VideoManager.Instance.UpdateSettings();
    }

    public void ChangeResolution(float n)
    {
        currentResolution = Mathf.RoundToInt(n);
        Resolution resolution = resolutions[currentResolution];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionText.text = resolutions[currentResolution].ToString();
    }
}
