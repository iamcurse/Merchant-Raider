using UnityEngine;

namespace Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [ShowOnly] public bool playerInAttackRange;
        [HideInInspector] public Animator animator;
    }
}