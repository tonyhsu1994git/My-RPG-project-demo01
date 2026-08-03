using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [SerializeField] private Skill[] skills;

    private SkillContext skillContext;
    private float[] nextReadyTimes;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        skillContext = new SkillContext
        {
            PlayerTransform = transform,
            PlayerRigidbody = GetComponent<Rigidbody2D>(),
            PlayerMovement = GetComponent<PlayerMovement>(),
            PlayerHealth = GetComponent<PlayerHealth>(),
            CoroutineRunner = this
        };

        playerAttack = GetComponent<PlayerAttack>();
        nextReadyTimes = skills != null ? new float[skills.Length] : System.Array.Empty<float>();
    }

    private void Update()
    {
        if (skills == null)
            return;

        for (int i = 0; i < skills.Length; i++)
        {
            Skill skill = skills[i];
            if (skill == null)
                continue;

            if (!Input.GetKeyDown(skill.ActivationKey))
                continue;

            if (Time.time < nextReadyTimes[i])
                continue;

            if (playerAttack != null && playerAttack.IsAttacking)
                continue;

            skill.Activate(skillContext);
            nextReadyTimes[i] = Time.time + skill.Cooldown;
        }
    }
}
