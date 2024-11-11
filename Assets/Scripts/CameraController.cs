using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _cameraObject;
    private Transform _player;
    private CinemachineCamera _camera;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _cameraObject = transform.GetChild(0).gameObject;
        _camera = transform.GetChild(0).gameObject.GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        _camera.Follow = _player;
        _camera.LookAt = _player;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            _cameraObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            _cameraObject.SetActive(false);
        }
    }
}
