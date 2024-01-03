using System;
using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour
{
    public bool playOnStart = false; // Boolean to determine if the sound should be played on Start
    SoundBank soundBank;

    private void Start()
    {
        soundBank = GameObject.Find("SoundBank").GetComponent<SoundBank>(); // Find the SoundBank in the scene

        
        if (playOnStart)
        {

            //PlaySoundFromBank("Walking");
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
            soundBank = GameObject.Find("SoundBank").GetComponent<SoundBank>(); // Find the SoundBank in the scene

        }
    }

    //stop sound

  public void StopSoundFromBank(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
        {
            Debug.LogWarning("Sound name is empty. Please assign a sound name.");
            return;
        }

        if (soundBank != null)
        {
            StartCoroutine(FadeOutAndStop(soundName));
        }
        else
        {
            soundBank = GameObject.Find("SoundBank").GetComponent<SoundBank>(); // Find the SoundBank in the scene
            Debug.LogWarning("SoundBank script not found in the scene. Make sure it's attached to an object.");
        }
    }

    IEnumerator FadeOutAndStop(string soundName)
    {
        Sound soundToFade = soundBank.sounds.Find(sound => sound.name == soundName);
        if (soundToFade != null)
        {
            AudioSource source = soundToFade.source;
            float startVolume = source.volume;

            while (source.volume > 0)
            {
                source.volume -= startVolume * Time.deltaTime / .01f; // Adjust the duration of fade-out here
                yield return null;
            }

            soundBank.StopSound(soundName);
            source.volume = startVolume; // Reset volume to initial value

        }
        else
        {
            Debug.LogWarning("Sound with name '" + soundName + "' not found in the sound bank.");
        }
    }
}
