using UnityEngine;

public class fire : MonoBehaviour
{
    private Transform orbitCenter;
    private float orbitRadius;
    private float orbitAngle;
    private float rotationSpeed;
    private float damage;
    private float lifetime;

    private bool hasHit;
    private int hitId;

    public void Initialize(
        Transform center,
        float radius,
        float startAngle,
        float angularSpeed,
        float damageAmount,
        float lifeTime)
    {
        orbitCenter = center;
        orbitRadius = radius;
        orbitAngle = startAngle;
        rotationSpeed = angularSpeed;
        damage = damageAmount;
        lifetime = lifeTime;
        hitId = GetInstanceID();

        UpdateOrbitPosition();
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (orbitCenter == null)
        {
            Destroy(gameObject);
            return;
        }

        orbitAngle += rotationSpeed * Time.deltaTime;
        UpdateOrbitPosition();
    }

    private void UpdateOrbitPosition()
    {
        float radians = orbitAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * orbitRadius;
        transform.position = orbitCenter.position + (Vector3)offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        if (!other.CompareTag("Enemy"))
            return;

        OnHit(other);
    }

    /// <summary>
    /// 触碰 Enemy 标签敌人时触发，伤害结算方式与 PlayerAttack.OnHit 一致。
    /// </summary>
    public void OnHit(Collider2D other)
    {
        if (hasHit)
            return;

        if (other.isTrigger)
            return;

        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth == null)
            return;

        enemyHealth.TakeDamage(damage, hitId);
        hasHit = true;
        Destroy(gameObject);
    }
}
