using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private KeyCode attackKey = KeyCode.J;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;

    private bool isAttacking;
    private bool hasHitThisAttack;

    private static readonly int IsAttackingHash = Animator.StringToHash("Isattacking");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (attackPoint == null)
            attackPoint = transform;
    }

    private void Update()
    {
        if (isAttacking)
            return;

        if (Input.GetKeyDown(attackKey))
            StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;
        hasHitThisAttack = false;

        if (animator != null)
            animator.SetBool(IsAttackingHash, true);
    }

    /// <summary>
    /// 在攻击动画命中帧添加 Animation Event，调用此方法。
    /// </summary>
    public void OnHit()
    {
        if (hasHitThisAttack)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

        foreach (Collider2D hit in hits)
        {
            if (hit.isTrigger || hit.transform.IsChildOf(transform) || hit.gameObject == gameObject)
                continue;

            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth == null)
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null && damagedEnemies.Add(enemyHealth))
                enemyHealth.TakeDamage(damage);
        }

        hasHitThisAttack = true;
    }

    /// <summary>
    /// 在攻击动画结束帧添加 Animation Event，调用此方法。
    /// </summary>
    public void AttackEnd()
    {
        isAttacking = false;
        hasHitThisAttack = false;

        if (animator != null)
            animator.SetBool(IsAttackingHash, false);
    }
}
