using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private KeyCode attackKey = KeyCode.J;
    [SerializeField] private bool useMouse = true;
    [SerializeField] private int mouseButton = 0;             // 0 = clic izquierdo
    [SerializeField] private Vector2 startFacing = Vector2.down;
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldown = 0.4f;
    [SerializeField] private float activeTime = 0.15f;
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.5f, 0.8f);
    [SerializeField] private float reach = 1.0f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private GameObject attackVisual;         // Efecto visual (desactivado por defecto)
    public UnityEvent onAttack;
    public UnityEvent onHit;
    private Vector2 facing;
    private float nextAttackTime;
    private bool attacking;

    private void Awake()
    {
        facing = SnapToCardinal(startFacing);
        if (attackVisual != null) attackVisual.SetActive(false);
    }

    private void Update()
    {
        UpdateFacing();

        bool pressed = Input.GetKeyDown(attackKey) ||
                       (useMouse && Input.GetMouseButtonDown(mouseButton));

        if (pressed && !attacking && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine(facing));
    }

    private void UpdateFacing()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 && v == 0)
            facing = new Vector2(Mathf.Sign(h), 0f);
        else if (v != 0 && h == 0)
            facing = new Vector2(0f, Mathf.Sign(v));
        else if (h != 0 && v != 0)
        {
            if (Input.GetButtonDown("Horizontal")) facing = new Vector2(Mathf.Sign(h), 0f);
            else if (Input.GetButtonDown("Vertical")) facing = new Vector2(0f, Mathf.Sign(v));
        }
    }

    private static Vector2 SnapToCardinal(Vector2 d)
    {
        if (d == Vector2.zero) return Vector2.down;
        return Mathf.Abs(d.x) >= Mathf.Abs(d.y)
            ? new Vector2(Mathf.Sign(d.x), 0f)
            : new Vector2(0f, Mathf.Sign(d.y));
    }

    private IEnumerator AttackRoutine(Vector2 dir)
    {
        attacking = true;
        nextAttackTime = Time.time + cooldown;
        onAttack?.Invoke();
        ShowVisual(dir, true);

        var alreadyHit = new HashSet<Collider2D>();
        bool hitSomething = false;
        float end = Time.time + activeTime;

        while (Time.time < end)
        {
            if (Scan(dir, alreadyHit)) hitSomething = true;
            yield return new WaitForFixedUpdate();
        }

        ShowVisual(dir, false);
        if (hitSomething) onHit?.Invoke();
        attacking = false;
    }

    private bool Scan(Vector2 dir, HashSet<Collider2D> alreadyHit)
    {
        Vector2 center = (Vector2)transform.position + dir * reach;
        Vector2 size = dir.y != 0 ? new Vector2(hitboxSize.y, hitboxSize.x) : hitboxSize;

        bool any = false;
        foreach (var c in Physics2D.OverlapBoxAll(center, size, 0f, targetLayers))
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

    private void ShowVisual(Vector2 dir, bool show)
    {
        if (attackVisual == null) return;
        if (show)
        {
            attackVisual.transform.position = (Vector2)transform.position + dir * reach;
            attackVisual.transform.rotation =
                Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
        attackVisual.SetActive(show);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 dir = Application.isPlaying ? facing : SnapToCardinal(startFacing);
        Vector2 size = dir.y != 0 ? new Vector2(hitboxSize.y, hitboxSize.x) : hitboxSize;
        Gizmos.DrawWireCube(transform.position + (Vector3)(dir * reach), size);
    }
}