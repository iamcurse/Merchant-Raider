using UnityEngine;

public class FirstGarden : MonoBehaviour
{
[SerializeField]
    private AudioClip theme, previousTheme;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            previousTheme = BackgroundMusic.instance.GetTrack();
            BackgroundMusic.instance.ChangeTrack(theme);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BackgroundMusic.instance.ChangeTrack(previousTheme);
        }
    }
}
