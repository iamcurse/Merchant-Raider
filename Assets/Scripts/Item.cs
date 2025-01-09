using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    
    private PlayerController _playerController;

    private void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = itemData.itemSprite;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
         if (!other.CompareTag("Player")) return;

    _playerController = other.GetComponent<PlayerController>();

    // Check if there is space for the item (max inventory size)
    if (_playerController.inventoryManager.CountItem() >= _playerController.inventory.maxItems) return;

    // Add the item to the inventory (this method handles stacking)
    _playerController.inventoryManager.AddItem(itemData);

    // Destroy the item object from the world
    Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController = null;
    }
}
