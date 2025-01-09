using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public string description;
    public Sprite itemSprite;
    public GameObject prefab;
    public bool isStackable;
    public int stackCount = 1; 
    [DisabledIf("isStackable")] public int maxStackSize = 99;
    
    
    public ItemData Clone()
    {
        var clone = CreateInstance<ItemData>();
        clone.itemID = itemID;
        clone.itemName = itemName;
        clone.itemSprite = itemSprite;
        clone.stackCount = stackCount;
        return clone;
    }
}
