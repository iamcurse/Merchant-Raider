using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private Inventory _inventory;
    private GameObject _itemsPanel;
    // public List<Image> slots;
    // public List<Image> slotImages;
    // public string itemImagesFolder = "ItemImages";

    private void Awake()
    {
        _inventory = FindAnyObjectByType<PlayerController>().inventory;
        _itemsPanel = transform.GetChild(1).transform.GetChild(1).gameObject;
    }

    private void FixedUpdate() => ListItems();

    private void ListItems()
    {
        for (var currentSlotIndex = 0; currentSlotIndex < _inventory.items.Count; currentSlotIndex++)
        {
            var slotImage = _itemsPanel.transform.GetChild(currentSlotIndex).GetChild(0).gameObject
                .GetComponent<Image>();
            slotImage.sprite = _inventory.items[currentSlotIndex].itemSprite;
        }
    }
    
    private void ClearInventory()
    {
        _inventory.items.Clear();
    }
    
    public void AddItem(ItemData item)
    {
        if (_inventory.items.Count < _inventory.maxItems)
            _inventory.items.Add(item);
        
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    


    // private void AssignSlotsAndSlotImages()
    // {
    //     slots.Clear();
    //     slotImages.Clear();
    //
    //     foreach (Transform item in _itemsPanel.transform)
    //     {
    //         Image itemImage = item.GetComponent<Image>();
    //         if (itemImage != null)
    //         {
    //             slots.Add(itemImage);
    //
    //             Transform slotTransform = item.Find("Slot");
    //             if (slotTransform != null)
    //             {
    //                 Image slotImage = slotTransform.GetComponent<Image>();
    //                 if (slotImage != null)
    //                 {
    //                     slotImages.Add(slotImage);
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // public void AddItemByName(string itemName)
    // {
    //     Sprite[] itemImages = Resources.LoadAll<Sprite>(itemImagesFolder);
    //
    //     bool objectFound = false;
    //
    //     foreach (var item in itemImages)
    //     {
    //         if (item.name == itemName)
    //         {
    //             objectFound = true;
    //             AddItemBySprite(item);
    //             break;
    //         }
    //     }
    //
    //     if (!objectFound)
    //     {
    //         Debug.Log("No matching item image found for: " + itemName);
    //     }
    // }
    //
    // private void AddItemBySprite(Sprite itemSprite)
    // {
    //     if (_currentSlotIndex >= slotImages.Count)
    //     {
    //         Debug.LogWarning("Inventory is full!");
    //         return;
    //     }
    //
    //     Image slotImage = slotImages[_currentSlotIndex];
    //     slotImage.sprite = itemSprite;
    //     slotImage.enabled = true;
    //
    //     _currentSlotIndex++;
    // }
    //
    // public void ClearInventory()
    // {
    //     _currentSlotIndex = 0;
    //
    //     foreach (Image slotImage in slotImages)
    //     {
    //         slotImage.sprite = null;
    //         slotImage.enabled = false;
    //     }
    // }
    //
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Item"))
    //     {
    //         string collidedObjectName = collision.gameObject.name;
    //
    //         Sprite[] itemImages = Resources.LoadAll<Sprite>("ItemImages");
    //
    //         bool objectFound = false;
    //
    //         foreach (var item in itemImages)
    //         {
    //             if (item.name == collidedObjectName)
    //             {
    //                 objectFound = true;
    //                 break;
    //             }
    //         }
    //
    //         if (objectFound)
    //         {
    //             Debug.Log("Found matching item image for: " + collidedObjectName);
    //
    //             // Call the method in InventoryManager to add the item by name
    //             AddItemByName(collidedObjectName);
    //
    //             // Destroy the interactable object
    //             Destroy(collision.gameObject);
    //         }
    //         else
    //         {
    //             Debug.Log("No matching item image found for: " + collidedObjectName);
    //         }
    //     }
    // }
    //
    // private void OnCollisionExit2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Item"))
    //     {
    //         Debug.Log("Stopped colliding with an interactable object: " + collision.gameObject.name);
    //     }
    // }
    
    
}
