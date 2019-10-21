using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    protected static SoundManager _Instance;

    public static SoundManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<SoundManager>();
            }
            return _Instance;
        }
    }

    public List<AudioSource> sources = new List<AudioSource>();

    public bool cutMusic;
    public bool cutSFX;
    float musicVolume;
    float cardVolume;
    float sfxVolume;

    [Header("Music")]
    public AudioClip musicMenuLoop;
    public AudioClip musicStart;
    public AudioClip musicLoop;
    [Header("Cards")]
    public AudioClip click;
    public AudioClip hover;
    public AudioClip select;
    public AudioClip spawn;

    [Header("SFX")]
    public AudioClip sfx_sword;
    public AudioClip sfx_bomb;
    public AudioClip sfx_shield;
    public AudioClip sfx_bolt;
    public AudioClip sfx_heal;
    public AudioClip sfx_consume;
    public AudioClip sfx_unlock;
    public AudioClip sfx_arrow;


    public void Start()
    {
        musicVolume = sources[0].volume;
        cardVolume = sources[1].volume;
        sfxVolume = sources[2].volume;

        StartMenuMusic();
    }

    public void Update()
    {
        if (!sources[0].isPlaying && sources[0].clip == musicStart)
        {
            sources[0].loop = true;
            sources[0].clip = musicLoop;
            sources[0].Play();
        }
    }

    public void StartMainMusic()
    {
        sources[0].loop = false;
        sources[0].clip = musicStart;
        sources[0].Play();
    }

    public void StartMenuMusic()
    {
        sources[0].loop = true;
        sources[0].clip = musicMenuLoop;
        sources[0].Play();
    }

    public void CutMusic()
    {
 
        cutMusic = true;
        sources[0].volume = 0;

    }

    public void PlayMusic()
    {
        cutMusic = false;
        sources[0].volume = musicVolume;
    }
    public void CutSFX()
    {

        cutSFX = true;
        sources[1].volume = 0;
        sources[2].volume = 0;

    }

    public void PlaySFX()
    {
        cutSFX = false;
        sources[1].volume = cardVolume;
        sources[2].volume = sfxVolume;
    }


    public void PlaySound(int s, AudioClip clip)
    {
        sources[s].PlayOneShot(clip);
    }
    public void PlaySound(int s, AudioClip clip, float volume)
    {
        sources[s].PlayOneShot(clip, volume);
    }

    public IEnumerator PlayTwoSounds(int s, AudioClip clip1, AudioClip clip2, float delay)
    {
        sources[s].PlayOneShot(clip1);
        yield return new WaitForSeconds(delay);
        sources[s].PlayOneShot(clip2);
    }

    public IEnumerator PlayThreeSounds(int s, AudioClip clip1, AudioClip clip2, AudioClip clip3, float delay)
    {
        sources[s].PlayOneShot(clip1);
        yield return new WaitForSeconds(delay);
        sources[s].PlayOneShot(clip2);
        yield return new WaitForSeconds(delay);
        sources[s].PlayOneShot(clip3);
    }

        public IEnumerator PlaySpreadSounds(List<AudioClip> clips, float delay)
    {
        for (int i = 0; i<clips.Count; i++)
        {
            yield return new WaitForSeconds(1f);

        }
    }

}
