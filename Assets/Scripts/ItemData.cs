using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{ 
    [Header("Basic Information")]
    public int itemID;               // Unique identifier for the item
    public string itemName;          // Name of the item
    public Sprite itemSprite;        // Icon or sprite for the item
    public string itemDescription;
    public GameObject prefab; // A brief description of the item

    [Header("Stacking Properties")]
    public int stackCount = 1;       // Current number of items in the stack
    public int maxStackCount = 99;   // Maximum number of items in a stack
    
    public ItemData Clone()
    {
        var clone = CreateInstance<ItemData>();
        clone.itemID = itemID;
        clone.itemName = itemName;
        clone.itemSprite = itemSprite;
        clone.itemDescription = itemDescription;
        clone.stackCount = stackCount;
        clone.maxStackCount = maxStackCount;
        return clone;
    }
}