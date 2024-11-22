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
            // Set the player in attack range in EnemyController to true when player enters the attack range of enemy
            _enemy.playerInAttackRange = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Set the player in attack range in EnemyController to false when player exits the attack range of enemy
            _enemy.playerInAttackRange = false;
        }
    }
}
