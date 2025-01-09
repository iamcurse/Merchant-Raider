using UnityEngine;

public class DropItems : MonoBehaviour
{
    [SerializeField] private DroppedItem item; 
    public void DropItem(Vector2 position)
    {
        if (item == null || item.worldPrefab == null)
        {
            Debug.LogWarning("Item is not assigned.");
            return;
        }

        Instantiate(item.worldPrefab, position, Quaternion.identity);
        Debug.Log($"Dropped {item.itemName} at {position}");
    }
}
