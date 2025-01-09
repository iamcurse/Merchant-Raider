using System.Collections;
using UnityEngine;
using Pathfinding;

namespace Enemy
{
    public class GolemController : EnemyBase
    {
        private PlayerController _player;
    
        [ShowOnly][SerializeField] private int health;
        
        private static readonly int IsMoving = Animator.StringToHash("isWalking");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int IsHit = Animator.StringToHash("IsHit");
        private static readonly int IsDead = Animator.StringToHash("IsDead");


        [SerializeField] private Transform target;
        [ShowOnly][SerializeField] private bool lineOfSight;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float speed = 1f;
        [SerializeField] private int chasingRange = 5;
        private const float NextWaypointDistance = 3f;
        [SerializeField] private float chaseDuration = 4f;
        
        private Path _path;
        private int _currentWaypoint;
        private bool _reachedEndOfPath;
        private bool _isFinish;
        private Seeker _seeker;
        private Rigidbody2D _rigidbody2D;
        
        private RaycastHit2D _hit;
        private Coroutine _chaseCoroutine;
        private float ConvertChaseRange => (2 *chasingRange - 1) * 0.08f;
        
        [ShowOnly][SerializeField] public bool isAttack;
        [SerializeField] private float attackCooldown = 1f;
    
        [SerializeField] private EnemyInfo enemyInfo;
        [ShowOnly] public bool isGettingHit;
    
        private EnemyAttackRange _enemyAttackRange;
        private BoxCollider2D _attackRange;
        private float _attackRangeOffSetX;
        private float _attackRangeOffSetY;
        private float _attackRangeSizeX;
        private float _attackRangeSizeY;

        private SpriteRenderer _shadow;
        
        private void Awake()
        {
            // Find the Player object and assign it to the target instead of FindWithTag
            _player = PlayerController.Instance;
            if (_player == null)
                Debug.LogError("Player not found");

            if (!target)
                target = _player.transform;
        
            _seeker = GetComponent<Seeker>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            _isFinish = true;
            isAttack = false;
            health = enemyInfo.maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();
            _attackRange = transform.GetChild(0).GetComponent<BoxCollider2D>();
            _shadow = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _enemyAttackRange = GetComponentInChildren<EnemyAttackRange>();
        }
        
        private void Start()
        {
            InvokeRepeating(nameof(UpdatePath), 0f, 0.2f);
            Physics2D.queriesStartInColliders = false;
            
            _attackRangeOffSetX = _attackRange.offset.x;
            _attackRangeOffSetY = _attackRange.offset.y;
            _attackRangeSizeX = _attackRange.size.x;
            _attackRangeSizeY = _attackRange.size.y;
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
            AttackRangeOffset();
            ZOrder();
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
            var hit = Physics2D.Raycast(transform.position, _player.transform.position - transform.position, ConvertChaseRange, collisionMask);
            lineOfSight = hit.collider != null && hit.collider.CompareTag("Player Raycast");
            return lineOfSight;
        }
        
        private void OnPathComplete(Path path)
        {
            if (path.error) return;
            _path = path;
            _currentWaypoint = 1;
        }
        
        private void Move()
        {
            if (_reachedEndOfPath) return;
        
            var direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
            var force = direction * (speed /* Time.deltaTime*/);
        
            if (isDead || isGettingHit)
            {
                force = Vector2.zero;
            }
        
            _rigidbody2D.linearVelocity = force;
        
            var distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);
            if (distance < NextWaypointDistance)
            {
                _currentWaypoint++;
            }
        }
        
        private void Animate()
        {
            if (isAttack && !isGettingHit && !isDead) return;

            // If enemy is attacking, getting hit, or dead, it will not play the movement animation
            animator.SetBool(IsMoving, !_isFinish);

            animator.SetFloat(MoveX, _rigidbody2D.linearVelocity.x);
            animator.SetFloat(MoveY, _rigidbody2D.linearVelocity.y);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // If player is still staying in Attack Range of Enemy, Enemy will keep trying to attack player using TryAttack() method
            if (!other.CompareTag("Player Hit Box")) return;
            TryAttack();
        }
        
        private void TryAttack()
        {
            // If Enemy is already attacking, getting hit, or dead, it will not attack player
            if (isAttack || isGettingHit || isDead) return;
        
            // Perform a raycast to check for obstacles
            var direction = (_player.transform.position - transform.position).normalized;
            var distance = Vector2.Distance(transform.position, _player.transform.position);
            var hit = Physics2D.Raycast(transform.position, direction, distance, collisionMask);

            // If the raycast hits an obstacle, skip the attack
            if (hit.collider != null && !hit.collider.CompareTag("Player")) return;
        
            Debug.Log("Attacking player");
            // Set isAttack to true, so that the enemy will not attack player again until the attack cooldown is over
            isAttack = true;
            // Start the attack animation, and call CallAttack() on the specified time in the animation
            animator.SetTrigger(Attack);
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
        
        private void OnDrawGizmos()
        {
            Gizmos.color = !lineOfSight ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, ConvertChaseRange);
        }
        
        private void AttackRangeOffset()
        {
            if (animator.GetFloat(MoveX) < 0 && animator.GetFloat(MoveY) < 0)  //Front Left
                _attackRange.offset = new Vector2(_attackRangeOffSetX - 0.62f, _attackRangeOffSetY);
            else if (animator.GetFloat(MoveX) > 0 && animator.GetFloat(MoveY) < 0) //Front Right
                _attackRange.offset = new Vector2(_attackRangeOffSetX, _attackRangeOffSetY);
            else if (animator.GetFloat(MoveX) < 0 && animator.GetFloat(MoveY) > 0) //Back Left
            {
                _attackRange.offset = new Vector2(_attackRangeOffSetX - 0.62f, _attackRangeOffSetY + 0.18f);
                _attackRange.size = new Vector2(_attackRangeSizeX, _attackRangeSizeY + 0.28f); 
            }
            else if (animator.GetFloat(MoveX) > 0 && animator.GetFloat(MoveY) > 0) //Back Right
            {
                _attackRange.offset = new Vector2(_attackRangeOffSetX, _attackRangeOffSetY + 0.18f);
                _attackRange.size = new Vector2(_attackRangeSizeX, _attackRangeSizeY + 0.28f);
            }

            _shadow.flipX = !(animator.GetFloat(MoveX) > 0);
        }
        
        public override void GetHit()
        {
            health--;
            isGettingHit = true;
            Debug.Log("Enemy gets hit");
            // Start the get hit animation
            
            if (health % 5 == 0)
                animator.SetBool(IsHit, true);
        
            _isFinish = false;
            // Start chasing the player when hit
            if (_chaseCoroutine != null)
            {
                StopCoroutine(_chaseCoroutine);
            }
            _chaseCoroutine = StartCoroutine(ChasePlayer());
        }
        
        private void CheckDead()
        {
            if (health > 0) return;
        
            // If enemy health is <= 0, set isDead to true, stop the rigidbody movement, disable the attack range collider, and start the dead animation
            isDead = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _enemyAttackRange.gameObject.SetActive(false);
            animator.SetTrigger(IsDead);
        }   
        
        private void DestroyThis()
        {
            Destroy(gameObject);
        }
        
        private void StopGetHit()
        {
            animator.SetBool(IsHit, false);
        }

        private void ZOrder()
        {
            if (_player.transform.position.y > transform.position.y)
            {
                GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            else
            {
                GetComponent<SpriteRenderer>().sortingOrder = -1;
            }
        }
    }
}
