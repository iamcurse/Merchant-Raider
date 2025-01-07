using UnityEngine;

namespace Enemy
{
    public class EnemyAttackRange : MonoBehaviour
    {
        private EnemyBase _enemy;
        private Collider2D _attackRange;

        private float _attackRangeX;
        private float _attackRangeY;
    
        private void Awake()
        {
            _enemy = GetComponentInParent<EnemyBase>();
            _attackRange = GetComponent<Collider2D>();
        }

        private void Start()
        {
            _attackRangeX = _attackRange.offset.x;
            _attackRangeY = _attackRange.offset.y;
        }

        private void Update()
        {
            _attackRange.offset = _enemy.spriteRenderer.flipX ? new Vector2(-_attackRangeX, _attackRangeY) : new Vector2(_attackRangeX, _attackRangeY);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player Hit Box"))
            {
                // Set the player in attack range in EnemyController to true when player enters the attack range of enemy
                _enemy.playerInAttackRange = true;
            }
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player Hit Box"))
            {
                // Set the player in attack range in EnemyController to false when player exits the attack range of enemy
                _enemy.playerInAttackRange = false;
            }
        }
    }
}
