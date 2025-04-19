using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// -----------------------------------------------------------------------------
// Class to Handle Camera
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Components")]
    private Camera mainCamera; // Reference to the main camera component
    public Transform cameraTransform;

    // -----------------------------------------------------------------------------
    // Handle Camera
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

    // -----------------------------------------------------------------------------
    // Camera Settings
    public void UpdateSettings()
    {
        // Optional Camera Settings
    }

    public void SetCameraState(bool state)
    {
        mainCamera.enabled = state;
    }

    // -----------------------------------------------------------------------------
    // Get Camera Position
    public Vector3 GetCameraPosition()
    {
        Vector3 cameraPosition = cameraTransform.position;
        return cameraPosition;
    }

    public Vector3 GetCameraRotation()
    {
        Vector3 cameraRotation = new Vector3(cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        return cameraRotation;
    }
}
