using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private KeyCode attackKey = KeyCode.J;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackAnimDuration = 0.65f;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;

    private bool isAttacking;
    private bool hasHitThisAttack;
    private int currentAttackId;
    private Coroutine attackResetCoroutine;

    public bool IsAttacking => isAttacking;

    private static readonly int IsAttackingHash = Animator.StringToHash("Isattacking");
    private static readonly int AttackStateHash = Animator.StringToHash("Attack");

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
        {
            TryFinishAttackByAnimProgress();
            return;
        }

        if (Input.GetKeyDown(attackKey))
            StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;
        hasHitThisAttack = false;
        currentAttackId++;

        if (animator != null)
        {
            animator.SetBool(IsAttackingHash, true);
            // 强制从头播放，避免已在 Attack 状态时动画不重播、AttackEnd 不触发
            animator.Play("Attack", 0, 0f);
        }

        if (attackResetCoroutine != null)
            StopCoroutine(attackResetCoroutine);
        attackResetCoroutine = StartCoroutine(AttackTimeoutFallback());
    }

    private void TryFinishAttackByAnimProgress()
    {
        if (animator == null)
            return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.shortNameHash == AttackStateHash && state.normalizedTime >= 0.99f)
            AttackEnd();
    }

    private IEnumerator AttackTimeoutFallback()
    {
        yield return new WaitForSeconds(attackAnimDuration);
        if (isAttacking)
            AttackEnd();
        attackResetCoroutine = null;
    }

    /// <summary>
    /// 技能打断或状态不同步时调用，重置攻击状态。
    /// </summary>
    public void ResetAttackState()
    {
        isAttacking = false;
        hasHitThisAttack = false;

        if (animator != null)
            animator.SetBool(IsAttackingHash, false);

        if (attackResetCoroutine != null)
        {
            StopCoroutine(attackResetCoroutine);
            attackResetCoroutine = null;
        }
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
            if (hit.isTrigger)
                continue;

            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth == null || !damagedEnemies.Add(enemyHealth))
                continue;

            enemyHealth.TakeDamage(damage, currentAttackId);
        }

        hasHitThisAttack = true;
    }

    /// <summary>
    /// 在攻击动画结束帧添加 Animation Event，调用此方法。
    /// </summary>
    public void AttackEnd()
    {
        if (!isAttacking)
            return;

        isAttacking = false;
        hasHitThisAttack = false;

        if (animator != null)
            animator.SetBool(IsAttackingHash, false);

        if (attackResetCoroutine != null)
        {
            StopCoroutine(attackResetCoroutine);
            attackResetCoroutine = null;
        }
    }
}
