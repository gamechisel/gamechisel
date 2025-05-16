using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionJumpData
{
    [Header("Simple Jump")]
    public float jumpForce = 4f;
    public float jumpDuration = 0.2f;
    public float jumpRedirectSpeed = 0.1f;
    public float jumpMinTime = 0.1f;
    public AudioClip jumpSound = null;
    public ParticleSystem jumpParticles;
    public float staminaCost = 10f;

    [Header("Air Jump")]
    public bool canAirJump;
    public float airJumpForce = 4f;
    public AudioClip airJumpSound = null;
    public ParticleSystem airJumpParticles;

    [Header("Crouch Jump")]
    public bool canCrouchJump = false;
    public float crouchJumpForce = 6f;
    public AudioClip crouchJumpSound = null;
    public ParticleSystem crouchJumpParticles;

    [Header("Roll Jump")]
    public bool canRollJump = false;
    public float forwardJumpForce = 3f;
    public float forwardUpForce = 3f;
    public AudioClip rollJumpSound = null;
    public ParticleSystem rollJumpParticles;

    [Header("Height Jump")]
    public bool canHeightJump = false;
    public float heightJumpForce = 6f;
    public AudioClip heightJumpSound = null;
    public ParticleSystem heightJumpParticles;
}

[System.Serializable]
public class ActionJump
{
    [Header("Jump Data")]
    public ActionJumpData jumpData;

    [Header("References")]
    private Essentials essentials;
    private Surroundings surroundings;
    private PlayerActionState state;
    private ActionAnimation actionAnimation;
    private ActionSound actionSound;
    private StaminaManager staminaManager;
    private bool hasAirJump = false;
    private bool wasRollJumping = false;
    private bool wasRollJumpingDebug = false;

    public void Init(Essentials _essentials, ActionAnimation _actionAnimation, ActionSound _actionSound, PlayerActionState _state, Surroundings _surroundings, StaminaManager _staminaManager)
    {
        state = _state;
        actionAnimation = _actionAnimation;
        actionSound = _actionSound;
        surroundings = _surroundings;
        essentials = _essentials;
        staminaManager = _staminaManager;
    }

    public void SetAirJump(bool b)
    {
        hasAirJump = b;
    }

    public void SetOffGround()
    {
        surroundings.SetOffGround();
    }

    public void UpdateJump()
    {
        UpdateAirJump();
        UpdateRollJumpingCombo();
    }

    private void UpdateAirJump()
    {
        if (surroundings.IsGrounded() && !surroundings.IsSlope() && !surroundings.inWater)
        {
            SetAirJump(true);
        }

        if (surroundings.inWater)
        {
            SetAirJump(false);
        }
    }

    private void UpdateRollJumpingCombo()
    {
        if (wasRollJumping && !state.currentState.Equals("rolljump"))
        {
            if (!state.currentState.Equals("none") || surroundings.IsGrounded() && !surroundings.IsSlope())
            {
                if (wasRollJumpingDebug)
                {
                    wasRollJumpingDebug = false;
                }
                else
                {
                    wasRollJumping = false;
                }
            }
        }
    }

    // _________________________________ Try Jumping
    public bool TryJump()
    {
        if (staminaManager.currentStamina < jumpData.staminaCost) { return false; }

        if (surroundings.IsGrounded() && !surroundings.IsSlope() && wasRollJumping)
        {
            staminaManager.currentStamina -= jumpData.staminaCost;
            // Height Jump
            wasRollJumping = false;
            wasRollJumpingDebug = false;
            HeightJump();

            return true;
        } else if (surroundings.IsCoyote() && !surroundings.IsSlope())
        {
            staminaManager.currentStamina -= jumpData.staminaCost;
            // Simple Jump
            Jump(jumpData.jumpForce);

            return true;
        } else if (hasAirJump && jumpData.canAirJump && !surroundings.IsCoyote())
        {
            staminaManager.currentStamina -= jumpData.staminaCost;

            // Air Jump
            AirJump(jumpData.jumpForce);

            return true;
        }

        return false;
    }

    public bool TryRollJump()
    {
        if (surroundings.IsGrounded() && !surroundings.IsSlope())
        {
            // Roll Jump
            RollJump();
            wasRollJumping = true;
            wasRollJumpingDebug = true;

            return true;
        }

        return false;
    }

    public bool TryCrouchJump()
    {
        if (!surroundings.isSomethingAboveHead)
        {
            if (surroundings.IsGrounded() && !surroundings.IsSlope())
            {
                // Roll Jump
                CrouchJump();

                return true;
            }

        }
        return false;
    }

    // _________________________________ Jumping
    public void RollJump()
    {
        if (true) // movementGroundTime.Coyote()
        {
            essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
            state.SetActionState("rolljump", 0.3f);

            SoundData sound = new SoundData(jumpData.jumpSound, 1f);
            if (sound != null)
            {
                actionSound.PlaySound(sound);
            }

            actionAnimation.PlayAnimation("jump");
            SetAirJump(false);


            // wantJumpTime = 0f;
            essentials.rigidbody.AddForce(Vector3.up * jumpData.forwardUpForce, ForceMode.Impulse);
            essentials.rigidbody.AddForce(essentials.rigidbody.transform.forward * jumpData.forwardJumpForce, ForceMode.Impulse);
            SetOffGround();

            wasRollJumping = true;
            wasRollJumpingDebug = true;
            // heightJumpTime = heightJumpDuration;
        }
    }

    public void CrouchJump()
    {
        if (true) // movementGroundTime.Coyote()
        {
            essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
            state.SetActionState("crouchjump", 0.6f);

            SoundData sound = new SoundData(jumpData.jumpSound, 1f);
            if (sound != null)
            {
                actionSound.PlaySound(sound);
            }
            actionAnimation.PlayAnimation("frontflip");

            // wantJumpTime = 0f;
            essentials.rigidbody.AddForce(Vector3.up * jumpData.crouchJumpForce, ForceMode.Impulse);

            // airJump = false;
            // wantJumpTime = 0f;
            // Jump(rb, crouchJumpForce);
            SetOffGround();
            SetAirJump(false);
        }
    }

    public void HeightJump()
    {
        if (true) // movementGroundTime.Coyote()
        {
            essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
            state.SetActionState("crouchjump", 0.6f);

            SoundData sound = new SoundData(jumpData.jumpSound, 1f);
            if (sound != null)
            {
                actionSound.PlaySound(sound);
            }
            actionAnimation.PlayAnimation("frontflip");

            // wantJumpTime = 0f;
            essentials.rigidbody.AddForce(Vector3.up * jumpData.crouchJumpForce, ForceMode.Impulse);

            // airJump = false;
            // wantJumpTime = 0f;
            // Jump(rb, crouchJumpForce);
            SetOffGround();
            SetAirJump(false);
        }
    }

    // Simple Jump
    public void Jump(float jumpForce)
    {
        SetOffGround();

        essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
        Vector3 newVelo = essentials.rigidbody.linearVelocity;
        newVelo.y = 0f;
        newVelo = newVelo - newVelo * jumpData.jumpRedirectSpeed;
        essentials.rigidbody.linearVelocity = newVelo;
        Vector3 currentVelocity = essentials.rigidbody.linearVelocity;
        if (currentVelocity != Vector3.zero)
        {
            // float limitingFactor = currentVelocity.magnitude;
            // currentVelocity /= limitingFactor;
            // if (limitingFactor > jumpData.limitJumpHorizontalSpeed)
            // {
            //     limitingFactor = jumpData.limitJumpHorizontalSpeed;
            // }
            // currentVelocity = currentVelocity * limitingFactor;
            // essentials.rigidbody.linearVelocity = currentVelocity;
        }
        essentials.rigidbody.AddForce(essentials.rigidbody.transform.up * jumpForce, ForceMode.Impulse);

        state.SetActionState("jump", jumpData.jumpDuration);
        SoundData sound = new SoundData(jumpData.jumpSound, 1f);
        if (sound != null)
        {
            actionSound.PlaySound(sound);
        }
        actionAnimation.PlayAnimation("jump");
    }

    // AirJump
    public void AirJump(float jumpForce)
    {
        SetOffGround();
        hasAirJump = false;

        essentials.rigidbody.linearVelocity = new Vector3(essentials.rigidbody.linearVelocity.x, 0f, essentials.rigidbody.linearVelocity.z);
        Vector3 newVelo = essentials.rigidbody.linearVelocity;
        newVelo.y = 0f;
        newVelo = newVelo - newVelo * jumpData.jumpRedirectSpeed;
        essentials.rigidbody.linearVelocity = newVelo;
        Vector3 currentVelocity = essentials.rigidbody.linearVelocity;
        if (currentVelocity != Vector3.zero)
        {
            // float limitingFactor = currentVelocity.magnitude;
            // currentVelocity /= limitingFactor;
            // if (limitingFactor > jumpData.limitJumpHorizontalSpeed)
            // {
            //     limitingFactor = jumpData.limitJumpHorizontalSpeed;
            // }
            // currentVelocity = currentVelocity * limitingFactor;
            // essentials.rigidbody.linearVelocity = currentVelocity;
        }
        essentials.rigidbody.AddForce(essentials.rigidbody.transform.up * jumpForce, ForceMode.Impulse);

        state.SetActionState("jump", jumpData.jumpDuration);
        SoundData sound = new SoundData(jumpData.jumpSound, 1f);
        if (sound != null)
        {
            actionSound.PlaySound(sound);
        }
        actionAnimation.PlayAnimation("frontflip");
    }
}