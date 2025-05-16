using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ColliderState
{
    public string colliderId = "";
    public GameObject collider;
}

[System.Serializable]
public class PlayerActionState
{
    [Header("State")]
    public string currentState = "none";
    public float actionTime = 0f;

    [Header("Collider")]
    public List<ColliderState> colliders = new List<ColliderState>();
    private string activeColliderId = "regular";

    public void SetActionState(string _newState, float _newTime)
    {
        currentState = _newState;
        actionTime = _newTime;

        if (currentState == "crouch" || currentState == "roll")
        {
            ActivateCollider("crouch");
        }
        else
        {
            ActivateCollider("regular");
        }
    }

    public void ClearState()
    {
        currentState = "none";
        actionTime = 0f;
    }

    public bool IsActionState(string checkActionState)
    {
        return currentState == checkActionState;
    }

    public void FU_State()
    {
        // Update Action State
        if (IsActionState("none"))
        {
            return;
        }

        // ActionState lastState = actionState;
        actionTime -= Time.deltaTime;
        if (actionTime <= 0f)
        {
            actionTime = 0f;
            currentState = "none";
        }
    }

    public void ActivateCollider(string _colliderId)
    {
        activeColliderId = _colliderId;
        foreach (ColliderState cs in colliders)
        {
            if (cs.colliderId == _colliderId)
            {
                cs.collider.SetActive(true); // Activate specific collider
            }
            else
            {
                cs.collider.SetActive(false); // Deactivate other colliders
            }
        }
    }
}
