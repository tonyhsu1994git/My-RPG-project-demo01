using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FireballSkill", menuName = "Skills/Fireball Skill")]
public class FireballSkill : Skill
{
    [SerializeField] private fire firePrefab;
    [SerializeField] private int fireCount = 3;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float orbitRadius = 1.2f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float damage = 15f;

    public override void Activate(SkillContext context)
    {
        if (firePrefab == null || context.CoroutineRunner == null)
            return;

        context.CoroutineRunner.StartCoroutine(SpawnOrbitingFires(context.PlayerTransform));
    }

    private IEnumerator SpawnOrbitingFires(Transform player)
    {
        fire[] fires = new fire[fireCount];
        float angleStep = 360f / fireCount;

        for (int i = 0; i < fireCount; i++)
        {
            fire instance = Instantiate(firePrefab, player.position, Quaternion.identity);
            instance.Initialize(
                player,
                orbitRadius,
                angleStep * i,
                rotationSpeed,
                damage,
                duration);

            fires[i] = instance;
        }

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < fires.Length; i++)
        {
            if (fires[i] != null)
                Destroy(fires[i].gameObject);
        }
    }
}
