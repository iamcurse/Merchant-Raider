using UnityEngine;

public class Level01 : MonoBehaviour
{ 
    private InventoryManager _inventoryManager;

    private void Start()
   {
        _inventoryManager = InventoryManager.Instance;
        _inventoryManager.ClearInventory();
   }
}
