using System;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private PlayerController _playerController;
    [SerializeField][ShowOnly] private bool isActivated;
    private bool _playerInRange;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController = other.GetComponent<PlayerController>();
        _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController = null;
        _playerInRange = false;
    }

    private void DoDamage()
    {
        if (_playerController == null) return;
        
        if (isActivated && _playerInRange)
            _playerController.GetHit();
    }

    private void FixedUpdate()
    {
        DoDamage();
    }
}
