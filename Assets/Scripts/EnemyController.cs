using System.Collections;
using UnityEngine;
using Pathfinding;
using Vector2 = UnityEngine.Vector2;

public class EnemyController : MonoBehaviour
{
    [ShowOnly][SerializeField] private int health;
    private static readonly int IsMoving = Animator.StringToHash("isWalking");
    private static readonly int IsHit = Animator.StringToHash("isHit");
    private static readonly int IsDead = Animator.StringToHash("isDead");
    private static readonly int Attack = Animator.StringToHash("Attack");
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
    private SpriteRenderer _spriteRenderer;

    private Animator _animator;
    
    private RaycastHit2D _hit;
    private Coroutine _chaseCoroutine;
    private float ConvertChaseRange => (2 *chasingRange - 1) * 0.08f;

    [ShowOnly][SerializeField] public bool isAttack;
    [SerializeField] private float attackCooldown = 1f;
    
    [SerializeField] private EnemyInfo enemyInfo;
    [ShowOnly] public bool isGettingHit;
    [ShowOnly] public bool isDead;
    
    private PlayerAttack _playerAttack;
    private EnemyAttackRange _enemyAttackRange;
    
    private void Awake()
    {
        if (!target)
        {
            target = GameObject.Find("Player").transform;
        }
        _seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindWithTag("Player").transform;
        _isFinish = true;
        isAttack = false;
        health = enemyInfo.maxHealth;
        _enemyAttackRange = transform.GetChild(0).GetComponent<EnemyAttackRange>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        RandomFlip();
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
        if (_chaseCoroutine != null)
        {
            StopCoroutine(_chaseCoroutine);
        }
        _chaseCoroutine = StartCoroutine(ChasePlayer());
    }
    
    private IEnumerator ChasePlayer()
    {
        _isFinish = false;
        
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
            _rigidbody2D.linearVelocity = Vector2.zero;
            yield break;
        }
    }
    
    private bool CheckLineOfSight()
    {
        var hit = Physics2D.Raycast(transform.position, _player.position - transform.position, ConvertChaseRange, collisionMask);
        lineOfSight = hit.collider != null && hit.collider.CompareTag("Player Raycast");
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
        ZOrder();
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

        _spriteRenderer.flipX = _rigidbody2D.linearVelocity.x switch
        {
            <= 0.01f => true,
            >= -0.01f => false,
            _ => _spriteRenderer.flipX
        };
    }

    private void Animate()
    {
        if (isAttack && !isGettingHit && !isDead) return;

        // If enemy is attacking, getting hit, or dead, it will not play the movement animation
        _animator.SetBool(IsMoving, !_isFinish);
    }
    
    // Draw Gizmos to show the chase range of the enemy
    private void OnDrawGizmos()
    {
        Gizmos.color = !lineOfSight ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, ConvertChaseRange);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // If player is still staying in Attack Range of Enemy, Enemy will keep trying to attack player using TryAttack() method
        if (!other.CompareTag("Player")) return;
        
        TryAttack();
    }

    private void TryAttack()
    {
        // If Enemy is already attacking, getting hit, or dead, it will not attack player
        if (isAttack || isGettingHit || isDead) return;
        
        Debug.Log("Attacking player");
        // Set isAttack to true, so that the enemy will not attack player again until the attack cooldown is over
        isAttack = true;
        // Start the attack animation, and call CallAttack() on the specified time in the animation
        _animator.SetTrigger(Attack);
        // Start the attack cooldown timer at the first frame of the attack animation
        StartCoroutine(AttackCooldown());
    }
    
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttack = false;
    }
    
    private void CallAttack()
    {
        // Check if player is still in attack range of enemy when the attack animation is played then call HitPlayer() method if player is still in range
        if (!playerInAttackRange) return;
        _player.GetComponent<PlayerController>().GetHit();
    }

    // GetHit() method is called when player attack enemy
    public void GetHit()
    {
        health--;
        isGettingHit = true;
        Debug.Log("Enemy gets hit");
        // Start the get hit animation
        _animator.SetBool(IsHit, true);
        
        _isFinish = false;
        // Start chasing the player when hit
        if (_chaseCoroutine != null)
        {
            StopCoroutine(_chaseCoroutine);
        }
        _chaseCoroutine = StartCoroutine(ChasePlayer());
    }     
    
    // CheckDead() method is called at the second last frame of the get hit animation to check if the enemy health is <= 0
    private void CheckDead()
    {
        if (health > 0) return;
        
        // If enemy health is <= 0, set isDead to true, stop the rigidbody movement, disable the attack range collider, and start the dead animation
        isDead = true;
        _rigidbody2D.linearVelocity = Vector2.zero;
        _enemyAttackRange.gameObject.SetActive(false);
        _animator.SetTrigger(IsDead);
    }   
    
    // StopGetHit() method is called at the last frame of the get hit animation to set IsHit parameter in animator to false
    private void StopGetHit()
    {
        _animator.SetBool(IsHit, false);
    }

    // DestroyThis() method is called at the last frame of the dead animation to destroy the enemy object
    private void DestroyThis()
    {
        Destroy(gameObject);
    }
    
    private void ZOrder()
    {
        if (_player.position.y > transform.position.y)
        {
            GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else
        {
            GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
    }
    
    private void RandomFlip()
    {
        var random = Random.Range(0, 2);
        _spriteRenderer.flipX = random == 0;
    }
}
