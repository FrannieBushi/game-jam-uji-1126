using UnityEngine;

/// <summary>
/// Enemigo de aura: quita vida al jugador mientras esté dentro de un radio,
/// respetando el cooldown compartido de Enemy (damageCooldown, 1 s por defecto).
/// No necesita collider ni trigger: usa la distancia al jugador.
/// </summary>
public class Aura : Enemy
{
    [Header("Aura")]
    [SerializeField] private float radius = 3f;

    private Collider2D playerCol;

    protected override void Awake()
    {
        base.Awake();
        if (player != null)
            playerCol = player.GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {
        if (player == null || playerCol == null) return;

        if (DistanceToPlayer() <= radius)
            TryContactDamage(playerCol, baseDamage);
    }

    // Muestra el radio en la vista Scene al seleccionar el objeto.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}