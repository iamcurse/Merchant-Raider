using System;
using System.Linq;
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
    
    public ItemData GetItem(String itemName)
    {
        return itemDatabase.items.Find(item => item.itemName == itemName);
    }
    
    public bool CheckItem(ItemData item)
    {
        return _inventory.items.Contains(item);
    }
    
    public int CountItem(ItemData item)
    {
        return _inventory.items.Count(i => i == item);
    }
    
    public void RemoveItem(ItemData item)
    {
        if (CheckItem(item))
            _inventory.items.Remove(item);
        ListItems();
    }
}
