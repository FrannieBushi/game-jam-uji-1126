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

    [Header("Orientación")]
    [Tooltip("Sprite que se voltea. Si se deja vacío, se busca en este objeto o en sus hijos.")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Margen mínimo en X para cambiar de lado (evita parpadeo cuando el objetivo está casi en vertical).")]
    [SerializeField] private float flipDeadZone = 0.05f;

    private Vector2 target;
    private bool hasTarget;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Collider2D>().isTrigger = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

        UpdateFacing();

        if (Vector2.Distance(transform.position, target) <= arriveThreshold)
            PickNewTarget();
    }

    private void UpdateFacing()
    {
        if (spriteRenderer == null) return;

        float dirX = target.x - transform.position.x;

        // El sprite original mira a la izquierda:
        // yendo a la derecha -> flipX = true, yendo a la izquierda -> flipX = false.
        if (dirX > flipDeadZone) spriteRenderer.flipX = true;
        else if (dirX < -flipDeadZone) spriteRenderer.flipX = false;
        // Dentro de la zona muerta se mantiene la orientación actual.
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