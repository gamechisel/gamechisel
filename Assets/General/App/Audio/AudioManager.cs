using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Singleton Instance
    public static AudioManager Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [Header("Music")]
    [SerializeField] private string musicID;
    [SerializeField] private bool musicPlaying;
    [SerializeField] private bool looping;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        HandleLoopingMusic();
    }

    public void UpdateSettings(AudioSettings audioSettings)
    {
        audioMixer.SetFloat("volumeMaster", audioSettings.masterVolume);
        audioMixer.SetFloat("volumeSound", audioSettings.soundVolume);
        audioMixer.SetFloat("volumeMusic", audioSettings.musicVolume);
        audioMixer.SetFloat("volumeUI", audioSettings.uiVolume);
    }

    public void LoadPlayUI(string id)
    {
        AudioClip clip = ResourceSystem.GetSoundUI(id);
        if (clip)
        {
            PlayUI(clip);
        }
        else
        {
            Debug.LogError("UI sound clip not found: " + id);
        }
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip)
        {
            uiSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("UI sound clip not given.");
        }
    }

    public void LoadPlaySound(string id)
    {
        AudioClip clip = ResourceSystem.GetSound(id);
        if (clip)
        {
            PlaySound(clip);
        }
        else
        {
            Debug.LogError("Sound clip not found: " + id);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip)
        {
            soundSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Sound clip not given.");
        }
    }

    public void PlayMusic(string id, bool loop, bool force)
    {
        if (!musicPlaying || musicPlaying && force)
        {
            if (musicID != id)
            {
                musicSource.Stop();
                musicID = id;
                AudioClip clip = ResourceSystem.GetMusic(id);
                musicSource.clip = clip;
                musicSource.Play();
            }
            looping = loop;
            musicPlaying = true;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicID = "";
        looping = false;
        musicPlaying = false;
    }

    public void HandleLoopingMusic()
    {
        if (musicPlaying && !musicSource.isPlaying)
        {
            if (looping)
            {
                musicSource.Play();
            }
            else
            {
                StopMusic();
            }
        }
    }
}
