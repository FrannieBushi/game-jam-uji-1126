using System.Collections;
using UnityEngine;

public class PlayerDamageFlash : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Player player;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Parpadeo")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private float flashDuration = 0.08f; // tiempo con el color de daño
    [SerializeField] private float intervalDuration = 0.08f; // tiempo con el color normal

    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        if (player == null) player = GetComponent<Player>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    void OnEnable()
    {
        if (player != null) player.OnDamaged += HandleDamage;
    }

    void OnDisable()
    {
        if (player != null) player.OnDamaged -= HandleDamage;

        // Por si se desactiva en mitad del parpadeo
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    private void HandleDamage(float amount)
    {
        // Si ya estaba parpadeando, reinicia el efecto
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(intervalDuration);
        }

        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }
}
