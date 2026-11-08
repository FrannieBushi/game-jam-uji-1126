using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats base")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float moveSpeed = 3f;
    [Tooltip("Segundos mínimos entre dos golpes de contacto.")]
    [SerializeField] protected float damageCooldown = 1f;

    protected Transform player;
    private float nextDamageTime;

    public float BaseDamage => baseDamage;
    public float MoveSpeed => moveSpeed;

    protected virtual void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning($"{name}: no se encontró ningún objeto con tag 'Player'.");
    }

    protected float DistanceToPlayer()
    {
        return player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;
    }

    ///Daño inmediato, sin cooldown (aggro, expansion, etc.).
    protected bool DamagePlayer(Collider2D other, float amount)
    {
        Player target = other.GetComponentInParent<Player>();
        if (target == null) return false;
        target.LoseHealth(amount);
        return true;
    }

    ///Daño de contacto respetando el cooldown compartido.
    protected bool TryContactDamage(Collider2D other, float amount)
    {
        if (Time.time < nextDamageTime) return false;
        if (!DamagePlayer(other, amount)) return false;
        nextDamageTime = Time.time + damageCooldown;
        return true;
    }
}
