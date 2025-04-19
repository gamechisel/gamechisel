using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class RenderingManager : MonoBehaviour
{
    public static RenderingManager Instance { get; private set; }

    [Header("Editor")]
    public bool applyCurrent = false;

    [Header("RenderPipeline")]
    public UniversalRenderPipelineAsset asset;

    [Header("Shader")]
    public float lowVegetationDistance;
    public bool windEffects;
    // public float windDistance;
    public float windForce;

    [Header("Post Processing")]
    public GameObject ppVolume;

    private void Awake()
    {
        applyCurrent = false;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (applyCurrent)
        {
            applyCurrent = false;
            UpdateShaderSettings();
        }
    }

    // Apply Render Settings
    public void UpdateSettings(RenderingSettings _settings)
    {
        lowVegetationDistance = _settings.lowVegetationDistance;
        // windDistance = 50f;
        windEffects = _settings.windEffects;
        windForce = _settings.windForce;
        SwitchPP(_settings.postProcessing);
        UpdateShaderSettings();
    }

    // Update Shader Settings
    public void UpdateShaderSettings()
    {
        Shader.SetGlobalFloat("lowVegetationDistance", lowVegetationDistance);
        // Shader.SetGlobalFloat("windDistance", windDistance);
        Shader.SetGlobalFloat("windStrength", windForce);
        if (windEffects)
        {
            Shader.SetGlobalFloat("windEffects", 1f);
        }
        else
        {
            Shader.SetGlobalFloat("windEffects", 0f);
        }
    }

    public void SwitchPP(bool _state)
    {
        ppVolume.SetActive(_state);
    }

    // The rest of the commented-out parts are kept here for future reference
    // ...

    // private void GetVolume()
    // {
    //     MotionBlur mB;
    //     if (volume.profile.TryGet<MotionBlur>(out mB))
    //     {
    //         motionBlur = mB;
    //     }
    //     Bloom b;
    //     if (volume.profile.TryGet<Bloom>(out b))
    //     {
    //         bloom = b;
    //     }
    //     Tonemapping tM;
    //     if (volume.profile.TryGet<Tonemapping>(out tM))
    //     {
    //         tonemapping = tM;
    //     }
    //     ColorAdjustments cA;
    //     if (volume.profile.TryGet<ColorAdjustments>(out cA))
    //     {
    //         colorAdjustments = cA;
    //     }
    //     DepthOfField dOF;
    //     if (volume.profile.TryGet<DepthOfField>(out dOF))
    //     {
    //         depthOfField = dOF;
    //     }
    //     WhiteBalance wB;
    //     if (volume.profile.TryGet<WhiteBalance>(out wB))
    //     {
    //         whiteBalance = wB;
    //     }
    // }

    // public void UpdateSettingsPP()
    // {
    //     volume.enabled = Options.Instance.settingsData.postProcessing;
    //     motionBlur.active = Options.Instance.settingsData.motionBlur;
    //     motionBlur.intensity.value = Options.Instance.settingsData.motionBlurIntensity;
    //     motionBlur.clamp.value = Options.Instance.settingsData.motionBlurClamp;
    //     depthOfField.active = Options.Instance.settingsData.depthOfField;
    //     depthOfField.highQualitySampling.value = Options.Instance.settingsData.highQualityDOF;
    //     bloom.active = Options.Instance.settingsData.bloom;
    //     bloom.threshold.value = Options.Instance.settingsData.bloomThreshold;
    //     bloom.intensity.value = Options.Instance.settingsData.bloomIntensity;
    //     tonemapping.active = Options.Instance.settingsData.tonemapping;
    //     colorAdjustments.active = Options.Instance.settingsData.colorAdjustments;
    //     colorAdjustments.postExposure.value = Options.Instance.settingsData.postExposure;
    //     colorAdjustments.contrast.value = Options.Instance.settingsData.contrast;
    //     colorAdjustments.saturation.value = Options.Instance.settingsData.saturation;
    //     whiteBalance.active = Options.Instance.settingsData.whiteBalance;
    //     whiteBalance.temperature.value = Options.Instance.settingsData.temperature;
    //     whiteBalance.tint.value = Options.Instance.settingsData.tint;
    // }

    // [Header("RenderPipeline")]
    // shadowDistance
    // supportsHDR
    //asset.msaaSampleCount = 1;
    // Disabled = 1,
    // _2x = 2,
    // _4x = 4,
    // _8x = 8
    // asset.renderScale = 1f;
    // 0.1 - 2
    // asset.upscalingFilter = UpscalingFilterSelection.Auto;
    //[InspectorName("Bilinear")] Linear
    //[InspectorName("Nearest-Neighbor")] Point
    //[InspectorName("FidelityFX Super Resolution 1.0")] FSR
    // asset.shadowCascadeCount = 2;
    // 1 - 4
    // asset.colorGradingMode = ColorGradingMode.LowDynamicRange;
    // ColorGradingMode.HighDynamicRange;
}
