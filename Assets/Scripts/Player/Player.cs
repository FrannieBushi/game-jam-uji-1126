using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxHealth = 100;

    private Rigidbody2D rb2D;
    private Vector2 movementInput;
    private float currentHealth;
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

    public void LoseHealth(float amountLost)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amountLost);
        OnDamaged?.Invoke(amountLost);
    }

    public void RestoreHealth(float curedQuantity)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + curedQuantity);
    }
}
