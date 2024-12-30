using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite itemSprite;
    
    public ItemData Clone()
    {
        var clone = CreateInstance<ItemData>();
        clone.itemID = itemID;
        clone.itemName = itemName;
        clone.itemSprite = itemSprite;
        return clone;
    }
}
