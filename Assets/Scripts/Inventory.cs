using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public List<ItemData> items;
    public int maxItems = 36;
    
    public Inventory Clone()
    {
        var clone = CreateInstance<Inventory>();
        clone.items = new List<ItemData>(items);
        clone.maxItems = maxItems;
        return clone;
    }
}
