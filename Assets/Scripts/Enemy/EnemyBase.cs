using UnityEngine;

namespace Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [ShowOnly] public bool playerInAttackRange;
        [HideInInspector] public Animator animator;

        public virtual void GetHit() => Debug.Log("Enemy got hit");
        public virtual void GetHit(int damage) => Debug.Log($"Enemy got hit with {damage} damage");
        [ShowOnly] public bool isDead;
    }
}