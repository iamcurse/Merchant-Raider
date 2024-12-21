using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private PlayerAttack _playerAttack;
    
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float lifeTime = 4f;
    
    [Obsolete("Obsolete")]
    private void Awake()
    {
        _playerAttack = FindObjectOfType<PlayerAttack>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }   

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = transform.up * _playerAttack.arrowSpeed;
    }
    
    public void SetDirection(Vector2 direction)
    {
        _rigidbody2D.linearVelocity = direction * _playerAttack.arrowSpeed;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Collision") && !other.gameObject.CompareTag("Interactable Object")) return;
        Debug.Log("Hit: " + other.gameObject.name);
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy Hit Box")) return;
        Debug.Log("Hit: " + other.gameObject.name);
        var enemy = other.gameObject.GetComponentInParent<EnemyController>();
        enemy.GetHit();
        Destroy(gameObject);
    }
    
    
}
