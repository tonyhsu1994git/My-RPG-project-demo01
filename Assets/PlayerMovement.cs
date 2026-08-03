using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private PlayerAttack playerAttack;
    private Vector2 moveInput;

    public Vector2 MoveInput => moveInput;

    public Vector2 GetBackwardDirection()
    {
        if (spriteRenderer != null && spriteRenderer.flipX)
            return Vector2.right;

        return Vector2.left;
    }

    private static readonly int HengHash = Animator.StringToHash("heng");
    private static readonly int ShuHash = Animator.StringToHash("shu");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        if (playerAttack != null && playerAttack.IsAttacking)
        {
            moveInput = Vector2.zero;

            if (animator != null)
            {
                animator.SetFloat(HengHash, 0f);
                animator.SetFloat(ShuHash, 0f);
            }

            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        if (animator != null)
        {
            animator.SetFloat(HengHash, Mathf.Abs(moveInput.x));
            animator.SetFloat(ShuHash, Mathf.Abs(moveInput.y));
        }

        UpdateFacing();
    }

    private void UpdateFacing()
    {
        if (spriteRenderer == null || moveInput.x == 0f)
            return;

        // 默认朝右；向左移动时水平翻转
        spriteRenderer.flipX = moveInput.x < 0f;
    }

    private void FixedUpdate()
    {
        if (playerAttack != null && playerAttack.IsAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = moveInput * speed;
    }
}
