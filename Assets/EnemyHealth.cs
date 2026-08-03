using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float hpBarYOffset = 1.2f;
    [SerializeField] private Vector2 hpBarSize = new Vector2(1.2f, 0.15f);
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Image hpBackground;
    [SerializeField] private Image hpRed;

    private static readonly Color FlashColor = new Color(1f, 0.25f, 0.25f, 1f);
    private static readonly Color BackgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
    private static readonly Color RedColor = new Color(0.85f, 0.15f, 0.15f, 1f);

    private float currentHealth;
    private int lastDamageAttackId = -1;
    private Color originalColor;
    private Coroutine bodyFlashCoroutine;
    private static Sprite s_WhiteSprite;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercent => maxHealth > 0f ? currentHealth / maxHealth : 0f;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        EnsureHealthBarUI();
        ConfigureHealthBarVisuals();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damageAmount, int attackId = -1)
    {
        if (damageAmount <= 0f || currentHealth <= 0f)
            return;

        if (attackId >= 0 && attackId == lastDamageAttackId)
            return;

        lastDamageAttackId = attackId;
        currentHealth -= damageAmount;
        UpdateHealthBar();
        PlayBodyFlash();

        if (currentHealth <= 0f)
            Die();
    }

    private void PlayBodyFlash()
    {
        if (spriteRenderer == null)
            return;

        if (bodyFlashCoroutine != null)
            StopCoroutine(bodyFlashCoroutine);

        bodyFlashCoroutine = StartCoroutine(BodyFlashRoutine());
    }

    private IEnumerator BodyFlashRoutine()
    {
        spriteRenderer.color = FlashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        bodyFlashCoroutine = null;
    }

    private void UpdateHealthBar()
    {
        if (hpRed == null)
            return;

        hpRed.fillAmount = HealthPercent;
        hpRed.color = RedColor;
        hpRed.enabled = HealthPercent > 0f;
    }

    private void EnsureHealthBarUI()
    {
        if (hpBackground != null && hpRed != null)
            return;

        Transform existingBar = transform.Find("HPBar");
        if (existingBar != null)
        {
            BindExistingHealthBar(existingBar);
            return;
        }

        GameObject hpBarRoot = new GameObject("HPBar");
        hpBarRoot.transform.SetParent(transform, false);
        hpBarRoot.transform.localPosition = new Vector3(0f, hpBarYOffset, 0f);

        Canvas canvas = hpBarRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = hpBarSize;
        canvasRect.localScale = Vector3.one;

        hpBackground = CreateBarImage("HPUI_Background", canvasRect, BackgroundColor, false, 0);
        hpRed = CreateBarImage("HPUI_Red", canvasRect, RedColor, true, 1);
    }

    private void ConfigureHealthBarVisuals()
    {
        if (hpBackground != null)
        {
            hpBackground.type = Image.Type.Simple;
            hpBackground.fillAmount = 1f;
        }

        SetupFilledBar(hpRed, RedColor);
    }

    private void SetupFilledBar(Image image, Color color)
    {
        if (image == null)
            return;

        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
        image.fillOrigin = (int)Image.OriginHorizontal.Left;
        image.color = color;
        image.fillAmount = 1f;
    }

    private void BindExistingHealthBar(Transform hpBarRoot)
    {
        hpBackground = hpBarRoot.Find("HPUI_Background")?.GetComponent<Image>();
        hpRed = hpBarRoot.Find("HPUI_Red")?.GetComponent<Image>();
    }

    private Image CreateBarImage(string objectName, RectTransform parent, Color color, bool filled, int siblingIndex)
    {
        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);

        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.SetSiblingIndex(siblingIndex);

        Image image = imageObject.AddComponent<Image>();
        image.sprite = GetWhiteSprite();
        image.color = color;
        image.raycastTarget = false;

        if (filled)
        {
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = 1f;
        }

        return image;
    }

    private static Sprite GetWhiteSprite()
    {
        if (s_WhiteSprite != null)
            return s_WhiteSprite;

        s_WhiteSprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0f, 0f, 4f, 4f),
            new Vector2(0.5f, 0.5f),
            4f);

        return s_WhiteSprite;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
