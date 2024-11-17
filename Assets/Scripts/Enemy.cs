using System.Collections;
using UnityEngine;
using Pathfinding;
using Vector2 = UnityEngine.Vector2;

public class Enemy : MonoBehaviour
{
    static readonly int IsMoving = Animator.StringToHash("isWalking");
    private static readonly int Attack1 = Animator.StringToHash("isAttacking");
    private static readonly int IsHit = Animator.StringToHash("isHit");
    private static readonly int IsDead = Animator.StringToHash("isDead");
    [SerializeField] private Transform target;
    [ShowOnly][SerializeField] private bool lineOfSight;
    [ShowOnly] public bool playerInAttackRange;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float speed = 75f;
    [SerializeField] private int chasingRange = 5;
    private readonly float _nextWaypointDistance = 3f;
    private Transform _player;
    [SerializeField] private float chaseDuration = 4f;
    
    private Path _path;
    private int _currentWaypoint;
    private bool _reachedEndOfPath;
    private bool _isFinish;
    private Seeker _seeker;
    private Rigidbody2D _rigidbody2D;

    private Animator _animator;
    
    private RaycastHit2D _hit;
    private Coroutine _chaseCoroutine;
    private float ConvertChaseRange => (2 *chasingRange - 1) * 0.08f;

    [HideInInspector] public bool isAttack;
    [SerializeField] private float attackCooldown = 1f;
    private bool _isCooldown;
    
    [SerializeField] private EnemyInfo enemyInfo;
    public bool isGettingHit;
    public bool isDead;
    
    private PlayerAttack _playerAttack;
    
    private void Awake()
    {
        _seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindWithTag("Player").transform;
        _isFinish = true;
        isAttack = false;
        enemyInfo.health = enemyInfo.maxHealth;
        //_playerAttack = _player.GetChild(0).gameObject.GetComponent<PlayerAttack>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdatePath), 0f, 0.2f);
        Physics2D.queriesStartInColliders = false;
    }
    
    private void UpdatePath()
    {
        if (!_seeker.IsDone()) return;

        var distanceToPlayer = Vector2.Distance(_rigidbody2D.position, target.position);
        if (!(distanceToPlayer <= ConvertChaseRange) || !CheckLineOfSight()) return;
        _isFinish = false;
        if (_chaseCoroutine != null)
        {
            StopCoroutine(_chaseCoroutine);
        }
        _chaseCoroutine = StartCoroutine(ChasePlayer());
    }
    
    private IEnumerator ChasePlayer()
    {
        while (true)
        {
            _seeker.StartPath(_rigidbody2D.position, target.position, OnPathComplete);
            yield return new WaitForSeconds(0.2f); // Update path every 0.2 seconds

            var distanceToPlayer = Vector2.Distance(_rigidbody2D.position, target.position);
            if (!(distanceToPlayer > ConvertChaseRange)) continue;
            var chaseEndTime = Time.time + chaseDuration;
            while (Time.time < chaseEndTime)
            {
                _seeker.StartPath(_rigidbody2D.position, target.position, OnPathComplete);
                yield return new WaitForSeconds(0.2f); // Update path every 0.2 seconds

                if (CheckLineOfSight())
                {
                    break; // Continue chasing if player is seen again
                }
            }

            if (CheckLineOfSight()) continue;

            // Finish the current path before stopping
            while (!_reachedEndOfPath)
            {
                yield return null;
            }

            _path = null; // Stop chasing if player is not seen within the duration
            _isFinish = true;
            yield break;
        }
    }
    
    private bool CheckLineOfSight()
    {
        var hit = Physics2D.Raycast(transform.position, _player.position - transform.position, ConvertChaseRange, collisionMask);
        lineOfSight = hit.collider != null && hit.collider.CompareTag("Player");
        return lineOfSight;
    }

    private void FixedUpdate()
    {
        if (_path == null) return;
        _reachedEndOfPath = _currentWaypoint >= _path.vectorPath.Count;
        
        Move();
    }
    
    private void Update()
    {
        Animate();
    }

    private void OnPathComplete(Path path)
    {
        if (path.error) return;
        _path = path;
        _currentWaypoint = 0;
    }

    private void Move()
    {
        if (_reachedEndOfPath) return;
        var direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
        var force = direction * (speed * Time.deltaTime);

        if (isDead)
        {
            force = Vector2.zero;
        }
        _rigidbody2D.AddForce(force);

        var distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);
        if (distance < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }

        transform.localScale = _rigidbody2D.linearVelocity.x switch
        {
            <= 0.01f => new Vector3(-1f, 1f, 1f),
            >= -0.01f => new Vector3(1f, 1f, 1f),
            _ => transform.localScale
        };
    }

    private void Animate()
    {
        if (isAttack && !isGettingHit && !isDead)
        {
            _animator.SetBool(Attack1, true);
            return;
        }
        else
        {
            _animator.SetBool(Attack1, false);
        }

        _animator.SetBool(IsMoving, !_isFinish);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = !lineOfSight ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, ConvertChaseRange);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _isCooldown) return;
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        _isCooldown = true;
        isAttack = true;
        yield return new WaitForSeconds(attackCooldown);
        _isCooldown = false;
    }

    public void TryAttack()
    {
        Debug.Log("Attacking player");
        if (playerInAttackRange)
        {
            _player.gameObject.GetComponent<PlayerController>().GetHit();
        }
    }

    public void GetHit()
    {
        enemyInfo.health--;
        isGettingHit = true;
        Debug.Log("Enemy gets hit");
        if (enemyInfo.health != 0)
        {
            _animator.SetBool(IsHit, true);
        }
        else
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            _animator.SetTrigger(IsDead);
            isDead = true;
        }
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
    
    private void StopGetHit()
    {
        _animator.SetBool(IsHit, false);
    }
}
