using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public GameObject itemsPanel;
    public List<Image> slots;
    public List<Image> slotImages;
    public string itemImagesFolder = "ItemImages";

    private int currentSlotIndex = 0;

    private void Start()
    {
        AssignSlotsAndSlotImages();
    }

    private void AssignSlotsAndSlotImages()
    {
        slots.Clear();
        slotImages.Clear();

        foreach (Transform item in itemsPanel.transform)
        {
            Image itemImage = item.GetComponent<Image>();
            if (itemImage != null)
            {
                slots.Add(itemImage);

                Transform slotTransform = item.Find("Slot");
                if (slotTransform != null)
                {
                    Image slotImage = slotTransform.GetComponent<Image>();
                    if (slotImage != null)
                    {
                        slotImages.Add(slotImage);
                    }
                }
            }
        }
    }

    public void AddItemByName(string itemName)
    {
        Sprite[] itemImages = Resources.LoadAll<Sprite>(itemImagesFolder);

        bool objectFound = false;

        foreach (var item in itemImages)
        {
            if (item.name == itemName)
            {
                objectFound = true;
                AddItemBySprite(item);
                break;
            }
        }

        if (!objectFound)
        {
            Debug.Log("No matching item image found for: " + itemName);
        }
    }

    private void AddItemBySprite(Sprite itemSprite)
    {
        if (currentSlotIndex >= slotImages.Count)
        {
            Debug.LogWarning("Inventory is full!");
            return;
        }

        Image slotImage = slotImages[currentSlotIndex];
        slotImage.sprite = itemSprite;
        slotImage.enabled = true;

        currentSlotIndex++;
    }

    public void ClearInventory()
    {
        currentSlotIndex = 0;

        foreach (Image slotImage in slotImages)
        {
            slotImage.sprite = null;
            slotImage.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            string collidedObjectName = collision.gameObject.name;

            Sprite[] itemImages = Resources.LoadAll<Sprite>("ItemImages");

            bool objectFound = false;

            foreach (var item in itemImages)
            {
                if (item.name == collidedObjectName)
                {
                    objectFound = true;
                    break;
                }
            }

            if (objectFound)
            {
                Debug.Log("Found matching item image for: " + collidedObjectName);

                // Call the method in InventoryManager to add the item by name
                AddItemByName(collidedObjectName);

                // Destroy the interactable object
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("No matching item image found for: " + collidedObjectName);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            Debug.Log("Stopped colliding with an interactable object: " + collision.gameObject.name);
        }
    }
}
