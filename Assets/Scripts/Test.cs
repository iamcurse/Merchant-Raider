using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController.GetHit();
        Debug.Log("Player entered the trigger");
    }
}
