using UnityEngine;

[System.Serializable]
public class RenderingSettings
{
    [Header("Shader")]
    public float lowVegetationDistance = 35f;
    public bool windEffects = true;
    public float windForce = 0.1f;

    [Header("Render Pipeline")]
    public bool postProcessing = true;
}