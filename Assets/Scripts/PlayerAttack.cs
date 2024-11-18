using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerController _playerController;
    private List<Enemy> _enemiesInRange;
    // ReSharper disable once NotAccessedField.Local
    [ShowOnly][SerializeField] private int enemyCount;
    
    private BoxCollider2D _boxCollider2D;
    [HideInInspector] public bool isRight;
    [HideInInspector] public bool isLeft;
    [HideInInspector] public bool isUp;
    [HideInInspector] public bool isDown;
    
    private void Awake()
    {
        _playerController = transform.parent.gameObject.GetComponent<PlayerController>();
        _enemiesInRange = new List<Enemy>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy == null || _enemiesInRange.Contains(enemy)) return;
        _enemiesInRange.Add(enemy);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy == null || !_enemiesInRange.Contains(enemy)) return;
        _enemiesInRange.Remove(enemy);
    }
    
    private void Update()
    {
        _playerController.enemyInAttackRange = _enemiesInRange.Count > 0;
        enemyCount = _enemiesInRange.Count;
        
        if (isUp)
        {
            AttackUp();
        }
        else if (isDown)
        {
            AttackDown();
        }
        else if (isLeft)
        {
            AttackLeft();
        }
        else if (isRight)
        {
            AttackRight();
        }
    }

    public void Attack()
    {
        if (_enemiesInRange.Count == 0) return;

        var enemiesToRemove = new List<Enemy>();

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
        _boxCollider2D.offset = new Vector2(0.14f, -0.04f);
        _boxCollider2D.size = new Vector2(0.28f, 0.18f);
    }
    private void AttackLeft()
    {
        _boxCollider2D.offset = new Vector2(-0.14f, -0.04f);
        _boxCollider2D.size = new Vector2(0.28f, 0.18f);
    }
    private void AttackUp()
    {
        _boxCollider2D.offset = new Vector2(0, 0.09f);
        _boxCollider2D.size = new Vector2(0.28f, 0.18f);
    }
    private void AttackDown()
    {
        _boxCollider2D.offset = new Vector2(0, -0.14f);
        _boxCollider2D.size = new Vector2(0.28f, 0.18f);
    }
}
