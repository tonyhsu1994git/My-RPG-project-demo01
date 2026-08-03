using UnityEngine;

[CreateAssetMenu(fileName = "HealSkill", menuName = "Skills/Heal Skill")]
public class HealSkill : Skill
{
    [SerializeField] private float healAmount = 100f;
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private float effectDuration = 1f;

    public override void Activate(SkillContext context)
    {
        if (context.PlayerHealth != null)
            context.PlayerHealth.Heal(healAmount);

        if (healEffectPrefab != null && context.PlayerTransform != null)
        {
            GameObject effect = Instantiate(
                healEffectPrefab,
                context.PlayerTransform.position,
                Quaternion.identity,
                context.PlayerTransform);

            Destroy(effect, effectDuration);
        }
    }
}
