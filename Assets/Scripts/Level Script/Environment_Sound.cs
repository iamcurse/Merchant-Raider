using UnityEngine;

public class Environment_Sound : MonoBehaviour
{
    public AudioClip[] doorSounds;
    public AudioClip[] trapSounds;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void PlayDoor()
    {
        AudioClip clip = doorSounds[(int)Random.Range(0, doorSounds.Length)];
        source.clip = clip;
        source.Play();
        Debug.Log(clip.name);
    }

    void PlayTrap()
    {
        AudioClip clip = trapSounds[(int)Random.Range(0, trapSounds.Length)];
        source.clip = clip;
        source.Play();
        Debug.Log(clip.name);
    }
}
