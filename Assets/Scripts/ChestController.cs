using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

public class ChestController : MonoBehaviour
{
    private static readonly int Open = Animator.StringToHash("Open");
    [SerializeField][ShowOnly] private bool isOpen;
    private PlayerController _playerController;
    private Animator _animator;

    [SerializeField] private UnityEvent onOpen;
    
    [SerializeField] private List<ItemData> _itemsDrop;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController = other.GetComponent<PlayerController>();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController = null;
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("OpenChest", this, SymbolExtensions.GetMethodInfo(() => OpenChest("")));
    }
    
    private void OnDisable()
    {
        Lua.UnregisterFunction("OpenChest");
    }

    public void ChestInteraction()
    {
        if (isOpen || (_playerController == null)) return;
        isOpen = true;
        
        DialogueLua.SetVariable("ChestName", name);
        
        switch (_itemsDrop.Count)
        {
            case 1:
                DialogueLua.SetVariable("ItemName", _itemsDrop[0].itemName);
                break;
            case > 1:
                var items = string.Join(", ", _itemsDrop.Take(_itemsDrop.Count - 1).Select(item => item.itemName)) +
                            ", and " + _itemsDrop.Last().itemName;
                DialogueLua.SetVariable("ItemName", items);
                break;
        }
        
        onOpen.Invoke();
    }
    
    private void OpenChest(string gameObjectName)
    {
        var chestGameObject = GameObject.Find(gameObjectName).GetComponent<ChestController>();
        chestGameObject.OpenChest();
    }

    private void OpenChest()
    {
        _animator.SetTrigger(Open);
        foreach (var item in _itemsDrop)
        {
            _playerController.inventoryManager.AddItem(item);
        }
    }
}
