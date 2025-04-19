using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VideoManager : MonoBehaviour
{
    // Singleton instance
    public static VideoManager Instance { get; private set; }

    [Header("Settings")]
    private bool showStats;

    [Header("Components")]
    [SerializeField] private GameObject uiHolder;
    [SerializeField] private TMP_Text infoText;
    private float fpsTimer = 0f;
    private int frameCount = 0;
    private int physicSteps = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    } 

    public void UpdateSettings(VideoSettings videoSettings)
    {
        // Fullscreen
        Screen.fullScreen = videoSettings.fullscreen;

        // V-Sync
        if (videoSettings.vSync){QualitySettings.vSyncCount = 1;}
        else{QualitySettings.vSyncCount = 0;}

        // Framerate
        if (videoSettings.limitFramerate > 240 || videoSettings.limitFramerate < 30){Application.targetFrameRate = -1;}
        else{Application.targetFrameRate = (int)videoSettings.limitFramerate;}

        // Stats
        showStats = videoSettings.showStats;
        uiHolder.SetActive(showStats);
        fpsTimer = 0f;
        frameCount = 0;
        physicSteps = 0;
    }

    // Count Physic Steps
    private void FixedUpdate()
    {
        if(showStats)
        {
            physicSteps++;
        }
    }

    // Handle Video Stats
    public void Update()
    {
        if(showStats)
        {
            frameCount++;
            fpsTimer += Time.deltaTime;
            if (fpsTimer >= 1f)
            {
                int frameRate = Mathf.RoundToInt(frameCount / fpsTimer);
                infoText.text = string.Format("{0} :FPS", frameRate) + "\n" + string.Format("{0} :UPS", physicSteps);
                fpsTimer -= 1f;
                frameCount = 0;
                physicSteps = 0;
            }
        }
    }
}
