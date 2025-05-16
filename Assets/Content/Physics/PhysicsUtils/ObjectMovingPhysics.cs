using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectMovingPhysics
{
    [Header("References")]
    private Essentials essentials;

    [Header("GravityForce")]
    private float gravityForce = 10f;
    private Vector3 gravityDir = Vector3.down;

    [Header("ForceMode")]
    private bool withForce = false;
    private float springStrength = 150f;
    private float damping = 15f;
    private float maxSpringForce = 25f;

    [Header("DragForce")]
    private float airDrag = 0.002f;
    private float waterDrag = 0.03f;
    private float groundFriction = 0.1f;

    [Header("Platform")]
    private Transform platform;
    private Vector3 platformVelocity;
    private Vector3 platLastPosition;
    private Vector3 platLastRotation;
    private Vector3 platRelativePosition;

    [Header("Ledge")]
    private Vector3 ledgePos;

    public void Init(Essentials _essentials)
    {
        essentials = _essentials;
    }

    public void Gravity()
    {
        essentials.rigidbody.AddForce(gravityDir * gravityForce * essentials.weight * Time.deltaTime, ForceMode.Acceleration);
    }

    public void AirDrag()
    {
        ApplyDrag(airDrag);
    }

    public void WaterDrag()
    {
        ApplyDrag(waterDrag);
    }

    private void ApplyDrag(float dragCoefficient)
    {
        var horizontalVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
        var verticalVelocity = new Vector3(0f, essentials.rigidbody.linearVelocity.y, 0f);
        var horizontalDrag = dragCoefficient * horizontalVelocity.magnitude * horizontalVelocity.magnitude;
        var verticalDrag = dragCoefficient * verticalVelocity.magnitude * verticalVelocity.magnitude;
        var horizontalForce = horizontalDrag * horizontalVelocity.normalized * -1f;
        var verticalForce = verticalDrag * verticalVelocity.normalized * -1f;
        essentials.rigidbody.linearVelocity += horizontalForce + verticalForce;
    }

    public void GroundFriction()
    {
        Vector3 currentVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
        Vector3 newVel = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction);
        Vector3 wantVel = new Vector3(newVel.x, essentials.rigidbody.linearVelocity.y, newVel.z);
        essentials.rigidbody.linearVelocity = wantVel;
    }

    public void GroundForce(Rigidbody groundBody, Vector3 platformHit)
    {
        groundBody.AddForceAtPosition(-Vector3.up * (essentials.rigidbody.mass / groundBody.mass), platformHit, ForceMode.Acceleration);
    }

    public void GroundAlign(float rideHeight, float offsetHeight, float groundSpeed, bool onlyUpwards, bool onlyDownwards, Vector3 groundNormal)
    {
        if (withForce)
        {
            Vector3 forceDir = groundNormal * -1f;
            float forceOffset = offsetHeight - rideHeight;
            float relVel = Vector3.Dot(forceDir, essentials.rigidbody.linearVelocity);
            float springForce = forceOffset * springStrength - damping * relVel;
            springForce = Mathf.Clamp(springForce, -maxSpringForce, maxSpringForce);
            essentials.rigidbody.AddForce(forceDir * springForce);
        }
        else
        {
            // only upwards
            if (onlyUpwards)
            {
                float offset = offsetHeight - rideHeight;
                if (offset < 0f)
                {
                    Vector3 targetPos = new Vector3(essentials.rigidbody.transform.position.x, essentials.rigidbody.transform.position.y - offset, essentials.rigidbody.transform.position.z);
                    targetPos = Vector3.MoveTowards(essentials.rigidbody.transform.position, targetPos, groundSpeed);
                    essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
                    essentials.rigidbody.transform.position = targetPos;
                }
            }
            else if (onlyDownwards)
            {
                float offset = offsetHeight - rideHeight;
                if (offset > 0f)
                {
                    Vector3 targetPos = new Vector3(essentials.rigidbody.transform.position.x, essentials.rigidbody.transform.position.y - offset, essentials.rigidbody.transform.position.z);
                    targetPos = Vector3.MoveTowards(essentials.rigidbody.transform.position, targetPos, groundSpeed);
                    essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
                    essentials.rigidbody.transform.position = targetPos;
                }
            }
            // Keeps character floating above ground / up and downwards
            else
            {
                float offset = offsetHeight - rideHeight;
                Vector3 targetPos = new Vector3(essentials.rigidbody.transform.position.x, essentials.rigidbody.transform.position.y - offset, essentials.rigidbody.transform.position.z);
                targetPos = Vector3.MoveTowards(essentials.rigidbody.transform.position, targetPos, groundSpeed);
                essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
                essentials.rigidbody.transform.position = targetPos;
            }
        }
    }

    public void HandlePosChange(bool _resetPlatform, Transform platform)
    {
        if (_resetPlatform)
        {
            platLastPosition = platform.position;
            platLastRotation = platform.eulerAngles;
            platRelativePosition = essentials.rigidbody.transform.position - platform.transform.position;
            platRelativePosition.y = 0f;
        }
        else
        {
            // Set position
            Vector3 deltaPosition = platform.position - platLastPosition;
            essentials.rigidbody.transform.position += deltaPosition;
            platformVelocity = deltaPosition;

            // Set angle
            var angle = platform.eulerAngles.y - platLastRotation.y;
            if (angle > 180) { angle -= 360f; }
            if (angle < 180) { angle += 360f; }
            Vector3 newEuler = new Vector3(essentials.rigidbody.transform.eulerAngles.x, essentials.rigidbody.transform.eulerAngles.y + angle, essentials.rigidbody.transform.eulerAngles.z);
            essentials.rigidbody.transform.eulerAngles = newEuler;

            // Set relative position when rotating
            Vector3 relate = platRelativePosition;
            relate = Quaternion.Euler(0f, angle, 0f) * relate;
            Vector3 change = relate - platRelativePosition;
            essentials.rigidbody.transform.position += change;

            // Reset values
            platLastPosition = platform.position;
            platLastRotation = platform.eulerAngles;
            platRelativePosition = essentials.rigidbody.transform.position - platform.transform.position;
            platRelativePosition.y = 0f;
        }
    }

    public void HandleLedgePosChange(bool _resetPlatform, Transform platform)
    {
        if (_resetPlatform)
        {
            platLastPosition = platform.position;
            platLastRotation = platform.eulerAngles;
            platRelativePosition = essentials.rigidbody.transform.position - platform.transform.position;
            // platRelativePosition.y = 0f;

            return;
        }

        // Set position
        Vector3 deltaPosition = platform.position - platLastPosition;
        essentials.rigidbody.transform.position += deltaPosition;
        platformVelocity = deltaPosition;
        ledgePos += deltaPosition;

        // Set angle
        var angle = platform.eulerAngles.y - platLastRotation.y;
        if (angle > 180) { angle -= 360f; }
        if (angle < 180) { angle += 360f; }
        var angleX = platform.eulerAngles.x - platLastRotation.x;
        var angleZ = platform.eulerAngles.z - platLastRotation.z;

        Vector3 newEuler = new Vector3(essentials.rigidbody.transform.eulerAngles.x, essentials.rigidbody.transform.eulerAngles.y + angle, essentials.rigidbody.transform.eulerAngles.z);
        essentials.rigidbody.transform.eulerAngles = newEuler;

        // Set relative position when rotating
        Vector3 relate = platRelativePosition;
        relate = Quaternion.Euler(0f, angle, 0f) * relate;
        Vector3 change = relate - platRelativePosition;
        essentials.rigidbody.transform.position += change;
        ledgePos += change;

        Vector3 l = ledgePos - platform.position;
        Vector3 r = l;
        r = Quaternion.Euler(angleX, 0f, angleZ) * r;
        Vector3 c = r - l;
        essentials.rigidbody.transform.position += c;
        ledgePos += c;

        // Reset values
        platLastPosition = platform.position;
        platLastRotation = platform.eulerAngles;
        platRelativePosition = essentials.rigidbody.transform.position - platform.transform.position;
    }
}
