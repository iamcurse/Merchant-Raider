using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    
    [SerializeField] private float playerSpeed = 1f;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canInteract;
    
    [SerializeField] private bool canCloseAttack = true;
    [EnabledIf("canCloseAttack")]
    [SerializeField] private float closeAttackCooldown = 1f;
    [EnabledIf("canCloseAttack")]
    [SerializeField] private float closeRangeAttackMoveSpeed = 0.5f;
    
    [SerializeField] public bool canLongAttack = true;
    [EnabledIf("canLongAttack")]
    [SerializeField] private float longAttackCooldown = 1f;
    [EnabledIf("canLongAttack")]
    [SerializeField] private float longRangeAttackMoveSpeed = 0.5f;
    
    [EnabledIf("canLongAttack")]
    [SerializeField] private bool infiniteArrow;
    [EnabledIf("canLongAttack")] 
    public float arrowSpeed = 3f;
    
    // ReSharper disable once NotAccessedField.Global
    [ShowOnly] public bool enemyInAttackRange;
    
    [SerializeField] private bool canRoll;
    [EnabledIf("canRoll")]
    [SerializeField] private float rollDistance = 0.32f; // Fixed roll distance
    [EnabledIf("canRoll")]
    [SerializeField] private float rollSpeed = 2.5f;
    [EnabledIf("canRoll")]
    [SerializeField] private float rollCooldown = 1.5f;

    [SerializeField] private bool playerImmune;
    
    private bool _isRolling;
    
    private bool _immune;
    
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
    private static readonly int IsRoll = Animator.StringToHash("IsRoll");

    private PlayerInputController _playerInput;
    private InputAction _move;
    private InputAction _interact;
    private InputAction _attackCloseRange;
    private InputAction _attackLongRange;

    [ShowOnly] public bool isDead;
    [ShowOnly] public bool gameOver;

    private UIController _uiController;
    private bool _inventoryActive;
    [HideInInspector] public InventoryManager inventoryManager;
    [ShowOnly] public bool isPause;
    
    private InteractableObject _interactableObject;

    public PlayerInfo playerInfo;
    
    private PlayerAttack _playerAttack;
    
    public Inventory inventory;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError("Multiple Player instances found! Destroying duplicate.");
            Destroy(gameObject);
        }
        
        _playerInput = new PlayerInputController();
        _move = _playerInput.Player.Move;
        _interact = _playerInput.Player.Interact;
        _attackCloseRange = _playerInput.Player.AttackCloseRange;
        _attackLongRange = _playerInput.Player.AttackLongRange;
        
        _playerAttack = GetComponentInChildren<PlayerAttack>();
        
        _uiController = UIController.Instance;
        inventoryManager = InventoryManager.Instance;
        
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
        RefreshHealth();
    }

    private void Start()
    {
        RefreshHealth();
        OnMoneyChanged();
    }
    
    private void Update()
    {
        Move();
        Animate();
        _inventoryActive = _uiController.IsInventoryActive();
        
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
        Lua.RegisterFunction(nameof(RefreshHealth), this, SymbolExtensions.GetMethodInfo(() => RefreshHealth()));
        
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
        Lua.UnregisterFunction(nameof(RefreshHealth));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable Object")) return;
        
       _interactableObject = other.GetComponent<InteractableObject>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Interactable Object")) return;
        
        _interactableObject = null;
    }

    private void Move()
    {
        if (_isRolling) return;
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
        
        if (stateInfo.IsName("Player_Roll"))
        {
            _isRolling = stateInfo.normalizedTime < 1;
        } else
        {
            _isRolling = false;
        }
        
        // Lock Attack animation during movement
        if (!isAttacking && !_isRolling)
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
        if (_immune || isDead || playerImmune) return;
        _immune = true;
        isHit = true;
        playerInfo.TakeDamage();
        Debug.Log("Player gets hit");
        
        // If player's health is below 0, it will skip the hit animation and go straight to death animation.
        if (!playerInfo.IsDead())
            _animator.SetTrigger(IsHit);
        OnHealthChanged();
    }
    
    public void GetHit(int damage)
    {
        if (_immune || isDead || playerImmune) return;
        _immune = true;
        isHit = true;
        playerInfo.TakeDamage(damage);
        Debug.Log("Player gets hit");
        if (!playerInfo.IsDead())
            _animator.SetTrigger(IsHit);
        OnHealthChanged();
    }
    
    // When damage calculation is done, OnHealthChanged() function is called to update the health UI.
    private void OnHealthChanged()
    {
        // Death animation is played when player's health is below 0.
        if (playerInfo.IsDead())
        {
            _rigidBody2D.linearVelocity = Vector2.zero;
            Debug.Log("Player is dead");
            isDead = true;
            _playerAttack.gameObject.SetActive(false);
            _animator.SetTrigger(IsDead);
        }
        
        _uiController.UpdateHealth(playerInfo.Health);
    }
    
    private void OnMoneyChanged()
    {
        _uiController.UpdateMoney(playerInfo.Money);
    }
        
    private void StopHit()
    {
        isHit = false;
    }

    private void OnInteract()
    {
        if (!canInteract || isPause || DialogueManager.isConversationActive || _inventoryActive || _isRolling) return;
        
        Debug.Log("Interact");
        // Do something when 'E' is pressed
        if (_interactableObject != null)
            _interactableObject.Interact();
    }
    
    private void OnInventory()
    {
        if (isPause || DialogueManager.isConversationActive) return;
        Debug.Log("Inventory");
        _uiController.InventoryControl();
    }
    
    // When player do Left-Click, set canAttack to false so that player can't attack again until the cooldown is over.
    // Cooldown was handle by AttackCooldown() coroutine which is called in CallShortRangeAttack().
    private void OnAttackCloseRange()
    {
        if (!canCloseAttack || isAttacking || isHit || isPause || DialogueManager.isConversationActive || _inventoryActive || _isRolling) return;
        
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
        if (!canLongAttack || isAttacking || isHit || isPause || DialogueManager.isConversationActive || _inventoryActive || _isRolling) return;
        
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
        
        if (!infiniteArrow)
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
        if(!DialogueManager.isConversationActive)
            _uiController.PauseScript();
    }

    private void GameOver()
    {
        _uiController.GameOver();
    }
    
    private void SetImmune()
    {
        _immune = true;
    }
    
    private void RemoveImmune()
    {
        _immune = false;
    }

    private void RefreshHealth()
    {
        playerInfo.RestoreHealth();
        OnHealthChanged();
    }
    
    private Vector2 _rollDirection = new Vector2(0, -1);
    private float _distanceTraveled;

    private void OnRoll()
    {
        if (!canRoll) return;
        Debug.Log("Roll");

        canRoll = false;
        _isRolling = true;

        // Reset distance traveled on each roll
        _distanceTraveled = 0f;

        // Get the roll direction from the animator (or movement input)
        _rollDirection = new Vector2(_animator.GetFloat(MoveX), _animator.GetFloat(MoveY));
        if (_rollDirection == Vector2.zero)
        {
            _rollDirection = new Vector2(0, -1); // Default to downward if no input
        }
        Debug.Log($"Roll Direction: {_rollDirection}");
        
        SetImmune();  // Prevent damage during the roll

        _animator.SetTrigger(IsRoll);  // Trigger roll animation

        // Start the roll movement coroutine
        StartCoroutine(RollCoroutine());
    }
    
    private IEnumerator RollCoroutine()
    {
        // While the player hasn't traveled the full roll distance
        while (_distanceTraveled < rollDistance)
        {
            // Calculate the step based on the roll speed and time
            var step = rollSpeed * Time.deltaTime;

            // Move the player in the roll direction
            _rigidBody2D.MovePosition(_rigidBody2D.position + _rollDirection * step);

            // Update the distance traveled
            _distanceTraveled += step;

            // Wait until the next fixed frame
            yield return null;
        }

        // Stop the roll once the distance is covered
        EndRoll();
    }
    
    private void EndRoll()
    {
        // Reset rolling state
        _isRolling = false;

        // Additional logic to handle after the roll ends (e.g., cooldown)
        StartCoroutine(RollCooldown());
    }
    
    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }
}
