using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canInteract;
    [SerializeField] private bool canAttack = true;
    
    private Vector2 _movementInput;
    private Rigidbody2D _rigidBody2D;
    
    [ShowOnly][SerializeField] private bool isAttacking;
    [ShowOnly][SerializeField] private bool isHit;

    private Animator _animator;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");


    private PlayerInput _playerInput;
    private InputAction _move;
    private InputAction _interact;
    private InputAction _attackCloseRange;
    private InputAction _attackLongRange;

    [ShowOnly] public bool isDead;

    private PauseMenu _pauseUI;
    [ShowOnly] public bool isPause;
    
    private InteractableObject _interactableObject;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _move = _playerInput.Player.Move;
        _interact = _playerInput.Player.Interact;
        _attackCloseRange = _playerInput.Player.AttackCloseRange;
        _attackLongRange = _playerInput.Player.AttackLongRange;

        _pauseUI = GameObject.Find("UI").GetComponent<PauseMenu>();
    }

    private void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Animate();
    }
    
    private void Update()
    {
        Move();
    }

    private void OnEnable()
    {
        _move.Enable();
        _interact.Enable();
        _attackCloseRange.Enable();
        _attackLongRange.Enable();
        
        Lua.RegisterFunction(nameof(GetHit), this, GetType().GetMethod(nameof(GetHit)));
    }
    
    private void OnDisable()
    {
        _move.Disable();
        _interact.Disable();
        _attackCloseRange.Disable();
        _attackLongRange.Disable();
        
        Lua.UnregisterFunction(nameof(GetHit));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable Object")) return;
        canInteract = true;
        
       _interactableObject = other.gameObject.GetComponent<InteractableObject>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable Object")) return;
        canInteract = false;
        _interactableObject = null;
    }

    private void Move()
    {
        if (!canMove || isDead || isPause || DialogueManager.isConversationActive) return;
        _movementInput = _move.ReadValue<Vector2>();
        _rigidBody2D.linearVelocity = _movementInput * speed;
        Debug.Log(_movementInput);
    }

    private void Animate()
    {
        if (isAttacking) return;
        if (_movementInput != Vector2.zero)
        {
            _animator.SetFloat(MoveX, _movementInput.x);
            _animator.SetFloat(MoveY, _movementInput.y);
            if (isHit) return;
            _animator.Play("Player_Walk");
        } else
        {
            if (isHit) return;
            _animator.Play("Player_Idle");
        }
    }

    public void GetHit()
    {
        //Public method to call when player gets hit, when player gets hit, player can't attack
        isHit = true;
        Debug.Log("Player got hit");
        _animator.Play("Player_Hit", -1, 0f);
    }

    private void OnInteract()
    {
        if (!canInteract || isPause || DialogueManager.isConversationActive) return;
        
        Debug.Log("Interact");
        //Do something when 'E' is pressed
        _interactableObject.Interact();
    }
    
    private void OnAttackCloseRange()
    {
        if (!canAttack || isHit || isPause || DialogueManager.isConversationActive) return;
        _animator.Play("Player_Attack");
        Debug.Log("Attack Close Range");
        
        //Do something when Left Click
        
    }
    
    private void OnAttackLongRange()
    {
        if (!canAttack || isHit || isPause || DialogueManager.isConversationActive) return;
        Debug.Log("Attack Long Range");
        
        //Do something when Right Click
        
    }

    private void OnPause()
    {
        if (DialogueManager.isConversationActive) return;
        _pauseUI.PauseScript();
    }
}
