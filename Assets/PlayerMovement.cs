using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector2 moveInput;

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
    }

    private void Update()
    {
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
        rb.velocity = moveInput * speed;
    }
}
