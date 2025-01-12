using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioClip[] swingSounds;
    public AudioClip[] shootSounds;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void PlaySwordSwing()
    {
        AudioClip clip = swingSounds[(int)Random.Range(0, swingSounds.Length)];
        source.clip = clip;
        source.Play();
        Debug.Log(clip.name);
    }

    void PlayBowShoot()
    {
        AudioClip clip = shootSounds[(int)Random.Range(0, shootSounds.Length)];
        source.clip = clip;
        source.Play();
        Debug.Log(clip.name);
    }


}
