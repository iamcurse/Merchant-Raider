using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    
    private PlayerController playerController;

    private void Awake()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = itemData.itemSprite;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerController = other.GetComponent<PlayerController>();
        if (playerController.inventory.items.Count >= playerController.inventory.maxItems) return;
        playerController.inventory.items.Add(itemData);
        Destroy(gameObject);
    }
}
