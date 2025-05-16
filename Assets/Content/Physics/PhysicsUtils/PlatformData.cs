using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlatformData
{
    [Header("Platform Data")]
    public GameObject platformObject = null; // Object
    public Vector3 platformHit = Vector3.zero; // Where Character stands on the platform
    public Rigidbody platformRb = null; // Platform Rigidbody if it has one

    [Header("Calculations")]
    private bool calculatedMovement = false;
    private Vector3 platformVelocity = Vector3.zero; // Platform Velocity
    private Vector3 platLastPosition = Vector3.zero; // Last Position of the Platform
    private Vector3 platLastRotation = Vector3.zero; // Last Rotation of the Platform
    private Vector3 platRelativePosition = Vector3.zero; // Relative Position of the Character to the Platform

    [Header("To be moved")]
    private Vector3 deltaPosition = Vector3.zero;
    private Vector3 deltaAngle = Vector3.zero;
    private Vector3 relativePositionChange = Vector3.zero;

    public void Reset()
    {
        // platform data
        platformObject = null;
        platformHit = Vector3.zero;
        platformRb = null;

        // moving platform data
        calculatedMovement = false;
        platformVelocity = Vector3.zero;
        platLastPosition = Vector3.zero;
        platLastRotation = Vector3.zero;
        platRelativePosition = Vector3.zero; // character midpoint
    }

    public void FU_Platform(Transform characterTransform)
    {
        if (platformObject != null)
        {
            UpdatePlatformMovingData(characterTransform);
            HandlePosChange(characterTransform);
        }
    }

    public void SetPlatform(GameObject _platform, Transform _characterTransform)
    {
        if (_platform == platformObject)
        // alte platform entdeckt
        {
            calculatedMovement = true;
            // platformHit = Vector3.zero;
        }
        else
        // neue platform entdeckt
        {
            calculatedMovement = false;

            // platform data
            platformObject = _platform;
            platformHit = Vector3.zero;
            if (_platform.tag == "PlatformRB")
            {
                platformRb = _platform.GetComponent<Rigidbody>();
            }
        }

        if (_platform == null)
        {
            Reset();
        }
    }

    public void UpdatePlatformMovingData(Transform _characterTransform)
    {
        if (calculatedMovement)
        {
            // Set position
            deltaPosition = platformObject.transform.position - platLastPosition;
            platformVelocity = deltaPosition;

            // Set angle
            var angle = platformObject.transform.eulerAngles.y - platLastRotation.y;
            if (angle > 180) { angle -= 360f; }
            if (angle < 180) { angle += 360f; }
            deltaAngle = new Vector3(0f, angle, 0f);

            // Set relative position when rotating
            Vector3 relate = platRelativePosition; // character position to midpoint of platform
            relate = Quaternion.Euler(deltaAngle) * relate;
            Vector3 change = relate - platRelativePosition;
            relativePositionChange = change;
        }

        // Reset values
        platLastPosition = platformObject.transform.position;
        platLastRotation = platformObject.transform.eulerAngles;
        platRelativePosition = _characterTransform.position - platformObject.transform.position;
        platRelativePosition.y = 0f;
    }

    public void HandlePosChange(Transform _object)
    {
        if (calculatedMovement)
        {
            // Move Position
            _object.position += deltaPosition;

            // Move Rotation
            Vector3 newEuler = _object.eulerAngles + deltaAngle;
            _object.eulerAngles = newEuler;

            // Handle Relative PosChange
            _object.position += relativePositionChange;

            // set zero
            calculatedMovement = false;
            deltaPosition = Vector3.zero;
            deltaAngle = Vector3.zero;
            relativePositionChange = Vector3.zero;
        }
    }
}