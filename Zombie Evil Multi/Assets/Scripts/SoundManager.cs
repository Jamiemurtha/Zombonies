using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource soundFX;

    void Awake()
    {
        instance = this;
    }


    void Update()
    {
        
    }

    public void PlaySoundFX(AudioClip clip)
    {
        soundFX.clip = clip;
        soundFX.Play();
    }
}
