using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
        [Header("Basic Information")]
    public int itemID;               // Unique identifier for the item
    public string itemName;          // Name of the item
    public Sprite itemSprite;        // Icon or sprite for the item
    public string itemDescription;   // A brief description of the item

    [Header("Stacking Properties")]
    public int stackCount = 1;       // Current number of items in the stack
    public int maxStackCount = 99;   // Maximum number of items in a stack

    [Header("Item Type")]
    public ItemType itemType;        // Enum for categorizing item types

    [Header("Miscellaneous")]
    public bool isConsumable;        // Is the item consumable (e.g., health potion)
    public int value;                // Value of the item (e.g., for selling or currency)

    /// <summary>
    /// Creates a deep copy of this item for inventory purposes.
    /// </summary>
    public ItemData Clone()
    {
        var clone = CreateInstance<ItemData>();
        clone.itemID = itemID;
        clone.itemName = itemName;
        clone.itemSprite = itemSprite;
        clone.itemDescription = itemDescription;
        clone.stackCount = stackCount;
        clone.maxStackCount = maxStackCount;
        clone.itemType = itemType;
        clone.isConsumable = isConsumable;
        clone.value = value;
        return clone;
    }
}

/// <summary>
/// Enum to define different item types.
/// Extend this to include specific types for your game.
/// </summary>
public enum ItemType
{
    Generic,
    Weapon,
    Armor,
    Consumable,
    KeyItem,
    CraftingMaterial
}
