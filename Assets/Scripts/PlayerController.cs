using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canAttack = true;
    
    private Vector2 _movementInput;
    private Rigidbody2D _rigidBody2D;
    [ShowOnly][SerializeField] private bool isAttacking;
    [HideInInspector] public Vector2 attackDirection;
    [ShowOnly][SerializeField] private bool isHit;

    private Animator _animator;
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");


    private PlayerInput _playerInput;
    private InputAction _move;
    private InputAction _interact;
    private InputAction _attackCloseRange;
    private InputAction _attackLongRange;

    public bool isDead;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _move = _playerInput.Player.Move;
        _interact = _playerInput.Player.Interact;
        _attackCloseRange = _playerInput.Player.AttackCloseRange;
        _attackLongRange = _playerInput.Player.AttackLongRange;
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
    }
    
    private void OnDisable()
    {
        _move.Disable();
        _interact.Disable();
        _attackCloseRange.Disable();
        _attackLongRange.Disable();
    }

    private void Move()
    {
        if (!canMove || isDead) return;
        _movementInput = _move.ReadValue<Vector2>();
        _rigidBody2D.linearVelocity = _movementInput * speed;
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
        if (!canInteract) return;
        Debug.Log("Interact");
        
        //Do something when 'E' is pressed
    }
    
    private void OnAttackCloseRange()
    {
        if (!canAttack || isHit) return;
        _animator.Play("Player_Attack");
        Debug.Log("Attack Close Range");
        
        //Do something when Left Click
    }
    
    private void OnAttackLongRange()
    {
        if (!canAttack || isHit) return;
        Debug.Log("Attack Long Range");
        
        //Do something when Right Click
    }
}
