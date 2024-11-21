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

    private void FixedUpdate()
    {
        if (gameOver && _a == 0) GameOver();
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
        if (isDead) return;
        if (_movementInput != Vector2.zero)
        {
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
        
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Player_Attack"))
        {
            isAttacking = stateInfo.normalizedTime < 1;
        } else
        {
            isAttacking = false;
        }
    }

    public void GetHit()
    {
        isHit = true;
        playerInfo.health--;
        Debug.Log("Player gets hit");
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

    private void OnInteract()
    {
        if (!canInteract || isPause || DialogueManager.isConversationActive) return;
        
        Debug.Log("Interact");
        //Do something when 'E' is pressed
        _interactableObject.Interact();
    }
    
    private void OnAttackCloseRange()
    {
        //When player do Left-Click, set canAttack to false so that player can't attack again until the cooldown is over. Cooldown was handle by AttackCooldown() coroutine which is called in CallShortRangeAttack().
        if (!canAttack || isHit || isPause || DialogueManager.isConversationActive) return;
        
        canAttack = false;
        _animator.SetTrigger(IsAttack);
        Debug.Log("Attack Close Range");
        StartCoroutine(AttackCooldown());
    }
    
    private void OnAttackLongRange()
    {
        if (!canAttack || isHit || isPause || DialogueManager.isConversationActive) return;
        Debug.Log("Attack Long Range");
        
        //Do something when Right Click
        
    }

    private void OnPause()
    {
        _pauseUI.PauseScript();
    }
    
    private void OnHealthChanged()
    {
        if (playerInfo.health <= 0)
        {
            _rigidBody2D.linearVelocity = Vector2.zero;
            Debug.Log("Player is dead");
            isDead = true;
            _playerAttack.gameObject.SetActive(false);
            _animator.SetTrigger(IsDead);
        }
        
        foreach (Transform child in _healthParent)
        {
            Destroy(child.gameObject);
        }
        
        for (var i = 0; i < playerInfo.health; i++)
        {
            Instantiate(heart, _healthParent);
        }
    }
    
    private void OnMoneyChanged()
    {
        _moneyText.text = playerInfo.money.ToString();
    }

    private void GameOver()
    {
        _a += 1;
        _pauseUI.Pause();
    }
    
    private void CallShortRangeAttack()
    {
        //CallShortRangeAttack() is called in Player_Attack animation event. This function is called when the animation reach the frame where the player attack the enemy.
        //This function will call Attack() function in PlayerAttack.cs script.
        _playerAttack.Attack();
        //AttackCooldown() coroutine is called to set canAttack to true after the cooldown is over.
    }
    
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(closeAttackCooldown);
        canAttack = true;
    }
    
    private void StopHit()
    {
        isHit = false;
    }
}
