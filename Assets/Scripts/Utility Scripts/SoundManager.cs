using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public float musicVolume = 1, soundEffectVolume = 1;
    public List<AudioClip> audioClips;
    public AudioClip creationClip, correctGuessClip, wrongGuessClip;

    public GameObject playSound, muteSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //If there is no key
        if(PlayerPrefs.HasKey("PlayEffects") == false)
        {
            PlayerPrefs.SetInt("PlayEffects", 1);
        }
        //If we are supposed to play sound
        if (PlayerPrefs.GetInt("PlayEffects") == 1)
        {
            //Play sound
            audioSource.volume = 1;
            //Show the playing sprite
            playSound.SetActive(true);
            muteSound.SetActive(false);
        }
        else
        {
            //Stop playing
            audioSource.volume = 0;
            //Show the muted sprite
            playSound.SetActive(false);
            muteSound.SetActive(true);
        }
    }

    public void SetPitch(float newPitch)
    {
        audioSource.pitch = newPitch;
    }
    public void ToggleEffects()
    {
        //If we're playing
        if (PlayerPrefs.GetInt("PlayEffects") == 1)
        {
            //Stop playing
            audioSource.volume = 0;
            //Show the muted sprite
            playSound.SetActive(false);
            muteSound.SetActive(true);
            //Save state
            PlayerPrefs.SetInt("PlayEffects", 0);
        }
        else
        {
            //Play sound
            audioSource.volume = 1;
            //Show the playing sprite
            playSound.SetActive(true);
            muteSound.SetActive(false);
            //Save state
            PlayerPrefs.SetInt("PlayEffects", 1);
        }
    }
}
