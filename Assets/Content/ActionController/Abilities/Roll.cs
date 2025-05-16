using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionRollData
{
    [Header("Roll Data")]
    public float rollForce = 6f;
    public float rollDuration = 0.6f;
    public float rollChangeDirectionForce = 1f;
    public AudioClip rollSound = null;
    public float staminaCost;
}

[System.Serializable]
public class ActionRoll
{
    public ActionRollData actionRollData;
    private Essentials essentials;
    private Surroundings surroundings;
    private ActionSound actionSound;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;

    public void Init(Essentials _essentials, ActionSound _actionSound, ActionAnimation _actionAnimation, Surroundings _surroundings, PlayerActionState _state, ActionRotation _rotation)
    {
        essentials = _essentials;
        actionSound = _actionSound;
        actionAnimation = _actionAnimation;
        surroundings = _surroundings;
        state = _state;
    }

    public void SetValues(ActionRollData _actionRollData)
    {
        actionRollData = _actionRollData;
    }

    public void WantRoll(Vector3 wantDir, Vector3 velDir)
    {
        Roll(wantDir, velDir);
    }

    public void Roll(Vector3 wantDir, Vector3 velDir)
    {
        Vector3 rollDirection = essentials.rigidbody.transform.forward;
        state.SetActionState("roll", actionRollData.rollDuration);

        SoundData sound = new SoundData(actionRollData.rollSound, 1f);
        if (sound != null)
        {
            actionSound.PlaySound(sound);
        }
        actionAnimation.PlayAnimation("roll");

        essentials.rigidbody.linearVelocity = Vector3.zero;
        essentials.rigidbody.AddForce(rollDirection * actionRollData.rollForce, ForceMode.Impulse);
    }
}