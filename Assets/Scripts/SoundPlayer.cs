using System;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public bool playOnStart = false; // Boolean to determine if the sound should be played on Start
    SoundBank soundBank;

    private void Start()
    {
        
        if (playOnStart)
        {
            soundBank = FindObjectOfType<SoundBank>();

            PlaySoundFromBank("Bow");
        }
    }

    public void PlaySoundFromBank(String soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            Debug.LogWarning("Sound name is empty. Please assign a sound name.");
            return;
        }

        if (soundBank != null)
        {
            soundBank.PlaySound(soundName);
        }
        else
        {
            Debug.LogWarning("SoundBank script not found in the scene. Make sure it's attached to an object.");
        }
    }
}
