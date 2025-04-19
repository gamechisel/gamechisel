using UnityEngine;

public class SoundData
{
    public AudioClip clip;
    public float volume;

    public SoundData (AudioClip _clip, float _volume)
    {
        clip = _clip;
        volume = _volume;
    }
}
