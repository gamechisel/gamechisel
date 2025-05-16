using UnityEngine;

public class TimeScaleEditor : MonoBehaviour
{
    [Range(0.1f, 2f)] // Slider range in the Inspector (0.1x to 2x speed)
    public float timeScale = 1f; // Default time scale (1 = normal speed)

    private void OnValidate()
    {
        // Apply the time scale value whenever it's changed in the Inspector
        Time.timeScale = Mathf.Clamp(timeScale, 0.1f, 2f);
        Debug.Log($"Time scale updated: {Time.timeScale:F2}");
    }

    private void Start()
    {
        // Ensure time scale is initialized correctly
        Time.timeScale = timeScale;
    }
}
