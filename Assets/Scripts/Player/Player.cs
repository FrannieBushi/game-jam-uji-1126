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
        if(rb2D.linearVelocity.x>0) GetComponent<SpriteRenderer>().flipX=true;
        else GetComponent<SpriteRenderer>().flipX=false;
        animator.SetFloat("xspeed", Math.Abs(rb2D.linearVelocity.x));
    }

    public void RestoreHealth(float curedQuantity)
    {
        currentHealth += curedQuantity;
    }

    public void LoseHealth(float amountLost)
    {
        currentHealth -= amountLost;
    }

    

}
