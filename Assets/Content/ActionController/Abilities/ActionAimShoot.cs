using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionAimShootData
{
    [Header("Aiming Data")]
    public float aimZoomFactor = 0.5f; // Camera zoom factor when aiming
    public float aimDuration = 0.3f; // Duration for aim transition
    public AudioClip aimSound = null; // Sound played when aiming

    [Header("Shooting Data")]
    public float shootCooldown = 0.5f; // Time between shots
    public AudioClip shootSound = null; // Sound played when shooting
    public GameObject bulletPrefab = null; // Bullet prefab to instantiate
    public Transform shootPoint = null; // The point where bullets spawn
    public float bulletSpeed = 20f; // Speed of the bullet
}

[System.Serializable]
public class ActionAimShoot
{
    public ActionAimShootData actionAimShootData;

    private Essentials essentials;
    private ActionSound actionSound;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;

    private bool isAiming = false;
    private float lastShootTime = 0f;

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
        if (isAiming) return; // Already aiming

        isAiming = true;

        // Play aim animation and sound
        actionAnimation.PlayAnimation("aim");
        if (actionAimShootData.aimSound != null)
        {
            actionSound.PlaySound(new SoundData(actionAimShootData.aimSound, 1f));
        }

        // Adjust camera zoom (if applicable)
        // Assuming the camera zoom logic is part of the Essentials or another script
        // essentials.cameraController.SetZoom(actionAimShootData.aimZoomFactor, actionAimShootData.aimDuration);

        state.SetActionState("aim", actionAimShootData.aimDuration);
    }

    // Stop Aiming
    public void StopAim()
    {
        if (!isAiming) return; // Not aiming

        isAiming = false;

        // Stop aim animation
        actionAnimation.PlayAnimation("idle"); // Return to idle animation

        // Reset camera zoom
        // essentials.cameraController.ResetZoom(actionAimShootData.aimDuration);

        // state.ClearActionState("aim");
    }

    // Shoot
    public void Shoot()
    {
        if (Time.time - lastShootTime < actionAimShootData.shootCooldown) return; // Check cooldown
        if (!isAiming) return; // Can only shoot while aiming

        lastShootTime = Time.time;

        // Play shooting animation and sound
        actionAnimation.PlayAnimation("shoot");
        if (actionAimShootData.shootSound != null)
        {
            actionSound.PlaySound(new SoundData(actionAimShootData.shootSound, 1f));
        }

        // Spawn the bullet
        if (actionAimShootData.bulletPrefab != null && actionAimShootData.shootPoint != null)
        {
            GameObject bullet = GameObject.Instantiate(
                actionAimShootData.bulletPrefab,
                actionAimShootData.shootPoint.position,
                actionAimShootData.shootPoint.rotation
            );

            // Apply velocity to the bullet
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.linearVelocity = actionAimShootData.shootPoint.forward * actionAimShootData.bulletSpeed;
            }
        }

        state.SetActionState("shoot", actionAimShootData.shootCooldown); // Shooting state for cooldown duration
    }
}
