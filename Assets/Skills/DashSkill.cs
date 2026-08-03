using UnityEngine;

[CreateAssetMenu(fileName = "DashSkill", menuName = "Skills/Dash Skill")]
public class DashSkill : Skill
{
    [SerializeField] private float dashDistance = 2f;

    public override void Activate(SkillContext context)
    {
        if (context.PlayerRigidbody == null || context.PlayerTransform == null)
            return;

        Vector2 direction = GetDashDirection(context);
        Vector2 target = context.PlayerRigidbody.position + direction * dashDistance;
        context.PlayerRigidbody.MovePosition(target);
        context.PlayerRigidbody.velocity = Vector2.zero;
    }

    private static Vector2 GetDashDirection(SkillContext context)
    {
        if (context.PlayerMovement != null)
        {
            Vector2 moveInput = context.PlayerMovement.MoveInput;
            if (moveInput.sqrMagnitude > 0.01f)
                return moveInput.normalized;
        }

        return context.PlayerMovement != null
            ? context.PlayerMovement.GetBackwardDirection()
            : Vector2.left;
    }
}
