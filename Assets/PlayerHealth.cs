using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float hpBarYOffset = 1.2f;
    [SerializeField] private Vector2 hpBarSize = new Vector2(1.2f, 0.15f);
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 2;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Image hpBackground;
    [SerializeField] private Image hpRed;
    [SerializeField] private Image hpGreen;

    private static readonly Color FlashColor = new Color(0xB6 / 255f, 0xB6 / 255f, 0xB6 / 255f, 1f);
    private static readonly Color BackgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);
    private static readonly Color RedColor = new Color(0.85f, 0.15f, 0.15f, 1f);
    private static readonly Color GreenColor = new Color(0.2f, 0.85f, 0.25f, 1f);

    private float currentHealth;
    private Color originalColor;
    private Coroutine bodyFlashCoroutine;
    private Coroutine hpBarFlashCoroutine;
    private static Sprite s_WhiteSprite;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercent => maxHealth > 0f ? currentHealth / maxHealth : 0f;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

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

    public void TakeDamage(float damageAmount)
    {
        if (damageAmount <= 0f || currentHealth <= 0f)
            return;

        currentHealth -= damageAmount;
        UpdateHealthBar();
        PlayBodyFlash();
        PlayHealthBarFlash();

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float healAmount)
    {
        if (healAmount <= 0f || currentHealth <= 0f)
            return;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthBar();
    }

    private void PlayBodyFlash()
    {
        if (spriteRenderer == null)
            return;

        if (bodyFlashCoroutine != null)
            StopCoroutine(bodyFlashCoroutine);

        bodyFlashCoroutine = StartCoroutine(BodyFlashRoutine());
    }

    private void PlayHealthBarFlash()
    {
        if (hpGreen == null || hpRed == null)
            return;

        if (hpBarFlashCoroutine != null)
            StopCoroutine(hpBarFlashCoroutine);

        hpBarFlashCoroutine = StartCoroutine(HealthBarFlashRoutine());
    }

    private IEnumerator BodyFlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = FlashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        bodyFlashCoroutine = null;
    }

    private IEnumerator HealthBarFlashRoutine()
    {
        float fill = HealthPercent;

        for (int i = 0; i < flashCount; i++)
        {
            hpGreen.enabled = false;
            hpRed.fillAmount = fill;
            hpRed.enabled = fill > 0f;
            yield return new WaitForSeconds(flashDuration);

            hpRed.enabled = false;
            hpGreen.enabled = true;
            yield return new WaitForSeconds(flashDuration);
        }

        UpdateHealthBar();
        hpBarFlashCoroutine = null;
    }

    private void UpdateHealthBar()
    {
        float percent = HealthPercent;

        if (hpGreen != null)
        {
            hpGreen.fillAmount = percent;
            hpGreen.color = GreenColor;
            hpGreen.enabled = true;
        }

        if (hpRed != null)
            hpRed.enabled = false;
    }

    private void EnsureHealthBarUI()
    {
        if (hpBackground != null && hpRed != null && hpGreen != null)
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
        hpGreen = CreateBarImage("HPUI_Green", canvasRect, GreenColor, true, 2);
    }

    private void ConfigureHealthBarVisuals()
    {
        if (hpBackground != null)
        {
            hpBackground.type = Image.Type.Simple;
            hpBackground.fillAmount = 1f;
        }

        SetupFilledBar(hpGreen, GreenColor);
        SetupFilledBar(hpRed, RedColor);

        if (hpRed != null)
            hpRed.enabled = false;
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
        hpGreen = hpBarRoot.Find("HPUI_Green")?.GetComponent<Image>();
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
        Debug.Log("Player died.");
        gameObject.SetActive(false);
    }
}
