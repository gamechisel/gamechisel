using UnityEngine;

[System.Serializable]
public class EnvironmentSettings
{
    public float dayScale = 0f;
    public bool timeIsCycling = false;
    public WeatherStates weatherState = WeatherStates.None;
}

public enum WeatherStates
{
    None,
    Sunny,
    Foggy,
    Rainy,
    Stormy
}

[ExecuteInEditMode]
public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    [Header("Editor")]
    public bool applyCurrent;

    [Header("Time(in min)")]
    public float dayCycle;
    public string clock;

    [Header("Time")]
    [Range(0f, 1f)] public float dayScale;
    public bool timeIsCycling;
    private float currentTime;

    [Header("Weather")]
    public WeatherStates weatherState;
    [Range(0f, 1f)] public float windForce;
    [Range(0f, 1f)] public float rainForce;
    [Range(0f, 1f)] public float fogDensity;

    [Header("Rain")]
    [SerializeField] private ParticleSystem rainParticles;
    private float rainDrops = 3000f;

    [Header("Sky")]
    [SerializeField] private Transform rotSky;
    [SerializeField] private InterpolatedTransform interpolatedTransform;

    [Header("Fog")]
    [SerializeField] private Gradient fogColor;
    [SerializeField] private Color stormColor;

    [Header("Clouds")]
    [SerializeField] private Gradient cloudsColor;
    [SerializeField] private Gradient cloudsCover;
    private Transform clouds;

    [Header("Light")]
    [SerializeField] private Light moonLight;
    [SerializeField] private Light sunLight;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private Gradient moonColor;
    [SerializeField] private AnimationCurve environmentLightCurve;

    private void Awake()
    {
        Instance = this;
        applyCurrent = false;

        // Set Render Settings
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.0045f;
        RenderSettings.reflectionIntensity = 0.3f;
    }

    public void SetEnvironment(EnvironmentSettings environmentSettings)
    {
        currentTime = environmentSettings.dayScale;
        weatherState = environmentSettings.weatherState;
    }

    private void Update()
    {
        if (applyCurrent)
        {
            FixedUpdate();
        }
    }

    private void FixedUpdate()
    {
        HandleTime();
        HandleSkybox();
        HandleLights();
        HandleWeather();
        HandleFog();
        HandleEnvironmentLight();
        if (!applyCurrent)
        {
            Interpolate();
        }
    }

    public EnvironmentSettings GetEnvironmentSettings()
    {
        EnvironmentSettings environmentSettings = new EnvironmentSettings();
        environmentSettings.dayScale = dayScale;
        environmentSettings.timeIsCycling = timeIsCycling;
        environmentSettings.weatherState = weatherState;
        return environmentSettings;
    }

    public void SetTime(float time)
    {
        if (time <= 1f && time >= 0f)
        {
            dayScale = time;
            currentTime = dayCycle * (dayScale * 60f);
        }
    }

    private void HandleTime()
    {
        if (timeIsCycling)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= (dayCycle * 60f))
            {
                currentTime -= (dayCycle * 60f);
            }
            dayScale = currentTime / (dayCycle * 60f);
        }

        float timerC = dayScale * 24f;
        if (timerC >= 24f)
        {
            timerC -= 24f;
        }

        clock = "Time: " + timerC.ToString();
    }

    private void HandleSkybox()
    {
        rotSky.eulerAngles = new Vector3(dayScale * 360f, 0f, 0f);
    }

    private void HandleLights()
    {
        if (dayScale <= 0.75f && dayScale >= 0.25f)
        {
            float sunColorTime = (dayScale - 0.25f) * 2f;
            if (sunColorTime > 1) { sunColorTime -= 1f; }
            sunLight.color = sunColor.Evaluate(sunColorTime);
        }
        else
        {
            float moonColorTime = dayScale + 0.75f;
            if (moonColorTime > 1) { moonColorTime -= 1f; }
            moonLight.color = moonColor.Evaluate(dayScale);
        }
    }

    private void HandleWeather()
    {
        if (Random.Range(0f, 1f) < 0.005f)
        {
            weatherState = (WeatherStates)Random.Range(1, (int)WeatherStates.Stormy + 1);
        }
        if (RenderingManager.Instance != null)
        {
            RenderingManager.Instance.windForce = windForce;
            RenderingManager.Instance.UpdateShaderSettings();
        }
        var rain_emission = rainParticles.emission;
        rain_emission.rateOverTime = rainDrops * (rainForce * rainForce);
    }

    private void HandleFog()
    {
        Color horizonColor = Color.Lerp(fogColor.Evaluate(dayScale), stormColor, rainForce / 1.5f);
        RenderSettings.fogColor = horizonColor;
        RenderSettings.fogDensity = fogDensity * fogDensity * 0.05f + 0.005f;
    }

    private void HandleEnvironmentLight()
    {
        RenderSettings.ambientIntensity = environmentLightCurve.Evaluate(dayScale);
    }

    private void Interpolate()
    {
        interpolatedTransform.LateFixedUpdate();
    }
}
