using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject defaultCamera;  // The camera for regular gameplay
    public GameObject aimingCamera;   // The camera for when the player is aiming
    public string aimingInput = "Fire2"; // Default aiming input (e.g., right mouse button)

    private bool isAiming = false;

    void Update()
    {
        // Check if the player is aiming (for example, by pressing the right mouse button or another input)
        isAiming = Input.GetButton(aimingInput);

        // Switch between the cameras based on whether the player is aiming or not
        if (isAiming)
        {
            SwitchToAimingCamera();
        }
        else
        {
            SwitchToDefaultCamera();
        }
    }

    private void SwitchToAimingCamera()
    {
        // Set the aiming camera active
        if (aimingCamera != null)
        {
            aimingCamera.SetActive(true);
        }

        // Deactivate the default camera
        if (defaultCamera != null)
        {
            defaultCamera.SetActive(false);
        }
    }

    private void SwitchToDefaultCamera()
    {
        // Set the default camera active
        if (defaultCamera != null)
        {
            defaultCamera.SetActive(true);
        }

        // Deactivate the aiming camera
        if (aimingCamera != null)
        {
            aimingCamera.SetActive(false);
        }
    }
}
