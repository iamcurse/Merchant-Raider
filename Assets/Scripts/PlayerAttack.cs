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
    [HideInInspector] public float arrowSpeed;
    
    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _enemiesInRange = new List<EnemyController>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        arrowSpeed = _playerController.arrowSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy Hit Box")) return;
        
        // Check if the enemy is already in the list before adding it
        var enemy = other.gameObject.GetComponentInParent<EnemyController>();
        if (enemy == null || _enemiesInRange.Contains(enemy)) return;
        // Add the enemy to the list of enemies in range
        _enemiesInRange.Add(enemy);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy Hit Box")) return;
        
        // Check if the enemy is in the list before removing it
        var enemy = other.gameObject.GetComponentInParent<EnemyController>();
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
            // Perform a raycast to check for obstacles
            var direction = (enemy.transform.position - transform.position).normalized;
            var distance = Vector2.Distance(transform.position, enemy.transform.position);
            var hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask("Collision"));
            
            // If the raycast hits an obstacle, skip this enemy
            if (hit.collider != null) continue;
            
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
        if (Camera.main == null) return;
        var mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0; // Ensure the z-coordinate is set to 0
        var playerPosition = _playerController.transform.position;

        // Calculate the direction based on the mouse position
        var direction = (mousePosition - playerPosition).normalized;

        // Adjust bow tip position based on the player's position and direction
        var bowTipPosition = new Vector3(playerPosition.x + direction.x * 0.1f, playerPosition.y + direction.y * 0.1f, 0);

        // Calculate the angle towards the mouse from the bow tip position
        var angle = Utility.AngleTowardsMouse(bowTipPosition);
        var rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 90f));

        // Instantiate the arrow at the bow tip position with the calculated rotation
        // ReSharper disable once UnusedVariable
        var arrow = Instantiate(arrowPrefab, bowTipPosition, rotation);
    }
}
