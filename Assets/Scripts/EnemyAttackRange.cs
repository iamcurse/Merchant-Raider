using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    private EnemyController _enemy;
    
    private void Awake()
    {
        _enemy = transform.parent.gameObject.GetComponent<EnemyController>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.playerInAttackRange = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.playerInAttackRange = false;
        }
    }
}
