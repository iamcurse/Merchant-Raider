using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerController _playerController;

    private List<Enemy> _enemiesInRange;
    
    // ReSharper disable once NotAccessedField.Local
    [ShowOnly][SerializeField] private int enemyCount;
    
    
    private void Awake()
    {
        _playerController = transform.parent.gameObject.GetComponent<PlayerController>();
        _enemiesInRange = new List<Enemy>();
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
}
