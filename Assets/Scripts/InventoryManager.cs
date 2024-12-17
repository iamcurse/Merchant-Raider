using System;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private Inventory _inventory;
    private GameObject _itemsPanel;
    [SerializeField] private Inventory itemDatabase;

    private void Awake()
    {
        _inventory = FindAnyObjectByType<PlayerController>().inventory;
        _itemsPanel = transform.GetChild(1).transform.GetChild(1).gameObject;
    }

    private void FixedUpdate() => ListItems();

    private void OnEnable()
    {
        Lua.RegisterFunction("AddItem", this, SymbolExtensions.GetMethodInfo(() => AddItem("")));
        Lua.RegisterFunction("CheckItem", this, SymbolExtensions.GetMethodInfo(() => CheckItem("")));
        Lua.RegisterFunction("CountItem", this, SymbolExtensions.GetMethodInfo(() => CountItem("")));
        Lua.RegisterFunction("RemoveItem", this, SymbolExtensions.GetMethodInfo(() => RemoveItem("")));
        Lua.RegisterFunction("ClearInventory", this, SymbolExtensions.GetMethodInfo(() => ClearInventory()));
    }
    
    private void OnDisable()
    {
        Lua.UnregisterFunction("AddItem");
        Lua.UnregisterFunction("CheckItem");
        Lua.UnregisterFunction("CountItem");
        Lua.UnregisterFunction("RemoveItem");
        Lua.UnregisterFunction("ClearInventory");
    }

    private void ListItems()
    {
        for (var i = 0; i < _itemsPanel.transform.childCount; i++)
        {
            var slotImage = _itemsPanel.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Image>();
            slotImage.sprite = null;
        }
        
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
    
    private void AddItem(String itemName)
    {
        var item = itemDatabase.items.Find(i => i.itemName == itemName);
        if (_inventory.items.Count < _inventory.maxItems)
            _inventory.items.Add(item);
    }
    
    public ItemData GetItem(String itemName)
    {
        return itemDatabase.items.Find(item => item.itemName == itemName);
    }
    
    public bool CheckItem(ItemData item)
    {
        return _inventory.items.Contains(item);
    }

    private bool CheckItem(String itemName)
    {
        return _inventory.items.Any(item => item.itemName == itemName);
    }
    
    public int CountItem()
    {
        return _inventory.items.Count;
    }
    
    public int CountItem(ItemData item)
    {
        return _inventory.items.Count(i => i == item);
    }
    
    private int CountItem(String itemName)
    {
        return _inventory.items.Count(i => i.itemName == itemName);
    }
    
    public void RemoveItem(ItemData item)
    {
        if (CheckItem(item))
            _inventory.items.Remove(item);
    }
    
    private void RemoveItem(String itemName)
    {
        var item = itemDatabase.items.Find(i => i.itemName == itemName);
        if (CheckItem(item))
            _inventory.items.Remove(item);
    }
}
