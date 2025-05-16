using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Surroundings
{
    [Header("References")]
    // It needs the essentials to be working
    private Essentials essentials; // character stats

    [Header("Ground Scanner")]
    // Not reliable ground detection
    private int groundCheckPointsCount = 3; // points of each circle
    private int groundCheckCircleCount = 2; // circle around the player
    private Vector3[] groundCheckPoints = new Vector3[0]; // saving the generated points + midPoint
    private float groundCheckPointsAmount; // amount of groundCheckPoints

    [Header("Head")]
    // Simple check if something is above the players head
    public bool isSomethingAboveHead = false; // only works in standing mode
    public float aboveFree = 0.1f; // value for headcheckdistance

    [Header("Water")]
    // Checks basic on or in Water States
    public bool inWater = false; // is player inside water swimming or diving?

    [Header("Drowning")]
    // is player drowning in water?
    public bool isDrowning = false; // checks if head is inside water

    [Header("Water Surface")]
    // check if player is swimming on water surface
    public bool foundWaterSurface = false; // if water surface is found
    public float waterLevel = 0f; // difference on water surface to swimheight

    [Header("Ground Info")]
    public bool foundGround = false;
    public bool foundSlope = false;
    public float groundAngle = 0f;
    public Vector3 groundNormal = Vector3.zero;
    public float hitDistance = 99f;
    public bool foundShallowWater = false;
    public float shallowWaterFactor = 0f;

    [Header("AdvancedGrounding")]
    // public bool canGround;
    // public bool isGrounded = false;
    public bool useGroundingTime = false;
    public float groundingDuration = 0.1f;
    private float groundTime;
    public bool useCoyote = false;
    public float coyoteDuration = 0.2f;
    private float coyoteTime;

    [Header("Platform & Ledge")]
    // Checks if character is on a platform
    public bool foundPlatform = false; // was a platform found?
    public bool resetPlatform = false;
    public PlatformData platformData = new PlatformData(); // contains platformData

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(Essentials _essentials)
    {
        essentials = _essentials;
    }

    // Update is called once per frame
    public void FU_Surroundings()
    {
        // _____________________________
        // creating ground check points 
        CreateGroundPoints();

        // _____________________________
        // if player is in water
        // if player is drowning 
        CheckWater();
        CheckDrowning();

        // _____________________________
        // only if in water:
        // checks waterlevel
        CheckWaterSurface();

        // _____________________________
        // checks if something is above the character
        CheckAboveHead();

        // _____________________________
        // ground check the surface for:
        // slopes
        // ground distance
        // platform
        CheckGround();

        // _____________________________
        // checks if player stands in shallow water
        // shallow water factor
        CheckShallowWater();

        // _____________________________
        // update advanced grounding
        // Coyote Time
        // grounding Time
        UpdateGround();

        // _____________________________
        // CanGround() -> checks if vel is under 0.1f
    }

    private void CreateGroundPoints()
    // This method creates transform points in multiple scaled circles to later
    // be projected onto the floor, this ensures dynamic ground checking
    // not efficient and not reliable
    // projecting a plan could be better! -> ?
    {
        // Calculate Vector3 amount
        groundCheckPoints = null;
        int pos = 1;
        for (int j = 0; j < groundCheckCircleCount; j++)
        {
            for (int i = 0; i < groundCheckPointsCount * (j + 1); i++)
            {
                pos += 1;
            }
        }
        groundCheckPoints = new Vector3[pos];
        groundCheckPointsAmount = pos;

        // Set Center Point
        groundCheckPoints[0] = essentials.midPoint.transform.position;
        pos = 1;

        // Add Circle Points
        for (int j = 0; j < groundCheckCircleCount; j++)
        {
            float radius = (essentials.characterRadius / groundCheckCircleCount) * (j + 1);
            float angleStep = 360f / (groundCheckPointsCount * (j + 1));

            for (int i = 0; i < groundCheckPointsCount * (j + 1); i++)
            {
                Vector3 pointDirection = Quaternion.AngleAxis(i * angleStep, Vector3.up) * Vector3.forward;
                groundCheckPoints[pos] = essentials.midPoint.transform.position + pointDirection * radius;
                pos++;
            }
        }
    }

    private void CheckWater()
    // Checks if player is inside water
    {
        // using swimheight + rideheight and character radius to check if he should swim
        inWater = Physics.CheckSphere(essentials.midPoint.transform.position + Vector3.up * essentials.swimHeight, essentials.characterRadius, essentials.waterLayer);
    }

    private void CheckDrowning()
    // Checks if player is drowning
    {
        if (inWater)
        {
            // drowning is checked at character headHeight
            isDrowning = Physics.CheckSphere(essentials.midPoint.transform.position + Vector3.up * essentials.headHeight, 0.1f, essentials.waterLayer);
        }
        else
        {
            isDrowning = false;
        }
    }

    private void CheckWaterSurface()
    // if water surface is found it needs to be evaluated
    {
        // Shoots ray downwards from swimgheight and checks if water surface was found
        bool _foundWaterSurface = false;

        if (inWater)
        {
            RaycastHit waterHit;
            if (Physics.Raycast(essentials.midPoint.transform.position + Vector3.up * (essentials.swimHeight + essentials.characterRadius), -Vector3.up, out waterHit, essentials.characterRadius * 2f, essentials.waterLayer))
            {
                Vector3 waterSwimLevel = essentials.midPoint.transform.position + Vector3.up * essentials.swimHeight;
                waterLevel = waterHit.point.y - waterSwimLevel.y;
                _foundWaterSurface = true;
            }
        }

        // set foundwatersurface
        foundWaterSurface = _foundWaterSurface;

        // Reset Water Value if not found
        if (!foundWaterSurface)
        {
            waterLevel = 0f;
        }
    }

    private void CheckAboveHead()
    // Simple Capsule projectin if something is above the head
    {
        isSomethingAboveHead = Physics.CheckCapsule(essentials.midPoint.transform.position, essentials.midPoint.transform.position + Vector3.up * (essentials.rideHeight + aboveFree - essentials.characterRadius), essentials.characterRadius * 0.9f, essentials.groundLayer);
    }

    private void CheckGround()
    // Checks the players ground
    // only grounded when Vertical Velocity > 0.1f !?
    {
        // contains raycast info
        RaycastHit findGround;

        // Distance to be checked
        hitDistance = 20f;

        // number of hits
        int numHits = 0;

        // average hit distance
        float test = 0;

        // normal of ground
        foundSlope = false;
        groundNormal = Vector3.zero;
        groundAngle = 0f;

        // shallow water
        foundShallowWater = false;
        shallowWaterFactor = 0f;

        // platform
        int platformHits = 0;
        foundPlatform = false;
        Vector3 platformForce = Vector3.zero;
        GameObject foundPlatformObject = null;

        // 1. Cast a sphere to check if ground can be found
        bool sphereGround = false;
        if (!foundGround)
        {
            // Draws grounded circle to check if player is grounded
            sphereGround = Physics.SphereCast(groundCheckPoints[0], essentials.characterRadius, -Vector3.up, out findGround, essentials.rideHeight - essentials.characterRadius, essentials.groundLayer);
        }

        // can or should check ground details?
        if (sphereGround)
        {
            // only say found ground when vertical velocity is positiv?
            bool allowGrounding = CanGround();
            sphereGround = allowGrounding;
        }

        if (!sphereGround && !foundGround)
        {
            // Reset ground data
            SetOffGroundData();

            // nah maybe?
            foundShallowWater = false;
            shallowWaterFactor = 0f;
        }

        if (sphereGround || foundGround)
        {
            foreach (Vector3 p in groundCheckPoints)
            {
                if (Physics.Raycast(p, Vector3.down, out findGround, (essentials.rideHeight + essentials.stairHeight), essentials.groundLayer))
                {
                    DrawRays.DrawRay(p, -Vector3.up, DrawRays.scanColor);
                    // if (findGround.distance >= (essentials.rideHeight - essentials.stairHeight))
                    // {
                    numHits++;

                    groundNormal += Vector3.Normalize(findGround.normal);
                    if (findGround.transform.gameObject.tag == "Platform" || findGround.transform.gameObject.tag == "PlatformRB")
                    {
                        foundPlatform = true;
                        if (foundPlatformObject == null)
                        {
                            foundPlatformObject = findGround.transform.gameObject;
                        }
                        platformForce += findGround.point;
                        platformHits++;
                    }
                    if (findGround.distance < hitDistance)
                    {
                        hitDistance = findGround.distance;
                    }
                    test += findGround.distance;
                    // }
                }
            }

            hitDistance = test / numHits;
            foundGround = numHits > 0.25f * groundCheckPointsAmount;
            groundNormal /= numHits;
            groundAngle = Vector3.Angle(Vector3.up, groundNormal);
            foundSlope = groundAngle > 35f;

            if (foundPlatform && foundGround)
            {
                // SetPlatform
                platformData.platformHit = platformForce / numHits;
                platformData.SetPlatform(foundPlatformObject, essentials.midPoint.transform);
            }
        }
    }

    public void CheckShallowWater()
    // checks for shallow water
    {
        // Raycast water hitpoint from center point
        RaycastHit findWater;
        if (Physics.Raycast(essentials.midPoint.transform.position + Vector3.up * essentials.swimHeight, -Vector3.up, out findWater, essentials.rideHeight + essentials.swimHeight, essentials.waterLayer))
        {
            DrawRays.DrawRay(essentials.midPoint.transform.position, -Vector3.up, DrawRays.scanColor);
            foundShallowWater = true;
            // shallowWaterFactor = findWater.distance; (essentials.swimHeight + essentials.rideHeight);
            shallowWaterFactor = (1 - (findWater.distance / (essentials.swimHeight + essentials.rideHeight)));
        }
    }

    // ______________________________________________________________
    // Adanved Grounding

    // checks if player should be able to ground
    public bool CanGround()
    {
        if (essentials.rigidbody.linearVelocity.y < 0.1f)
        {
            return true;
        }
        return false;
    }

    // needs to be called whenever player gets self detached from ground like jump
    public void SetOffGround()
    {
        coyoteTime = 0f;
        SetOffGroundData();
    }

    private void SetOffGroundData()
    {
        // disable ground connection
        foundGround = false;
        foundSlope = false;
        groundAngle = 0f;
        groundNormal = Vector3.zero;
        hitDistance = 0f;

        // advanced grounding
        groundTime = 0f;

        // platform
        DetachFromPlatform();
    }

    public void DetachFromPlatform()
    {
        platformData.Reset();
    }

    public bool IsGrounded()
    {
        return foundGround;
    }

    public bool IsGroundingTime()
    {
        if (useGroundingTime)
        {
            return groundTime >= groundingDuration;
        }
        return foundGround;
    }

    public bool IsSlope()
    {
        if (foundSlope)
        {
            return true;
        }

        return false;
    }

    public bool IsCoyote()
    {
        if (useCoyote)
        {
            return coyoteTime > 0f;
        }
        return foundGround;
    }

    public void UpdateGround()
    {
        UpdateGroundTime();
        UpdateCoyoteTime();
    }

    private void UpdateGroundTime()
    {
        if (foundGround)
        {
            if (groundTime < groundingDuration)
            {
                groundTime += Time.deltaTime;
            }
        }
        else
        {
            groundTime = 0f;
        }
    }

    private void UpdateCoyoteTime()
    {
        if (IsGrounded())
        {
            coyoteTime = coyoteDuration;
        }
        else
        {
            if (coyoteTime > 0)
            {
                coyoteTime -= Time.deltaTime;
                coyoteTime = coyoteTime < 0 ? 0 : coyoteTime;
            }
        }
    }
}
