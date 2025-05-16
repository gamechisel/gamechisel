using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionSlideData
{
    [Header("Slide Data")]
    public float slideForce = 8f;
    public float slideDuration = 0.8f;
    public AudioClip slideSound = null;
    public string slideAnimationName = "slide";
}

[System.Serializable]
public class ActionSlide
{
    public ActionSlideData actionSlideData;
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

    public void SetValues(ActionSlideData _actionSlideData)
    {
        actionSlideData = _actionSlideData;
    }

    public void WantSlide(Vector3 slideDirection)
    {
        Slide(slideDirection);
    }

    private void Slide(Vector3 slideDirection)
    {
        // Ensure the slide can only happen if the character is in a valid state.
        // if (!state.CanPerformAction(ActionState.Slide))
        // {
        return;
        // }

        // Normalize the slide direction.
        slideDirection = slideDirection.normalized;

        // Set the character state to Slide.
        state.SetActionState("slide", actionSlideData.slideDuration);

        // Play the slide sound if available.
        if (actionSlideData.slideSound != null)
        {
            SoundData sound = new SoundData(actionSlideData.slideSound, 1f);
            actionSound.PlaySound(sound);
        }

        actionAnimation.PlayAnimation("slide");

        // Apply the sliding force.
        essentials.rigidbody.linearVelocity = Vector3.zero; // Clear any existing velocity.
        essentials.rigidbody.AddForce(slideDirection * actionSlideData.slideForce, ForceMode.Impulse);
    }
}
