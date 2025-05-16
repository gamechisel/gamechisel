using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class ActionSound
{
    [Header("Sound Component")]
    public UnityEvent<SoundData> audioTrigger;

    public void PlaySound(SoundData soundData)
    {
        if (soundData != null)
        {
            audioTrigger.Invoke(soundData);
        }
    }
}