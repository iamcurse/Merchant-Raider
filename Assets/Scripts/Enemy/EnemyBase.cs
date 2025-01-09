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
        
        [SerializeField] private bool dropItemOnDeath;
        [EnabledIf("dropItemOnDeath")][SerializeField] private ItemData dropItem;


        protected void DropItem()
        {
            if (!dropItemOnDeath) return;
            if (dropItem != null && dropItem.prefab != null)
            {
                Instantiate(dropItem.prefab, transform.position, Quaternion.identity);  // Drop the item at the enemy's position
                Debug.Log($"Dropped {dropItem.itemName} at {transform.position}");
            }
            else
            {
                Debug.Log("Item is not assigned.");
            }
        }
    }
}