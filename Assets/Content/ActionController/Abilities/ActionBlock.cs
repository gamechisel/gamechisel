using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionBlockData
{
    [Header("Block Data")]
    public float blockDuration = 3.0f; // Maximum duration to hold block
    public AudioClip blockSound = null; // Sound for blocking
    public AudioClip counterSound = null; // Sound for counter
}

[System.Serializable]
public class ActionBlock
{
    public ActionBlockData actionBlockData;

    private Essentials essentials;
    private ActionSound actionSound;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;

    private bool isBlocking = false;
    private bool isCountering = false;
    private float blockStartTime = 0f;

    public void Init(Essentials _essentials, ActionSound _actionSound, ActionAnimation _actionAnimation, PlayerActionState _state)
    {
        essentials = _essentials;
        actionSound = _actionSound;
        actionAnimation = _actionAnimation;
        state = _state;
    }

    public void StartBlock()
    {
        // isBlocking = true;
        // blockStartTime = Time.time;

        // Play block animation and sound
        // actionAnimation.PlayAnimation("blocking");
        // if (actionBlockData.blockSound != null)
        // {
        //     actionSound.PlaySound(new SoundData(actionBlockData.blockSound, 1f));
        // }

        state.SetActionState("blocking", actionBlockData.blockDuration);
    }

    public void StopBlock()
    {
        if (!isBlocking) return; // Prevent stopping block if not active

        isBlocking = false;
        blockStartTime = 0f;
    }

    public void TryCounter()
    {
        if (!isBlocking) return; // Counter only if currently blocking

        // Counter action triggered by a click (not holding)
        isCountering = true;
    }

    public void OnEnemyHit()
    {
        if (!isBlocking) return; // No block to register hit

        if (isCountering)
        {
            TriggerCounter();
        }
        else
        {
            // Blocking without counter
            essentials.rigidbody.linearVelocity = Vector3.zero; // Absorb the hit
            Debug.Log("Enemy hit absorbed while blocking.");
        }
    }

    private void TriggerCounter()
    {
        isCountering = false; // Reset countering state

        // Play counter animation and sound
        actionAnimation.PlayAnimation("counter");
        if (actionBlockData.counterSound != null)
        {
            actionSound.PlaySound(new SoundData(actionBlockData.counterSound, 1f));
        }

        state.SetActionState("counter", 0.5f); // Set state for the duration of counter animation
        Debug.Log("Counter triggered!");
    }

    private void Update()
    {
        // Auto-stop block if holding longer than allowed duration
        if (isBlocking && Time.time - blockStartTime > actionBlockData.blockDuration)
        {
            StopBlock();
        }
    }
}
