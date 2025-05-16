using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class ActionAnimation
{
    [Header("Animation Component")]
    public UnityEvent<string> animatorTrigger;

    public void PlayAnimation(string id)
    {
        animatorTrigger.Invoke(id);
    }
}
