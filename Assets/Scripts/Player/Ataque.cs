using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ataque : MonoBehaviour
{

    [SerializeField] private KeyCode attackKey = KeyCode.J;
    [SerializeField] private bool useMouse = true;
    [SerializeField] private int mouseButton = 0;            // 0 = click izquierdo
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldown = 0.4f;
    [SerializeField] private float activeTime = 0.15f;       // Tiempo que la zona de golpe está activa
    [SerializeField] private LayerMask targetLayers = ~0;    // Qué capas pueden recibir golpe
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.5f, 0.8f);
    [SerializeField] private float reach = 1.0f;             // Distancia del centro del personaje
    [SerializeField] private float knockbackForce = 8f;      // Empuje a objetivos con Rigidbody2D
    [SerializeField] private GameObject attackVisual;

    public UnityEvent onAttack;                              // Al pulsar el ataque
    public UnityEvent onHit;                                 // Si golpea algo (una vez por ataque)

    private static readonly Vector2[] Directions =
        { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private float nextAttackTime;
    private bool attacking;

    private void Awake()
    {
        if (attackVisual != null) attackVisual.SetActive(false);
    }

    private void Update()
    {
        bool pressed = Input.GetKeyDown(attackKey) ||
                       (useMouse && Input.GetMouseButtonDown(mouseButton));

        if (pressed && !attacking && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        attacking = true;
        nextAttackTime = Time.time + cooldown;
        onAttack?.Invoke();

        if (attackVisual != null) attackVisual.SetActive(true);

        var alreadyHit = new HashSet<Collider2D>();
        bool hitSomething = false;
        float end = Time.time + activeTime;

        while (Time.time < end)
        {
            foreach (Vector2 dir in Directions)
                if (ScanDirection(dir, alreadyHit)) hitSomething = true;

            yield return new WaitForFixedUpdate();
        }

        if (attackVisual != null) attackVisual.SetActive(false);
        if (hitSomething) onHit?.Invoke();

        attacking = false;
    }

    private bool ScanDirection(Vector2 dir, HashSet<Collider2D> alreadyHit)

    {
        Vector2 center = (Vector2)transform.position + dir * reach;
        Vector2 size = dir.y != 0 ? new Vector2(hitboxSize.y, hitboxSize.x) : hitboxSize;
        Collider2D[] cols = Physics2D.OverlapBoxAll(center, size, 0f, targetLayers);
        bool any = false;
        foreach (var c in cols)

        {
            if (c.gameObject == gameObject || c.transform.IsChildOf(transform)) continue;
            if (!alreadyHit.Add(c)) continue;
            any = true;
            c.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Rigidbody2D target = c.attachedRigidbody;
            if (target != null && target.bodyType == RigidbodyType2D.Dynamic)
                target.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        }

        return any;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (Vector2 dir in Directions)
        {
            Vector2 size = dir.y != 0 ? new Vector2(hitboxSize.y, hitboxSize.x) : hitboxSize;
            Gizmos.DrawWireCube(transform.position + (Vector3)(dir * reach), size);
        }
    }
}