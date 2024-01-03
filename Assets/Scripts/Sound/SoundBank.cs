using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float minPitch = 1f; // Minimum pitch variation
    [Range(0.1f, 3f)]
    public float maxPitch = 1f; // Maximum pitch variation
    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}

public class SoundBank : MonoBehaviour
{
    public static SoundBank instance;

    public List<Sound> sounds = new List<Sound>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
    }

    public void PlaySound(string soundName)
    {
        Sound soundToPlay = sounds.Find(sound => sound.name == soundName);
        if (soundToPlay != null)
        {
            Debug.Log("Playing sound: " + soundName);

            soundToPlay.source.pitch = UnityEngine.Random.Range(soundToPlay.minPitch, soundToPlay.maxPitch);
            soundToPlay.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound with name '" + soundName + "' not found in the sound bank.");
        }
    }

    public void StopSound(string soundName)
    {
        Sound soundToStop = sounds.Find(sound => sound.name == soundName);
        if (soundToStop != null)
        {
            soundToStop.source.Stop();
        }
    }
}
