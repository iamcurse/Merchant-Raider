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
        Animate();
        
        if (gameOver && _a == 0) GameOver();
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
        if (!canMove || isDead || isPause || DialogueManager.isConversationActive) return;
        _movementInput = _move.ReadValue<Vector2>();
        _rigidBody2D.linearVelocity = _movementInput * speed;
        Debug.Log(_movementInput);
    }

    private void Animate()
    {
        if (isAttacking || isDead) return;
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
        playerInfo.health--;
        Debug.Log("Player gets hit");
        if (playerInfo.health != 0)
            _animator.Play("Player_Hit", -1, 0f);
        OnHealthChanged();
    }
    
    public void GetHit(int damage)
    {
        //Public method to call when player gets hit, when player gets hit, player can't attack
        isHit = true;
        playerInfo.health -= damage;
        Debug.Log("Player gets hit");
        if (playerInfo.health != 0)
            _animator.Play("Player_Hit", -1, 0f);
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
    
    private void OnHealthChanged()
    {
        if (playerInfo.health == 0)
        {
            Debug.Log("Player is dead");
            _animator.Play("Player_Death");
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
}
