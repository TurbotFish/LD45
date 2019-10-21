using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAnim : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    public float volume;

    public void PlaySound()
    {
        source.PlayOneShot(clip, volume);
    }
}
