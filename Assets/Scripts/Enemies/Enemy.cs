using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Stats base")]
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float moveSpeed = 3f;
    [Tooltip("Segundos mínimos entre dos golpes de contacto.")]
    [SerializeField] protected float damageCooldown = 1f;

    [Header("Sonido ambiente")]
    [Tooltip("Uno se elige al azar cada vez. Distinto por prefab.")]
    [SerializeField] protected AudioClip[] ambientClips;
    [Tooltip("Segundos entre sonidos (mínimo, máximo).")]
    [SerializeField] protected Vector2 ambientInterval = new Vector2(4f, 10f);
    [SerializeField, Range(0f, 1f)] protected float ambientVolume = 1f;
    [SerializeField] protected Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    protected Transform player;
    private AudioSource audioSource;
    private float nextDamageTime;

    public float BaseDamage => baseDamage;
    public float MoveSpeed => moveSpeed;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning($"{name}: no se encontró ningún objeto con tag 'Player'.");
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(AmbientSoundRoutine());
    }

    private IEnumerator AmbientSoundRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(ambientInterval.x, ambientInterval.y));
            PlayAmbientSound();
        }
    }

    /// Sobrescribible si algún enemigo quiere otra lógica.
    protected virtual void PlayAmbientSound()
    {
        if (ambientClips == null || ambientClips.Length == 0) return;
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(ambientClips[Random.Range(0, ambientClips.Length)], ambientVolume);
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
