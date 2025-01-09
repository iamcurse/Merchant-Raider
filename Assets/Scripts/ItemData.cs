using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite itemSprite;
    public int stackCount = 1; 
    public int maxStackSize = 99; 
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
