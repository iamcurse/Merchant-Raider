using UnityEngine;

public class MonsterSounds : MonoBehaviour
{

    public AudioClip[] meatSlashSounds;
    public AudioClip[] deathSounds;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void PlayCut()
    {
        AudioClip clip = meatSlashSounds[(int)Random.Range(0, meatSlashSounds.Length)];
        source.clip = clip;
        source.Play();
        Debug.Log(clip.name);
    }

    void PlayDeath()
    {
        AudioClip clip = deathSounds[(int)Random.Range(0, deathSounds.Length)];
        source.clip = clip;
        source.Play();
        Debug.Log(clip.name);
    }
}
