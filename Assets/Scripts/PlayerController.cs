using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 1f;
    [SerializeField] private float closeAttackCooldown = 1f;
    [SerializeField] private float closeRangeAttackMoveSpeed = 0.5f;
    [SerializeField] private float longAttackCooldown = 1f;
    [SerializeField] private float longRangeAttackMoveSpeed = 0.5f;
    // ReSharper disable once NotAccessedField.Global
    [ShowOnly] public bool enemyInAttackRange;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canInteract;
    [SerializeField] private bool canCloseAttack = true;
    [SerializeField] private bool canLongAttack = true;
    [SerializeField] private bool infiniteArrow;
    public float arrowSpeed = 3f;
    [SerializeField] private bool immune;
    private float _closeRangeAttackTimer;
    private float _longRangeAttackTimer;

    
    private Vector2 _movementInput;
    private Rigidbody2D _rigidBody2D;
    
    [ShowOnly][SerializeField] private bool isAttacking;
    [ShowOnly][SerializeField] private bool isHit;
    private bool _bowAttack;
    private Vector2 _bowDirection;

    private Animator _animator;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsHit = Animator.StringToHash("isHit");
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int IsDead = Animator.StringToHash("isDead");
    private static readonly int IsAttackBow = Animator.StringToHash("isAttackBow");

    private PlayerInput _playerInput;
    private InputAction _move;
    private InputAction _interact;
    private InputAction _attackCloseRange;
    private InputAction _attackLongRange;

    [ShowOnly] public bool isDead;
    [ShowOnly] public bool gameOver;

    private GameObject _ui;
    private MenuController _menuController;
    private GameObject _inventoryUI;
    private bool _inventoryActive;
    [HideInInspector] public InventoryManager inventoryManager;
    [ShowOnly] public bool isPause;
    
    private InteractableObject _interactableObject;

    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameObject heart;
    private Transform _healthParent;
    private TMPro.TextMeshProUGUI _moneyText;

    private int _a;
    
    private PlayerAttack _playerAttack;
    
    public Inventory inventory;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _move = _playerInput.Player.Move;
        _interact = _playerInput.Player.Interact;
        _attackCloseRange = _playerInput.Player.AttackCloseRange;
        _attackLongRange = _playerInput.Player.AttackLongRange;

        _ui = GameObject.Find("UI");
        _menuController = _ui.GetComponent<MenuController>();
        inventoryManager = _ui.GetComponent<InventoryManager>();
        
        _healthParent = _ui.transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform;
        _moneyText = _ui.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        _inventoryUI = _ui.transform.GetChild(1).gameObject;
        playerInfo.health = playerInfo.maxHealth;
        _playerAttack = transform.GetChild(0).gameObject.GetComponent<PlayerAttack>();
    }

    private void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        OnHealthChanged();
        OnMoneyChanged();
    }
    
    private void Update()
    {
        Move();
        Animate();
        _inventoryActive = _inventoryUI.activeSelf;
        
        if (_closeRangeAttackTimer > 0)
        {
            _closeRangeAttackTimer -= Time.deltaTime;
        }
        if (_longRangeAttackTimer > 0)
        {
            _longRangeAttackTimer -= Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        _move.Enable();
        _interact.Enable();
        _attackCloseRange.Enable();
        _attackLongRange.Enable();
        
        Lua.RegisterFunction(nameof(GetHit), this, SymbolExtensions.GetMethodInfo(() => GetHit()));
        
        OnHealthChanged();
        OnMoneyChanged();
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
        
       _interactableObject = other.gameObject.GetComponent<InteractableObject>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable Object")) return;
        
        _interactableObject = null;
    }

    private void Move()
    {
        if (!canMove || isDead || isPause /* || DialogueManager.isConversationActive*/)
        {
            _rigidBody2D.linearVelocity = Vector2.zero;
            return;
        }
        
        _movementInput = _move.ReadValue<Vector2>();
        
        if (isAttacking)
        {
            if (_bowAttack)
            {
                _rigidBody2D.linearVelocity = _movementInput * longRangeAttackMoveSpeed;
            } else
            {
                _rigidBody2D.linearVelocity = _movementInput * closeRangeAttackMoveSpeed;
            }
            return;
        }
        
        _rigidBody2D.linearVelocity = _movementInput * playerSpeed;
    }

    private void Animate()
    {
        // If player is dead, it will skip the movement animation.
        if (isDead) return;

        _animator.SetBool(IsMoving, _movementInput != Vector2.zero);
        
        // Check if attack animation is still playing
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Player_Attack") || stateInfo.IsName("Player_Attack_Bow"))
        {
            isAttacking = stateInfo.normalizedTime < 1;
        } else
        {
            isAttacking = false;
        }
        
        // Lock Attack animation during movement
        if (!isAttacking)
        {
            if (_movementInput != Vector2.zero)
            {
                _animator.SetFloat(MoveX, _movementInput.x);
                _animator.SetFloat(MoveY, _movementInput.y);
            }
            
            _playerAttack.moveX = _movementInput.x;
            _playerAttack.moveY = _movementInput.y;
        } else if (_bowAttack)
        {
            _animator.SetFloat(MoveX, _bowDirection.x);
            _animator.SetFloat(MoveY, _bowDirection.y);
        }
    }

    // GetHit() function is called when player gets hit by enemy.
    public void GetHit()
    {
        if (immune || isDead) return;
        immune = true;
        isHit = true;
        playerInfo.health--;
        Debug.Log("Player gets hit");
        
        // If player's health is below 0, it will skip the hit animation and go straight to death animation.
        if (playerInfo.health > 0)
            _animator.SetTrigger(IsHit);
        OnHealthChanged();
    }
    
    public void GetHit(int damage)
    {
        if (immune || isDead) return;
        immune = true;
        isHit = true;
        playerInfo.health -= damage;
        Debug.Log("Player gets hit");
        if (playerInfo.health > 0)
            _animator.SetTrigger(IsHit);
        OnHealthChanged();
    }
    
    // When damage calculation is done, OnHealthChanged() function is called to update the health UI.
    private void OnHealthChanged()
    {
        // Death animation is played when player's health is below 0.
        if (playerInfo.health <= 0)
        {
            _rigidBody2D.linearVelocity = Vector2.zero;
            Debug.Log("Player is dead");
            isDead = true;
            _playerAttack.gameObject.SetActive(false);
            _animator.SetTrigger(IsDead);
        }
        
        // Destroy all the heart UI when player's health is updated.
        foreach (Transform child in _healthParent)
        {
            Destroy(child.gameObject);
        }
        
        // Instantiate the heart UI based on player's health.
        for (var i = 0; i < playerInfo.health; i++)
        {
            Instantiate(heart, _healthParent);
        }
    }
        
    private void StopHit()
    {
        isHit = false;
    }

    private void OnInteract()
    {
        if (!canInteract || isPause || DialogueManager.isConversationActive || _inventoryActive) return;
        
        Debug.Log("Interact");
        // Do something when 'E' is pressed
        if (_interactableObject != null)
            _interactableObject.Interact();
    }
    
    private void OnInventory()
    {
        if (isPause || DialogueManager.isConversationActive) return;
        Debug.Log("Inventory");
        _menuController.InventoryControl();
    }
    
    // When player do Left-Click, set canAttack to false so that player can't attack again until the cooldown is over.
    // Cooldown was handle by AttackCooldown() coroutine which is called in CallShortRangeAttack().
    private void OnAttackCloseRange()
    {
        if (!canCloseAttack || isAttacking || isHit || isPause || DialogueManager.isConversationActive || _inventoryActive) return;
        
        canCloseAttack = false;
        
        // Start the attack animation, and call CallShortRangeAttack() on the specified time in the animation
        _animator.SetTrigger(IsAttack);
        Debug.Log("Attack Close Range");
        
        // AttackCooldown() coroutine is call to set canAttack to true after the cooldown is over. (Cooldown is started when the attack animation is played at first frame)
        StartCoroutine(CloseAttackCooldown());
    }
        
    // CallShortRangeAttack() is called in Player_Attack animation event.
    // This function will call Attack() function in PlayerAttack.cs script.
    private void CallShortRangeAttack()
    {
        if (_closeRangeAttackTimer > 0) return;
        
        Debug.Log("Call Close Range Attack");
        _playerAttack.CloseAttack();
        _closeRangeAttackTimer = closeAttackCooldown;

    }
    
    // Cooldown for close range attack is set in closeAttackCooldown variable.
    private IEnumerator CloseAttackCooldown()
    {
        yield return new WaitForSeconds(closeAttackCooldown);
        canCloseAttack = true;
    }
    
    private void OnAttackLongRange()
    {
        if (!infiniteArrow)
        {
            var arrow = inventoryManager.GetItem("Arrow");
            Debug.Log("Arrow Available: " + inventoryManager.CheckItem(arrow) + ", Amount: " + inventoryManager.CountItem(arrow));
            if (inventoryManager.CountItem(arrow) <= 0)
                return;
        }
        if (!canLongAttack || isAttacking || isHit || isPause || DialogueManager.isConversationActive || _inventoryActive) return;
        
        canLongAttack = false;
        _bowAttack = true;
        
        // Calculate angle towards mouse position
        var angle = Utility.AngleTowardsMouse(transform.position);
        var direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        
        isAttacking = true;
            
        // Update animator parameters
        _bowDirection = direction;
        
        _animator.SetTrigger(IsAttackBow);
        Debug.Log("Attack Long Range");

        StartCoroutine(LongAttackCooldown());
    }
    
    private void CallLongRangeAttack()
    {
        if (_longRangeAttackTimer > 0) return;
        
        inventoryManager.RemoveItem(inventoryManager.GetItem("Arrow"));
        Debug.Log("Call Long Range Attack");
        _playerAttack.LongAttack();
        _longRangeAttackTimer = longAttackCooldown;
        _bowAttack = false;
    }
    
    private IEnumerator LongAttackCooldown()
    {
        yield return new WaitForSeconds(longAttackCooldown);
        canLongAttack = true;
    }

    private void OnPause()
    {
        _menuController.PauseScript();
    }
    
    private void OnMoneyChanged()
    {
        _moneyText.text = playerInfo.money.ToString();
    }

    private void GameOver()
    {
        _menuController.Pause();
    }
    
    private void SetImmune()
    {
        immune = true;
    }
    
    private void RemoveImmune()
    {
        immune = false;
    }
}
