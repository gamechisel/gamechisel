using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionClimbData
{
    [Header("Climb Settings")]
    public float rayDistance = 1.0f; // Distance of the rays for detection
    public LayerMask climbableLayer; // Layer for climbable objects
    public string climbableTag = "Climbable"; // Tag for climbable objects
    public float maxClimbAngle = 10f; // Maximum angle deviation for climbable surfaces
    public float alignmentSpeed = 5f; // Speed to align the player with the wall
    public float climbSpeed = 3f; // Movement speed while climbing

    [Header("Ray Offsets")]
    public float rayVerticalOffsetUp = 0.5f; // Offset for the upper ray
    public float rayVerticalOffsetDown = -0.3f; // Offset for the lower ray
}

[System.Serializable]
public class ActionClimb
{
    public ActionClimbData actionClimbData;

    private Essentials essentials;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;

    public bool isClimbing = false;

    public void Init(Essentials _essentials, ActionAnimation _actionAnimation, PlayerActionState _state)
    {
        essentials = _essentials;
        actionAnimation = _actionAnimation;
        state = _state;
    }

    public void TryClimb()
    {
        // Perform raycasts
        RaycastHit middleHit;
        bool canMoveUp, canMoveDown, canMoveSideways;
        bool canClimb = CheckClimbable(out middleHit, out canMoveUp, out canMoveDown, out canMoveSideways);

        if (canClimb)
        {
            StartClimbing(middleHit);
        }
    }

    private bool CheckClimbable(out RaycastHit middleHit, out bool canMoveUp, out bool canMoveDown, out bool canMoveSideways)
    {
        middleHit = new RaycastHit();

        // Initialize movement flags
        canMoveUp = false;
        canMoveDown = false;
        canMoveSideways = false;

        Vector3 origin = essentials.rigidbody.transform.position;

        // Check each layer
        bool upperLayerHit = CheckLayer(origin + Vector3.up * actionClimbData.rayVerticalOffsetUp, out RaycastHit upperHit);
        bool middleLayerHit = CheckLayer(origin, out RaycastHit midHit);
        bool lowerLayerHit = CheckLayer(origin + Vector3.up * actionClimbData.rayVerticalOffsetDown, out RaycastHit lowerHit);

        // Set movement flags
        if (upperLayerHit) canMoveUp = true;
        if (lowerLayerHit) canMoveDown = true;
        if (middleLayerHit)
        {
            canMoveSideways = true; // Sideways movement requires middle layer hit
            middleHit = midHit; // Use middle layer hit for alignment
        }

        // Climbing is only possible if all layers have valid hits
        return upperLayerHit && middleLayerHit && lowerLayerHit;
    }

    private bool CheckLayer(Vector3 layerOrigin, out RaycastHit layerHit)
    {
        layerHit = new RaycastHit();

        // Define ray directions (center, left, right)
        Vector3 forward = essentials.rigidbody.transform.forward;
        Vector3 right = essentials.rigidbody.transform.right;
        Vector3[] directions = new Vector3[]
        {
            forward,                         // Center
            forward - right * 0.5f,          // Slightly left
            forward + right * 0.5f           // Slightly right
        };

        // Perform 3 raycasts for the layer
        foreach (var dir in directions)
        {
            if (Physics.Raycast(layerOrigin, dir, out RaycastHit hit, actionClimbData.rayDistance, actionClimbData.climbableLayer))
            {
                if (ValidateClimbable(hit))
                {
                    layerHit = hit;
                    return true; // Return as soon as we find a valid surface
                }
            }
        }

        return false;
    }

    private bool ValidateClimbable(RaycastHit hit)
    {
        // Check tag
        if (!hit.collider.CompareTag(actionClimbData.climbableTag)) return false;

        // Check angle
        Vector3 normal = hit.normal;
        float angle = Vector3.Angle(Vector3.up, normal);
        return Mathf.Abs(90f - angle) <= actionClimbData.maxClimbAngle;
    }

    private void StartClimbing(RaycastHit hit)
    {
        isClimbing = true;
        essentials.rigidbody.linearVelocity = Vector3.zero;

        // Align the player with the wall
        AlignToWall(hit);

        // Set climbing animation and state
        state.SetActionState("climbing", 0.04f);
        actionAnimation.PlayAnimation("climb");
    }

    private void AlignToWall(RaycastHit hit)
    {
        Vector3 wallNormal = hit.normal;
        Quaternion targetRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
        essentials.rigidbody.transform.rotation = Quaternion.Slerp(essentials.rigidbody.transform.rotation, targetRotation, actionClimbData.alignmentSpeed * Time.deltaTime);
    }

    public void ClimbMovement(Vector2 input)
    {
        // Check climbability and movement possibilities
        RaycastHit middleHit;
        bool canMoveUp, canMoveDown, canMoveSideways;
        bool canClimb = CheckClimbable(out middleHit, out canMoveUp, out canMoveDown, out canMoveSideways);

        if (!canClimb)
        {
            StopClimbing();
            return;
        }

        // Calculate movement
        Vector3 moveDirection = Vector3.zero;

        if (input.y > 0 && canMoveUp) // Move up
        {
            moveDirection += Vector3.up;
        }
        else if (input.y < 0 && canMoveDown) // Move down
        {
            moveDirection -= Vector3.up;
        }

        if (input.x != 0 && canMoveSideways) // Move sideways
        {
            moveDirection += essentials.rigidbody.transform.right * input.x;
        }

        // Apply movement
        essentials.rigidbody.linearVelocity = moveDirection.normalized * actionClimbData.climbSpeed;

        // Align to wall
        AlignToWall(middleHit);
    }

    private void StopClimbing()
    {
        isClimbing = false;

        // Reset to idle state
        actionAnimation.PlayAnimation("idle");
        // state.ClearActionState("climbing");
    }

    private void DrawDebugMovement(bool canMoveUp, bool canMoveDown, bool canMoveSideways)
    {
        Vector3 origin = essentials.rigidbody.transform.position;

        // Colors for movement directions
        Color upColor = canMoveUp ? Color.green : Color.red;
        Color downColor = canMoveDown ? Color.green : Color.red;
        Color sideColor = canMoveSideways ? Color.green : Color.red;

        // Debug rays for visualization
        Debug.DrawRay(origin + Vector3.up * actionClimbData.rayVerticalOffsetUp, Vector3.up * 0.5f, upColor);
        Debug.DrawRay(origin + Vector3.up * actionClimbData.rayVerticalOffsetDown, Vector3.down * 0.5f, downColor);
        Debug.DrawRay(origin, essentials.rigidbody.transform.right * 0.5f, sideColor);
        Debug.DrawRay(origin, -essentials.rigidbody.transform.right * 0.5f, sideColor);
    }
}
