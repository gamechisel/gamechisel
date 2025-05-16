using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionDamageData
{
    public float swimForce = 1.5f;
    public float waterLevelForce = 30f;
    public float waterDampingForce = 10f;
}

[System.Serializable]
public class ActionDamage
{
    public ActionSwimData data;
    public Essentials essentials;
    public ActionAnimation actionAnimation;
    public ActionSound actionSound;
    public PlayerActionState actionState;

    public void Init(Essentials _essentials, ActionAnimation _actionAnimation, ActionSound _actionSound, PlayerActionState _actionState)
    {
        essentials = _essentials;
        actionAnimation = _actionAnimation;
        actionSound = _actionSound;
        actionState = _actionState;
    }

    public void WaterFloating(float waterLevel)
    {
        float rayDirVel = Vector3.Dot(-essentials.rigidbody.transform.up, essentials.rigidbody.linearVelocity);
        float offset = waterLevel * -1f;
        float springForce = (offset * data.waterLevelForce) - (rayDirVel * data.waterDampingForce);
        essentials.rigidbody.AddForce(-essentials.rigidbody.transform.up * springForce);
    }

    public void WaterMovement(Vector3 moveDirection, bool waterUpButton, bool waterDownButton)
    {
        float swimForce = data.swimForce;

        if (waterUpButton && !waterDownButton) // !onWaterSurface
        {
            moveDirection += essentials.rigidbody.transform.up * 1f;
        }
        if (waterDownButton && !waterUpButton)
        {
            moveDirection -= essentials.rigidbody.transform.up * 1f;
        }
        moveDirection = Vector3.Normalize(moveDirection) * moveDirection.magnitude;
        essentials.rigidbody.AddForce(moveDirection * swimForce, ForceMode.Acceleration);
    }
}