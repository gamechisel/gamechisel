using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionAttackData
{
    [Header("Light Attack Data")]
    public float lightComboResetTime = 1.0f;
    public string lightAttack1Animation = "light_attack1";
    public float light_attack1Duration = 0.3f;
    public AudioClip lightAttack1Sound = null;
    public string lightAttack2Animation = "light_attack2";
    public float light_attack2Duration = 0.3f;
    public AudioClip lightAttack2Sound = null;
    public string lightAttack3Animation = "light_attack3";
    public float light_attack3Duration = 0.5f;
    public AudioClip lightAttack3Sound = null;

    [Header("Heavy Attack Data")]
    public float heavyChargeMinTime = 0.8f; // Minimum charge time
    public float heavyChargePerfectStart = 1.6f; // Perfect charge window start
    public float heavyChargeMaxTime = 2.0f; // Maximum charge time
    public string heavyAttackAnimation = "heavy_attack";
    public string perfectHeavyAttackAnimation = "perfect_heavy_attack";
    public AudioClip heavyAttackSound = null;
    public AudioClip perfectHeavyAttackSound = null;
}

[System.Serializable]
public class ActionAttack
{
    public ActionAttackData swordAttackData;

    public int setComboIndex = 3;
    private Essentials essentials;
    private ActionSound actionSound;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;
    private PlayerActionController playerActionController;

    private int lightComboIndex = 0;
    private float lastLightAttackTime;
    private bool isChargingHeavy = false;
    private float heavyChargeTime = 0f;

    public void Init(ActionSound _actionSound, ActionAnimation _actionAnimation, PlayerActionState _state, Essentials _essentials, PlayerActionController _playerActionController)
    {
        actionSound = _actionSound;
        actionAnimation = _actionAnimation;
        state = _state;
        essentials = _essentials;
        playerActionController = _playerActionController;
    }

    // Light Attack Logic
    public void LightAttack(Vector3 _forward)
    {
        if (essentials == null || actionAnimation == null || state == null) return;

        // Check if the combo time window has elapsed
        if (Time.time - lastLightAttackTime > swordAttackData.lightComboResetTime)
        {
            lightComboIndex = 0; // Reset combo if the time has passed
        }

        // Increment combo stage
        if (setComboIndex == 3)
        {
            lightComboIndex = (lightComboIndex % 3) + 1;
        }
        else if (setComboIndex == 2)
        {
            lightComboIndex = (lightComboIndex % 2) + 1;
        }
        else
        {
            lightComboIndex = 1;
        }

        // Play the corresponding animation and sound
        string animationName;
        AudioClip attackSound;
        float attackDuration;

        playerActionController.UseSwordItem(true);

        switch (lightComboIndex)
        {
            case 1:
                animationName = swordAttackData.lightAttack1Animation;
                attackSound = swordAttackData.lightAttack1Sound;
                attackDuration = swordAttackData.light_attack1Duration;
                break;
            case 2:
                animationName = swordAttackData.lightAttack2Animation;
                attackSound = swordAttackData.lightAttack2Sound;
                attackDuration = swordAttackData.light_attack2Duration;
                break;
            case 3:
                animationName = swordAttackData.lightAttack3Animation;
                attackSound = swordAttackData.lightAttack3Sound;
                attackDuration = swordAttackData.light_attack3Duration;
                break;
            default:
                animationName = swordAttackData.lightAttack1Animation;
                attackSound = swordAttackData.lightAttack1Sound;
                attackDuration = swordAttackData.light_attack1Duration;
                break;
        }

        actionAnimation.PlayAnimation(animationName);

        state.SetActionState(animationName, attackDuration);
        essentials.rigidbody.linearVelocity = Vector3.zero; // Reset velocity
        essentials.rigidbody.AddForce(_forward * attackDuration, ForceMode.Impulse);

        if (attackSound != null)
        {
            actionSound.PlaySound(new SoundData(attackSound, 1f));
        }

        // Update the last attack time
        lastLightAttackTime = Time.time;
    }

    public void UpdateHeavyCharging(bool _wantHeavyAttack)
    {
        if (isChargingHeavy)
        {
            UpdateHeavyCharge();
        }

        if (isChargingHeavy && !_wantHeavyAttack)
        {
            CancelHeavyAttack();
            PerformHeavyAttack(_wantHeavyAttack);
        }
    }

    public void TryHeavyAttack(bool _wantHeavyAttack)
    {
        if (_wantHeavyAttack && !isChargingHeavy)
        {
            StartHeavyAttack();
        }
    }

    // Heavy Attack Logic
    public void StartHeavyAttack()
    {
        if (isChargingHeavy) return; // Prevent multiple charge starts
        isChargingHeavy = true;
        heavyChargeTime = 0f;
        state.SetActionState("heavyloading", 2f);
    }

    public void CancelHeavyAttack()
    {
        if (!isChargingHeavy) return; // Prevent canceling when not charging
        isChargingHeavy = false;
        heavyChargeTime = 0f;
    }

    public void PerformHeavyAttack(bool isManual)
    {
        string animationName;
        AudioClip sound;

        // Determine if it's a perfect heavy attack
        if (isManual && heavyChargeTime >= swordAttackData.heavyChargePerfectStart && heavyChargeTime <= swordAttackData.heavyChargeMaxTime)
        {
            animationName = swordAttackData.perfectHeavyAttackAnimation;
            sound = swordAttackData.perfectHeavyAttackSound;
        }
        else if (heavyChargeTime >= swordAttackData.heavyChargeMinTime)
        {
            animationName = swordAttackData.heavyAttackAnimation;
            sound = swordAttackData.heavyAttackSound;
        }
        else
        {
            CancelHeavyAttack();
            return;
        }

        // Play the corresponding animation and sound
        actionAnimation.PlayAnimation(animationName);
        state.SetActionState("heavyattack", 0.5f);

        if (sound != null)
        {
            actionSound.PlaySound(new SoundData(sound, 1f));
        }

        // Reset charge time
        heavyChargeTime = 0f;
        isChargingHeavy = false;
    }

    // Updates the heavy charge time
    private void UpdateHeavyCharge()
    {
        if (isChargingHeavy)
        {
            heavyChargeTime += Time.deltaTime;

            // If charge time exceeds max, trigger automatic heavy attack
            if (heavyChargeTime >= swordAttackData.heavyChargeMaxTime)
            {
                PerformHeavyAttack(false); // Automatic heavy attack (not perfect)
                isChargingHeavy = false;
                heavyChargeTime = 0f;
            }
        }
    }
}
