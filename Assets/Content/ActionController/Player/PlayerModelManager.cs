using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class PlayerModelManager : MonoBehaviour
{
    [Header("Model Data")]
    public Transform model;

    [Header("Animator")]
    public Animator animator;

    [Header("Rig-Builder")]
    [SerializeField] private Rig aimRig;
    [SerializeField] private GameObject aimRotationRigTarget;
    [SerializeField] private Rig rightFootRig;
    [SerializeField] private GameObject rightFootTarget;
    [SerializeField] private Rig leftFootRig;
    [SerializeField] private GameObject leftFootTarget;
    private float upperBodyWeight = 0f;
    private float footLeftWeight = 0f;
    private float footRightWeight = 0f;

    [Header("Audio")]
    public AudioSource audioSource;

    // [Header("Partciles")]
    // placeholder

    public void SetTransform(Vector3 position, Vector3 eulerAngles)
    {
        model.position = position;
        model.eulerAngles = eulerAngles;
    }

    public void PlaySound(SoundData soundData)
    {
        if (soundData != null)
        {
            audioSource.PlayOneShot(soundData.clip, soundData.volume);
        }
    }

    public void PlayAnimation(string anim)
    {
        if (animator != null)
        {   
            animator.SetTrigger(anim);
        }
    }

    public void UpdateValues(bool action, bool upperBody, bool grounded, bool crouch, bool blocking, float horizontalVelocity, float verticalVelocity, float sideVelocity, float forwardVelocity, Vector3 rotationTargetPosition, bool swimming, bool climbing, bool ledge)
    {
        float weight = 0f;
        float upperBodyWeight = 0f;
        if (blocking)
        {
            upperBody = true;
            upperBodyWeight = 1f;
        }
        animator.SetLayerWeight(animator.GetLayerIndex("UpperBody"), upperBodyWeight);

        if (blocking)
        {
            aimRig.weight = 1f;
        }
        else
        {
            aimRig.weight = 0f;
        }
        SetUpperRotationTarget(rotationTargetPosition);

        animator.SetBool("action", action);
        animator.SetBool("swimming", swimming);
        animator.SetBool("ledge", ledge);
        animator.SetBool("climbing", climbing);
        animator.SetBool("grounded", grounded);
        animator.SetBool("crouch", crouch);
        animator.SetBool("blocking", blocking);
        animator.SetFloat("horizontalVelocity", horizontalVelocity);
        animator.SetFloat("verticalVelocity", verticalVelocity);
        animator.SetFloat("forwardVelocity", forwardVelocity);
        animator.SetFloat("sideVelocity", sideVelocity);
    }

    public void SetUpperRotationTarget(Vector3 targetPosition)
    {
        // limit rotation
        aimRotationRigTarget.transform.position = targetPosition;
    }

    public void UpdateFootPlacement()
    {
        // errors lol
    }

    public void SetState(bool state)
    {
        model.gameObject.SetActive(state);
    }
}
