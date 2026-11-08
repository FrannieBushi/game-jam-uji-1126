using System;
using UnityEngine;

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

    private Rigidbody2D rb2D;
    private Vector2 movementInput;
    private float currentHealth;
    private float drainTimer;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public event Action<float> OnDamaged;
    public bool IsMoving => movementInput.sqrMagnitude > 0.01f;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
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
        rb2D.linearVelocity = movementInput * speed;

        //animaciones
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (rb2D.linearVelocity.x > 0) sprite.flipX = true;
        else if (rb2D.linearVelocity.x < 0) sprite.flipX = false;

        animator.SetFloat("xspeed", Math.Abs(rb2D.linearVelocity.x));
        animator.SetFloat("+yspeed", rb2D.linearVelocity.y);
        animator.SetFloat("-yspeed", rb2D.linearVelocity.y);
    }

    /// Daño normal: dispara OnDamaged (parpadeo, etc.).
    public void LoseHealth(float amountLost)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amountLost);
        OnDamaged?.Invoke(amountLost);
    }

    /// Daño sin efectos: no dispara OnDamaged.
    public void LoseHealthSilently(float amountLost)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amountLost);
    }

    public void RestoreHealth(float curedQuantity)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + curedQuantity);
    }
}