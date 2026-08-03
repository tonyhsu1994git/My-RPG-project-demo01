using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float aggroRange = 8f;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Transform player;
    private bool isAggroed;
    private bool isAttacking;
    private float nextAttackTime;

    private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        SetAnimatorState(idle: true);
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (player == null || !isAggroed)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > aggroRange)
        {
            LoseAggro();
            return;
        }

        if (distance <= attackRange && CanAttack())
        {
            rb.velocity = Vector2.zero;
            StartAttack();
            return;
        }

        if (distance > attackRange)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.velocity = direction * moveSpeed;
            SetAnimatorState(walking: true);
            UpdateFacing(direction.x);
            return;
        }

        rb.velocity = Vector2.zero;
        SetAnimatorState(idle: true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.transform;
        isAggroed = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.transform;
        isAggroed = true;
    }

    private void LoseAggro()
    {
        isAggroed = false;
        player = null;
        rb.velocity = Vector2.zero;

        if (!isAttacking)
            SetAnimatorState(idle: true);
    }

    private bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    private void StartAttack()
    {
        if (isAttacking || !CanAttack())
            return;

        isAttacking = true;
        nextAttackTime = Time.time + attackInterval;
        rb.velocity = Vector2.zero;

        Vector2 direction = player.position - transform.position;
        if (direction.x != 0f)
            UpdateFacing(direction.x);

        SetAnimatorState(attacking: true);
    }

    /// <summary>
    /// 在攻击动画命中帧添加 Animation Event，调用此方法。
    /// </summary>
    public void OnAttackHit()
    {
        if (player == null)
            return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);
    }

    /// <summary>
    /// 在攻击动画结束帧添加 Animation Event，调用此方法。
    /// </summary>
    public void OnAttackEnd()
    {
        isAttacking = false;
        SetAnimatorState(idle: true);
    }

    private void SetAnimatorState(bool idle = false, bool walking = false, bool attacking = false)
    {
        if (animator == null)
            return;

        animator.SetBool(IsIdleHash, idle);
        animator.SetBool(IsWalkingHash, walking);
        animator.SetBool(IsAttackingHash, attacking);
    }

    private void UpdateFacing(float directionX)
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.flipX = directionX < 0f;
    }
}
