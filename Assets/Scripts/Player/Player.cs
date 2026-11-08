using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxHealth = 100;

    [Header("Drenaje pasivo")]
    [SerializeField] private bool passiveDrainEnabled = true;
    [Tooltip("Cada cuántos segundos se pierde vida pasivamente.")]
    [SerializeField] private float drainInterval = 5f;
    [Tooltip("Vida que se pierde en cada tick.")]
    [SerializeField] private float drainAmount = 1f;

    [Header("Muerte")]
    [Tooltip("Overlay negro con ScreenFader. Si se deja vacío se busca por tag 'BlackOverlay'.")]
    [SerializeField] private ScreenFader deathFader;
    [SerializeField] private float deathFadeDuration = 2f;

    private Rigidbody2D rb2D;
    private Vector2 movementInput;
    private float currentHealth;
    private float drainTimer;
    private bool isDead;
    private bool isTransitioning;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public bool IsTransitioning => isTransitioning;
    public event Action<float> OnDamaged;
    public event Action OnDied;
    public bool IsMoving => movementInput.sqrMagnitude > 0.01f;

    Animator animator;

    public event Action<bool> OnKeyChanged;

    bool hasKey;
    public bool HasKey => hasKey;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (deathFader == null)
        {
            GameObject overlay = GameObject.FindWithTag("BlackOverlay");
            if (overlay != null) deathFader = overlay.GetComponent<ScreenFader>();
        }
    }

    void Update()
    {
        if (isDead) return;

        // Durante la transición de una puerta: sin input y sin drenaje.
        if (isTransitioning)
        {
            movementInput = Vector2.zero;
            return;
        }

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        HandlePassiveDrain();
    }

    private void HandlePassiveDrain()
    {
        if (!passiveDrainEnabled || currentHealth <= 0f) return;

        drainTimer += Time.deltaTime;
        if (drainTimer >= drainInterval)
        {
            drainTimer -= drainInterval;
            LoseHealthSilently(drainAmount);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        rb2D.linearVelocity = isTransitioning ? Vector2.zero : movementInput * speed;

        //animaciones
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (rb2D.linearVelocity.x > 0) sprite.flipX = true;
        else if (rb2D.linearVelocity.x < 0) sprite.flipX = false;

        animator.SetFloat("xspeed", Math.Abs(rb2D.linearVelocity.x));
        animator.SetFloat("+yspeed", rb2D.linearVelocity.y);
        animator.SetFloat("-yspeed", rb2D.linearVelocity.y);
    }

    /// Daño normal: dispara OnDamaged (parpadeo, etc.). Ignorado al morir o en transición.
    public void LoseHealth(float amountLost)
    {
        if (isDead || isTransitioning) return;

        currentHealth = Mathf.Max(0f, currentHealth - amountLost);
        OnDamaged?.Invoke(amountLost);
        CheckDeath();
    }

    /// Daño sin efectos: no dispara OnDamaged. Ignorado al morir o en transición.
    public void LoseHealthSilently(float amountLost)
    {
        if (isDead || isTransitioning) return;

        currentHealth = Mathf.Max(0f, currentHealth - amountLost);
        CheckDeath();
    }

    public void RestoreHealth(float curedQuantity)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + curedQuantity);
    }

    /// Activa/desactiva el estado de transición (puertas): sin movimiento y sin daño.
    public void SetTransitioning(bool value)
    {
        isTransitioning = value;
        if (value)
        {
            movementInput = Vector2.zero;
            if (rb2D != null) rb2D.linearVelocity = Vector2.zero;
        }
    }

    private void CheckDeath()
    {
        if (currentHealth <= 0f && !isDead)
            StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        // Quitar el control: parar movimiento e input.
        movementInput = Vector2.zero;
        if (rb2D != null) rb2D.linearVelocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetFloat("xspeed", 0f);
            animator.SetFloat("+yspeed", 0f);
            animator.SetFloat("-yspeed", 0f);
        }

        OnDied?.Invoke();

        if (deathFader != null)
        {
            deathFader.FadeIn(deathFadeDuration);
            yield return new WaitForSecondsRealtime(deathFadeDuration);
        }

        SceneManager.LoadScene("Main Menu");
    }

    public void GiveKey()
    {
        if (hasKey) return;
        hasKey = true;
        OnKeyChanged?.Invoke(true);
    }

    public bool TryUseKey()
    {
        if (!hasKey) return false;
        hasKey = false;
        OnKeyChanged?.Invoke(false);
        return true;
    }
}