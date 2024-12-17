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
        
        if (_playerController.inventoryManager.CountItem() >= _playerController.inventory.maxItems) return;
        _playerController.inventoryManager.AddItem(itemData);
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController = null;
    }
}
