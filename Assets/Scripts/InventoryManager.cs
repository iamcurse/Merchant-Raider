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
    
    private void ClearInventory()
    {
        _inventory.items.Clear();
    }
    
    public void AddItem(ItemData item)
    {
       // Check if the item already exists in the inventory (based on itemID)
    var existingItem = _inventory.items.FirstOrDefault(i => i.itemID == item.itemID);

    if (existingItem != null) // If the item already exists in the inventory
    {
        // Increase the stack count (ensure it doesn't exceed max stack size)
        existingItem.stackCount = Mathf.Min(existingItem.stackCount + 1, item.maxStackSize); // Adjust stacking behavior
    }
    else
    {
        // Add the item if it's not already in the inventory
        if (_inventory.items.Count < _inventory.maxItems)
        {
            item.stackCount = 1; // Initialize stack count to 1 for a new item
            _inventory.items.Add(item);
        }
    }
    }
    
    private void AddItem(string itemName)
    {
        var item = itemDatabase.items.Find(i => i.itemName == itemName);
        if (_inventory.items.Count < _inventory.maxItems)
            _inventory.items.Add(item);
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

    if (existingItem != null)
    {
        
        if (existingItem.stackCount > 1)
        {
            existingItem.stackCount--; 
        }
        else
        {
            _inventory.items.Remove(existingItem); 
        }
    }
    }
    
    private void RemoveItem(string itemName)
    {
        var item = itemDatabase.items.Find(i => i.itemName == itemName);
        if (CheckItem(item))
            _inventory.items.Remove(item);
    }
}
