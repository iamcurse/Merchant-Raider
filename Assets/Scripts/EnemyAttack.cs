using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Enemy _enemy;
    
    private void Awake()
    {
        _enemy = transform.parent.gameObject.GetComponent<Enemy>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.playerInRange = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _enemy.playerInRange = false;
        }
    }
}
