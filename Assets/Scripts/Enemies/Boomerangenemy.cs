using UnityEngine;

/// <summary>
/// Enemigo bumerán: atraviesa paredes. Se dirige a la posición del jugador en un
/// momento dado; al llegar toma la nueva posición del jugador y vuelve hacia ella, y así.
/// Necesita Collider2D con IsTrigger (sin colisión sólida con paredes) y un Rigidbody2D
/// Kinematic para que los triggers se detecten.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BoomerangEnemy : Enemy
{
    [SerializeField] private float arriveThreshold = 0.05f;

    private Vector2 target;
    private bool hasTarget;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        PickNewTarget();
    }

    private void Update()
    {
        if (!hasTarget) { PickNewTarget(); return; }

        // Movimiento por transform: ignora paredes.
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) <= arriveThreshold)
            PickNewTarget();
    }

    private void PickNewTarget()
    {
        if (player == null) return;
        target = player.position;   // snapshot: no sigue al jugador en tiempo real
        hasTarget = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryContactDamage(other, baseDamage);
    }
}
