using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionPunchData
{
    [Header("Punch Data")]
    public float punchForce = 5f;
    public float punchDuration = 0.3f;
    public AudioClip punchSound = null;
    public string punchAnimationName = "punch";
}

[System.Serializable]
public class ActionPunch
{
    public ActionPunchData actionPunchData;
    private Essentials essentials;
    private ActionSound actionSound;
    private ActionAnimation actionAnimation;
    private PlayerActionState state;

    public void Init(Essentials _essentials, ActionSound _actionSound, ActionAnimation _actionAnimation, PlayerActionState _state)
    {
        essentials = _essentials;
        actionSound = _actionSound;
        actionAnimation = _actionAnimation;
        state = _state;
    }

    public void SetValues(ActionPunchData _actionPunchData)
    {
        actionPunchData = _actionPunchData;
    }

    public void WantPunch()
    {
        Punch();
    }

    private void Punch()
    {
        // Set the character state to Punch.
        state.SetActionState("punch", actionPunchData.punchDuration);

        // Play the punch sound if available.
        if (actionPunchData.punchSound != null)
        {
            SoundData sound = new SoundData(actionPunchData.punchSound, 1f);
            actionSound.PlaySound(sound);
        }

        // Play the punch animation.
        if (!string.IsNullOrEmpty(actionPunchData.punchAnimationName))
        {
            actionAnimation.PlayAnimation(actionPunchData.punchAnimationName);
        }
    }
}
