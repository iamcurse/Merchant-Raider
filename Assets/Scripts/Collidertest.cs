using UnityEngine;

public class ColliderTest2D : MonoBehaviour
{
    public InventoryManager inventoryManager;  // Reference to the InventoryManager

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Interactable"))
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
    //             inventoryManager.AddItemByName(collidedObjectName);
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
    //     if (collision.gameObject.CompareTag("Interactable"))
    //     {
    //         Debug.Log("Stopped colliding with an interactable object: " + collision.gameObject.name);
    //     }
    // }
}
