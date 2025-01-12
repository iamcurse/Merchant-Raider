using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;
    private AudioSource source;
    public List<AudioClip> musicClips;
    private int currentClipIndex;
    public bool playSequentially = false; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicClips == null || musicClips.Count == 0)
        {
            Debug.LogError("No music clips assigned!");
            return;
        }

        source = GetComponent<AudioSource>();
        source.volume = 0f;
        PlayNextClip();
    }

    private void Update()
    {
        if (!source.isPlaying)
        {
            PlayNextClip();
        }
    }

    private void PlayNextClip()
    {
        if (playSequentially)
        {
            currentClipIndex = (currentClipIndex + 1) % musicClips.Count; 
        }
        else
        {
            currentClipIndex = Random.Range(0, musicClips.Count); 
        }

        source.clip = musicClips[currentClipIndex];
        source.Play();
        StartCoroutine(Fade(source, 2f, 1f));
    }

    public IEnumerator Fade(AudioSource source, float duration, float targetVolume)
    {
        float startVol = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void SetVolume(float volume)
    {
        source.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void PauseMusic()
    {
        if (source.isPlaying)
        {
            source.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!source.isPlaying)
        {
            source.UnPause();
        }
    }

    public void ChangeTrack(AudioClip clip)
    {
        StopAllCoroutines();
        source.clip = clip;
        source.volume = 0f;
        StartCoroutine(Fade(source, 2f, 1f));
    }

    public AudioClip GetTrack()
    {
        return source.clip;
    }
}
