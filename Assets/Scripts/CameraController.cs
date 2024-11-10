using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject 
        _camera;

    private void Awake()
    {
        _camera = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            _camera.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            _camera.SetActive(false);
        }
    }
}
