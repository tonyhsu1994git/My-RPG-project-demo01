using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [SerializeField] private KeyCode activationKey = KeyCode.None;
    [SerializeField] private float cooldown = 1f;

    public KeyCode ActivationKey => activationKey;
    public float Cooldown => cooldown;

    public abstract void Activate(SkillContext context);
}

public class SkillContext
{
    public Transform PlayerTransform { get; set; }
    public Rigidbody2D PlayerRigidbody { get; set; }
    public PlayerMovement PlayerMovement { get; set; }
    public PlayerHealth PlayerHealth { get; set; }
    public MonoBehaviour CoroutineRunner { get; set; }
}
