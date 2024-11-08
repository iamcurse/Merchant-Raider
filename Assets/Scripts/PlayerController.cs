using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [ShowOnly][SerializeField] private bool isInRange;
    [SerializeField] private float speed = 1f;
    
    private Vector2 _movementInput;
    private Rigidbody2D _rigidBody2D;
    private Animator _animator;
    
    
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");


    private PlayerInput _playerInput;
    private InputAction _move;
    private InputAction _interact;
    private InputAction _attackCloseRange;
    private InputAction _attackLongRange;

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
        _movementInput = _move.ReadValue<Vector2>();
        _rigidBody2D.linearVelocity = _movementInput * speed;
    }

    private void Animate()
    {
        if (_movementInput != Vector2.zero)
        {
            _animator.SetFloat(MoveX, _movementInput.x);
            _animator.SetFloat(MoveY, _movementInput.y);
            _animator.Play("Base Layer.Player_Walk");
        } else
        {
            _animator.Play("Base Layer.Player_Idle");
        }
        
    }
}
