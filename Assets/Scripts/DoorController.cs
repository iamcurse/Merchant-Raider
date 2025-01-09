using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

public class DoorController : MonoBehaviour
{
    private static readonly int Door = Animator.StringToHash("OpenDoor");
    [ShowOnly][SerializeField] private bool isOpen;
    [SerializeField] private bool needKey;
    [EnabledIf("needKey")][SerializeField] private ItemData key;
    
    private Animator _animator;
    
    private BoxCollider2D[] _boxCollider2Ds;
    private GameObject _closedDoor;
    private GameObject _openDoor;

    [SerializeField] private UnityEvent onDoorOpen;

    private void Awake()
    {
        _boxCollider2Ds = GetComponents<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        
        _closedDoor = transform.GetChild(1).gameObject;
        _openDoor = transform.GetChild(2).gameObject;
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("OpenDoor", this, SymbolExtensions.GetMethodInfo(() => OpenDoor("")));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("OpenDoor");
    }

    public void DoorInteract()
    {
        Debug.Log("Door interacted");
        if (isOpen) return;
        
        if (needKey && key == null)
            Debug.LogWarning("Key is missing!");
        
        UploadData();
        
        onDoorOpen.Invoke();
    }

    private void UploadData()
    {
        DialogueLua.SetVariable("Door_Name", name);
        DialogueLua.SetVariable("Door_NeedKey", needKey);
        DialogueLua.SetVariable("Door_KeyName", key.itemName);
    }

    private void OpenDoor()
    {
        isOpen = true;
        _closedDoor.SetActive(false);
        _animator.SetTrigger(Door);
        _boxCollider2Ds[0].enabled = false;
        _openDoor.SetActive(true);
    }
    
    private static void OpenDoor(string gameObjectName)
    {
        var door = SequencerTools.FindSpecifier(gameObjectName).GetComponent<DoorController>();
        door.OpenDoor();
    }
}
