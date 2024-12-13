using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerController _playerController;
    private List<EnemyController> _enemiesInRange;
    // ReSharper disable once NotAccessedField.Local
    [ShowOnly][SerializeField] private int enemyCount;
    
    private BoxCollider2D _boxCollider2D;
    [HideInInspector] public float moveX;
    [HideInInspector] public float moveY;
    
    [SerializeField] private GameObject arrowPrefab;
    public float arrowSpeed = 10f;
    
    private void Awake()
    {
        _playerController = transform.parent.gameObject.GetComponent<PlayerController>();
        _enemiesInRange = new List<EnemyController>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        
        // Check if the enemy is already in the list before adding it
        var enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy == null || _enemiesInRange.Contains(enemy)) return;
        // Add the enemy to the list of enemies in range
        _enemiesInRange.Add(enemy);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        
        // Check if the enemy is in the list before removing it
        var enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy == null || !_enemiesInRange.Contains(enemy)) return;
        // Remove the enemy from the list of enemies in range
        _enemiesInRange.Remove(enemy);
    }
    
    private void Update()
    {
        // Update the player in attack range in PlayerController
        _playerController.enemyInAttackRange = _enemiesInRange.Count > 0;
        enemyCount = _enemiesInRange.Count;
        
        switch (moveX)
        {
            case > 0 when moveY == 0:
                AttackRight();
                break;
            case < 0 when moveY == 0:
                AttackLeft();
                break;
            case 0 when moveY > 0:
                AttackUp();
                break;
            case 0 when moveY < 0:
                AttackDown();
                break;
        }
    }

    public void CloseAttack()
    {
        if (_enemiesInRange.Count == 0) return;

        var enemiesToRemove = new List<EnemyController>();

        foreach (var enemy in _enemiesInRange)
        {
            enemy.GetHit();
            if (!enemy.isDead) continue;
            enemiesToRemove.Add(enemy);
        }

        foreach (var enemy in enemiesToRemove)
        {
            _enemiesInRange.Remove(enemy);
        }
    }

    private void AttackRight()
    {
        _boxCollider2D.offset = new Vector2(0.145f, 0.095f);
        _boxCollider2D.size = new Vector2(0.28f, 0.18f);
    }
    private void AttackLeft()
    {
        _boxCollider2D.offset = new Vector2(-0.135f, 0.095f);
        _boxCollider2D.size = new Vector2(0.28f, 0.18f);
    }
    private void AttackUp()
    {
        _boxCollider2D.offset = new Vector2(0, 0.205f);
        _boxCollider2D.size = new Vector2(0.28f, 0.18f);
    }
    private void AttackDown()
    {
        _boxCollider2D.offset = new Vector2(0, -0.045f);
        _boxCollider2D.size = new Vector2(0.28f, 0.16f);
    }
    
    public void LongAttack()
    {
        var rotation = Quaternion.Euler(new Vector3(0f, 0f, Utility.AngleTowardsMouse(transform.position) - 90f));
        var Arrow = Instantiate(arrowPrefab, transform.position, rotation);
    }
}
