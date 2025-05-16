using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionThrowData
{
    [Header("Throwing Data")]
    public GameObject throwablePrefab = null; // The object to throw
    public Transform throwPoint = null; // Point where the throwable is instantiated
    public float throwForce = 10f; // Force applied to the throwable object
    public float throwCooldown = 1.0f; // Cooldown time between throws

    [Header("Aiming Data")]
    public float aimZoomFactor = 0.6f; // Camera zoom factor while aiming
    public float aimDuration = 0.3f; // Time to transition into aiming

    [Header("Audio")]
    public AudioClip throwSound = null; // Sound played when throwing
    public AudioClip aimSound = null; // Sound played when aiming
}

[System.Serializable]
public class ActionThrow
{
    public ActionThrowData actionThrowData;

    private Essentials essentials;
    private ActionSound actionSound;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;

    private bool isAiming = false;
    private float lastThrowTime = 0f;

    public void Init(Essentials _essentials, ActionSound _actionSound, ActionAnimation _actionAnimation, PlayerActionState _state)
    {
        essentials = _essentials;
        actionSound = _actionSound;
        actionAnimation = _actionAnimation;
        state = _state;
    }

    // Start Aiming
    public void StartAim()
    {
        if (isAiming) return; // Prevent multiple aim starts

        isAiming = true;

        // Play aim animation and sound
        actionAnimation.PlayAnimation("aim_throw");
        if (actionThrowData.aimSound != null)
        {
            actionSound.PlaySound(new SoundData(actionThrowData.aimSound, 1f));
        }

        // Adjust camera zoom (if applicable)
        // essentials.cameraController.SetZoom(actionThrowData.aimZoomFactor, actionThrowData.aimDuration);

        state.SetActionState("aim_throw", actionThrowData.aimDuration);
    }

    // Stop Aiming
    public void StopAim()
    {
        if (!isAiming) return; // Prevent stopping aim if not active

        isAiming = false;

        // Reset animation and zoom
        actionAnimation.PlayAnimation("idle");
        // essentials.cameraController.ResetZoom(actionThrowData.aimDuration);

        // state.ClearActionState("aim_throw");
    }

    // Throw
    public void Throw()
    {
        if (Time.time - lastThrowTime < actionThrowData.throwCooldown) return; // Check cooldown
        if (!isAiming) return; // Throwing is allowed only while aiming

        lastThrowTime = Time.time;

        // Play throw animation and sound
        actionAnimation.PlayAnimation("throw");
        if (actionThrowData.throwSound != null)
        {
            actionSound.PlaySound(new SoundData(actionThrowData.throwSound, 1f));
        }

        // Instantiate and throw the object
        if (actionThrowData.throwablePrefab != null && actionThrowData.throwPoint != null)
        {
            GameObject throwable = GameObject.Instantiate(
                actionThrowData.throwablePrefab,
                actionThrowData.throwPoint.position,
                actionThrowData.throwPoint.rotation
            );

            Rigidbody throwableRigidbody = throwable.GetComponent<Rigidbody>();
            if (throwableRigidbody != null)
            {
                throwableRigidbody.AddForce(actionThrowData.throwPoint.forward * actionThrowData.throwForce, ForceMode.Impulse);
            }
        }

        state.SetActionState("throw", 0.5f); // Set state for the duration of the throw animation
    }
}
