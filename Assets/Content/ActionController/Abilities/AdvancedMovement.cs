using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TrySprint
// TryCrouching

// Update Crouching Time -> Force Crouching or Break Crouching -> Move
// Update Sprinting Time -> Sprinting Break -> Move
// Update Strafing Time -> Strafing Break -> Move

// Movement ->

// Air Movement -> 
// Slope Movement
// Normal Movement


[System.Serializable]
public class AdvancedMovementData
{
    [Header("Basics")]
    public float moveForce = 3f;
    // public float crawlForce = 0.75f;
    public float crouchForce = 1.5f;
    public float sprintForce = 4f;
    public float accelerationFactor = 0.25f;
    [HideInInspector]
    public AnimationCurve accelerationCurve = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 1),
            new Keyframe(2, 2)
        );

    [Header("Ground")]
    public bool allowSlopeMovement = true;
    public float slopeMinFactor = 0.9f;
    public float slopeMaxFactor = 1.1f;

    [Header("AirMovement")]
    public bool allowAirMovement = true;
    public float airPenalty = 0.5f;

    [Header("Sound")]
    public bool movementSound;
    public AudioClip[] moveSounds;
    public int minMoveCount = 2;
    public int maxMoveCount = 3;

    [Header("Stamina")]
    public float sprintStaminaCost = 2f;

    [Header("Particle")]
    public bool enableParticles;
    public ParticleSystem moveParticles;
}

[System.Serializable]
public class AdvancedMovement
{
    [Header("Components")]
    private Essentials essentials;
    private ActionSound actionSound;
    private Surroundings surroundings;
    public AdvancedMovementData data;
    public StaminaManager staminaManager;

    [Header("SubState")]
    // [HideInInspector] public bool crawling = false;
    [HideInInspector] public bool crouching = false;
    [HideInInspector] public bool sprinting = false;
    [HideInInspector] public bool sliding = false;
    [HideInInspector] public bool strafing = false;
    private float footCount;

    public void Init(Essentials _essentials, ActionSound _actionSound, Surroundings _surroundings, StaminaManager _staminaManager)
    {
        essentials = _essentials;
        actionSound = _actionSound;
        surroundings = _surroundings;
        staminaManager = _staminaManager;
    }

    public void Setvalues(AdvancedMovementData _data)
    {
        data = _data;
    }

    public void FU_Movement(Vector3 moveDirection, bool wantCrouch, bool wantSprint, bool wantStrafe)
    {
        UpdateCrouching(wantCrouch);
        UpdateSprinting(wantSprint);
        UpdateStrafing(wantStrafe);
    }

    private void UpdateCrouching(bool _wantCrouch)
    {
        if (crouching)
        {
            if (!_wantCrouch && !surroundings.isSomethingAboveHead)
            {
                crouching = false;
            }

            if (!surroundings.IsGrounded() || !surroundings.IsSlope())
            {
                crouching = false;
            }
        }
        else
        {
            if (surroundings.IsGrounded() && !surroundings.IsSlope() && surroundings.isSomethingAboveHead)
            {
                crouching = true;
            }
        }
    }

    private void UpdateSprinting(bool _wantSprint)
    {
        if (sprinting)
        {
            if (!_wantSprint || !surroundings.IsGrounded() || surroundings.IsSlope() || crouching || strafing)
            {
                sprinting = false;
            }
        }
        else
        {
            if (_wantSprint && !crouching && !strafing)
            {
                if (!crouching && !strafing)
                {
                    sprinting = true;
                }
            }
        }
    }

    private void UpdateStrafing(bool _wantStrafe)
    {
        strafing = _wantStrafe;
    }

    public void Move(Vector3 moveDirection, bool wantCrouch, bool wantSprint, bool wantStrafe)
    {
        if (sprinting)
        {
            SprintingMovement();
        }
        else if (crouching)
        {
            CrouchingMovement();
        }
        else if (strafing)
        {
            StrafingMovement();
        }
        else
        {
            AirMovement();
        }

        float force = data.moveForce;
        if (crouching)
        {
            force = data.crouchForce;
        }
        else if (sprinting)
        {
            force = data.sprintForce;
        }
        else if (strafing)
        {
            force = 0.5f * data.moveForce;
        }

        // Calculate the dot product between moveDirection and rb.velocity
        float dotProduct = Vector3.Dot(moveDirection.normalized, essentials.rigidbody.linearVelocity.normalized);
        float mappedValue = data.accelerationCurve.Evaluate(dotProduct);

        // Normal Movement
        if (surroundings.foundGround && !surroundings.foundSlope)
        {
            float angleSpeed = 1f;
            float moveAngle = Vector3.Angle(surroundings.groundNormal, moveDirection);

            if (moveAngle > 90f && moveDirection != Vector3.zero)
            {
                angleSpeed = Mathf.Lerp(data.slopeMinFactor, 1f, 1f - (moveAngle - 90f) / 35f);
            }
            else if (moveAngle < 90f && moveDirection != Vector3.zero)
            {
                angleSpeed = Mathf.Lerp(1f, data.slopeMaxFactor, (-(moveAngle - 90f)) / 35f);
            }

            // Get current and wanted velocity
            Vector3 currentVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
            Vector3 goalVel = moveDirection * force * angleSpeed;

            // Calculate needed velocity
            float accel = data.accelerationFactor;
            if (moveDirection == Vector3.zero)
            {
                accel = 1f;
            }

            accel = accel * mappedValue;

            Vector3 newVel = Vector3.MoveTowards(currentVelocity, goalVel, accel);
            Vector3 wantVel = new Vector3(newVel.x, essentials.rigidbody.linearVelocity.y, newVel.z);
            Vector3 applyVelocity = wantVel - essentials.rigidbody.linearVelocity;
            essentials.rigidbody.AddForce(applyVelocity, ForceMode.VelocityChange);
        }
        else if (strafing && surroundings.foundGround && !surroundings.foundSlope)
        {
            float angleSpeed = 1f;
            float moveAngle = Vector3.Angle(surroundings.groundNormal, moveDirection);

            if (moveAngle > 90f && moveDirection != Vector3.zero)
            {
                angleSpeed = Mathf.Lerp(data.slopeMinFactor, 1f, 1f - (moveAngle - 90f) / 35f);
            }
            else if (moveAngle < 90f && moveDirection != Vector3.zero)
            {
                angleSpeed = Mathf.Lerp(1f, data.slopeMaxFactor, (-(moveAngle - 90f)) / 35f);
            }

            if (essentials.ForwardVelocity() < -0.2f && Mathf.Abs(essentials.SideVelocity()) > 0.2f)
            {
                force *= 0.75f;
            }

            // Get current and wanted velocity
            Vector3 currentVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
            Vector3 goalVel = moveDirection * force * angleSpeed;

            // Calculate needed velocity
            Vector3 newVel = Vector3.MoveTowards(currentVelocity, goalVel, data.accelerationFactor * mappedValue);
            Vector3 wantVel = new Vector3(newVel.x, essentials.rigidbody.linearVelocity.y, newVel.z);
            Vector3 applyVelocity = wantVel - essentials.rigidbody.linearVelocity;
            essentials.rigidbody.AddForce(applyVelocity, ForceMode.VelocityChange);
        }
        else
        {
            // Air Movement
            float accel = data.accelerationFactor * data.airPenalty; // mappedValue

            // Get current and wanted velocity
            Vector3 currentVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
            Vector3 goalVel = moveDirection * force;

            // Get acceleration of velocity
            float velDot = Vector3.Dot(Vector3.Normalize(goalVel), Vector3.Normalize(currentVelocity)) + 1f;
            Vector3 applyVelocity = Vector3.zero;

            if (currentVelocity.magnitude > data.moveForce && velDot > 1f)
            {
                // Without speeding up
                Vector3 wantVel = Vector3.MoveTowards(currentVelocity, goalVel, accel);
                Vector3 sideForce = Vector3.ProjectOnPlane(wantVel, currentVelocity) / (velDot);
                Vector3 speedLimit = Vector3.Normalize(currentVelocity) * sideForce.magnitude;
                applyVelocity = sideForce - speedLimit;
            }
            else
            {
                // Calculate needed velocity
                Vector3 newVel = Vector3.MoveTowards(currentVelocity, goalVel, accel);
                Vector3 wantVel = new Vector3(newVel.x, essentials.rigidbody.linearVelocity.y, newVel.z);
                applyVelocity = wantVel - essentials.rigidbody.linearVelocity;
            }
            essentials.rigidbody.AddForce(applyVelocity, ForceMode.VelocityChange);
        }

        MovementStepSound();
        MovementStepParticles();
    }

    public void NormalMovement()
    {
    }

    public void SprintingMovement()
    {

    }

    public void CrouchingMovement()
    {

    }

    public void StrafingMovement()
    {
    }

    public void AirMovement()
    {

    }

    private void SlopeMovement()
    {
        // Slope Movement equal to airmovement currently
        AirMovement();
    }

    private void MovementStepSound()
    {
        // Sound
        if (data.movementSound)
        {
            if (essentials.horizontalVelocity > 0.25f && surroundings.foundGround)
            {
                footCount -= Mathf.Lerp(data.minMoveCount, data.maxMoveCount, essentials.horizontalVelocity / data.moveForce);
                if (footCount <= 0)
                {
                    footCount = 50;
                    string randomStep = "locomotion/footsteps/step" + UnityEngine.Random.Range(1, 10).ToString();
                    float vol = Mathf.Lerp(0f, 0.5f, essentials.horizontalVelocity / data.moveForce);
                    AudioClip clip = ResourceSystem.GetSound(randomStep);
                    SoundData s = new SoundData(clip, vol);
                    if (s != null)
                    {
                        actionSound.PlaySound(s);
                    }
                }
            }
        }
    }

    private void MovementStepParticles()
    {
        // Particles
        if (data.enableParticles)
        {
            if (surroundings.foundGround && essentials.horizontalVelocity >= 1.5f)
            {
                data.moveParticles.Play();
            }
            else
            {
                data.moveParticles.Stop();
            }
        }
    }
}