using UnityEngine;

/// <summary>
/// Enemigo que persigue al jugador y le quita cordura al tocarlo.
/// Base de GhostCat y Slime. Necesita un Collider2D con IsTrigger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ChaserEnemy : Enemy
{
    [Header("Persecución")]
    [Tooltip("Distancia a la que empieza a perseguir.")]
    [SerializeField] protected float detectionRange = 8f;

    protected bool isChasing;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Collider2D>().isTrigger = true;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        if (!isChasing && DistanceToPlayer() <= detectionRange)
            OnAggro();

        if (isChasing)
            Chase();
    }

    /// <summary>Se llama una vez cuando el enemigo pilla aggro.</summary>
    protected virtual void OnAggro()
    {
        isChasing = true;
    }

    protected virtual void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!isChasing) return;
        TryContactDamage(other, baseDamage);
    }
}

