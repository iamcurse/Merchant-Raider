using UnityEngine;

namespace Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        [ShowOnly] public bool playerInAttackRange;
    }
}