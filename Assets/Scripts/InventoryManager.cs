using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    private PlayerController _playerController;
    private Inventory _inventory;
    private GameObject _itemsPanel;
    [SerializeField] private Inventory itemDatabase;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError("Multiple InventoryManager instances found! Destroying duplicate.");
            Destroy(gameObject);
        }
        
        _itemsPanel = transform.GetChild(1).transform.GetChild(1).gameObject;
    }
    
    private void Start()
    {
        _playerController = PlayerController.Instance;
        _inventory = _playerController.inventory;
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
        // Reset all inventory slots (clear item sprites and stack counts)
    for (var i = 0; i < _itemsPanel.transform.childCount; i++)
    {
        var slotImage = _itemsPanel.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Image>();
        slotImage.sprite = null;

        // Find the StackCounter (TextMeshProUGUI) component in the slot
        var stackText = _itemsPanel.transform.GetChild(i).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        stackText.text = ""; // Clear stack text
    }
    
    // Populate inventory slots with items
    for (var currentSlotIndex = 0; currentSlotIndex < _inventory.items.Count; currentSlotIndex++)
    {
        var item = _inventory.items[currentSlotIndex];
        
        // Set item sprite in the inventory slot
        var slotImage = _itemsPanel.transform.GetChild(currentSlotIndex).GetChild(0).gameObject.GetComponent<Image>();
        slotImage.sprite = item.itemSprite;

        // Find the StackCounter (TextMeshProUGUI) component
        var stackText = _itemsPanel.transform.GetChild(currentSlotIndex).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        
        // Show stack count if greater than 1
        stackText.text = item.stackCount > 1 ? item.stackCount.ToString() : ""; // Display stack count
    }
    }
    
    public void ClearInventory()
    {
        _inventory.items.Clear();
    }
    
    public void AddItem(ItemData item)
    {
    // Check if the item is already in the inventory
    var existingItem = _inventory.items.FirstOrDefault(i => i.itemID == item.itemID);

    if (existingItem != null)
    {
        // Check if the stack is not full
        if (existingItem.stackCount < item.maxStackCount)
        {
            existingItem.stackCount++;
            return;
        }
    }

    // If no existing stack or stack is full, find an empty slot
    if (_inventory.items.Count < _inventory.maxItems)
    {
        item.stackCount = 1;

        _inventory.items.Add(item);
    }
    else
    {
        Debug.Log("Inventory is full! Cannot add more items.");
    }
}

// Overloaded AddItem method to add by item name
public void AddItem(string itemName)
{
    // Find the item in the item database
    var itemToAdd = itemDatabase.items.FirstOrDefault(i => i.itemName == itemName);

    if (itemToAdd != null)
    {
        AddItem(itemToAdd);
    }
    else
    {
        Debug.LogError($"Item with name '{itemName}' not found in the item database.");
    }
}
    
    public ItemData GetItem(string itemName)
    {
        return itemDatabase.items.Find(item => item.itemName == itemName);
    }
    
    public bool CheckItem(ItemData item)
    {
        return _inventory.items.Contains(item);
    }

    private bool CheckItem(string itemName)
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
    
    private int CountItem(string itemName)
    {
        return _inventory.items.Count(i => i.itemName == itemName);
    }
    
    public void RemoveItem(ItemData item)
    {
         // Check if the item exists in the inventory
        var existingItem = _inventory.items.FirstOrDefault(i => i.itemID == item.itemID);
        if (existingItem == null)
        {
            Debug.LogError($"Item with name '{item.itemName}' was not found in the item database.");
            return;
        }
        
        if (existingItem == null) return;
        if (existingItem.stackCount > 1)
        {
            existingItem.stackCount--; 
        }
        else
        {
            _inventory.items.Remove(existingItem); 
        }
    }
    
    private void RemoveItem(string itemName)
    {
        var item = itemDatabase.items.FirstOrDefault(i => i.itemName == itemName);
        if (item == null)
        {
            Debug.LogError($"Item with name '{itemName}' not found in the item database.");
            return;
        }

        if (item == null) return;
        if (item.stackCount > 1)
        {
            item.stackCount--;
        }
        else
        {
            _inventory.items.Remove(item);
        }
    }
}
