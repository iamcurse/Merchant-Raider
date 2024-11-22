using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float closeAttackCooldown = 1f;
    // ReSharper disable once NotAccessedField.Global
    [ShowOnly] public bool enemyInAttackRange;
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
    private static readonly int IsHit = Animator.StringToHash("isHit");
    private static readonly int IsAttack = Animator.StringToHash("isAttack");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int IsDead = Animator.StringToHash("isDead");

    private PlayerInput _playerInput;
    private InputAction _move;
    private InputAction _interact;
    private InputAction _attackCloseRange;
    private InputAction _attackLongRange;

    [ShowOnly] public bool isDead;
    [ShowOnly] public bool gameOver;

    private GameObject _ui;
    private PauseMenu _pauseUI;
    [ShowOnly] public bool isPause;
    
    private InteractableObject _interactableObject;

    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] private GameObject heart;
    private Transform _healthParent;
    private TMPro.TextMeshProUGUI _moneyText;

    private int _a;
    
    private PlayerAttack _playerAttack;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _move = _playerInput.Player.Move;
        _interact = _playerInput.Player.Interact;
        _attackCloseRange = _playerInput.Player.AttackCloseRange;
        _attackLongRange = _playerInput.Player.AttackLongRange;

        _ui = GameObject.Find("UI");
        _pauseUI = _ui.GetComponent<PauseMenu>();
        _healthParent = _ui.transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform;
        _moneyText = _ui.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        
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
        if (!canMove || isDead || isPause/* || DialogueManager.isConversationActive*/) return;
        
        _movementInput = _move.ReadValue<Vector2>();
        _rigidBody2D.linearVelocity = _movementInput * speed;
    }

    private void Animate()
    {
        // If player is dead, it will skip the movement animation.
        if (isDead) return;
        
        if (_movementInput != Vector2.zero)
        {
            // Lock Attack animation during movement
            if (!isAttacking)
            {
                _animator.SetFloat(MoveX, _movementInput.x);
                _animator.SetFloat(MoveY, _movementInput.y);

                _playerAttack.moveX = _movementInput.x;
                _playerAttack.moveY = _movementInput.y;
            }
            
            _animator.SetBool(IsMoving, true);
        } else
        {
            _animator.SetBool(IsMoving, false);
        }
        
        // Check if attack animation is still playing
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Player_Attack"))
        {
            isAttacking = stateInfo.normalizedTime < 1;
        } else
        {
            isAttacking = false;
        }
    }

    // GetHit() function is called when player gets hit by enemy.
    public void GetHit()
    {
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
        if (!canInteract || isPause || DialogueManager.isConversationActive) return;
        
        Debug.Log("Interact");
        // Do something when 'E' is pressed
        _interactableObject.Interact();
    }
    
    // When player do Left-Click, set canAttack to false so that player can't attack again until the cooldown is over.
    // Cooldown was handle by AttackCooldown() coroutine which is called in CallShortRangeAttack().
    private void OnAttackCloseRange()
    {
        if (!canAttack || isHit || isPause || DialogueManager.isConversationActive) return;
        
        canAttack = false;
        
        // Start the attack animation, and call CallShortRangeAttack() on the specified time in the animation
        _animator.SetTrigger(IsAttack);
        Debug.Log("Attack Close Range");
        
        // AttackCooldown() coroutine is call to set canAttack to true after the cooldown is over. (Cooldown is started when the attack animation is played at first frame)
        StartCoroutine(AttackCooldown());
    }
        
    // CallShortRangeAttack() is called in Player_Attack animation event.
    // This function will call Attack() function in PlayerAttack.cs script.
    private void CallShortRangeAttack()
    {
        _playerAttack.Attack();
    }
    
    // Cooldown for close range attack is set in closeAttackCooldown variable.
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(closeAttackCooldown);
        canAttack = true;
    }
    
    private void OnAttackLongRange()
    {
        if (!canAttack || isHit || isPause || DialogueManager.isConversationActive) return;
        Debug.Log("Attack Long Range");
        
        // Do something when Right Click
        
    }

    private void OnPause()
    {
        _pauseUI.PauseScript();
    }
    
    private void OnMoneyChanged()
    {
        _moneyText.text = playerInfo.money.ToString();
    }

    private void GameOver()
    {
        _pauseUI.Pause();
    }
}
