using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WallBug : Enemy
{
    [Header("Expansión")]
    [SerializeField] private float triggerDistance = 3f;
    [SerializeField] private float normalWidth = 1f;
    [SerializeField] private float expandedWidth = 5f;
    [SerializeField] private float expandDuration = 0.6f;
    [Tooltip("Marca true si el pasillo es horizontal y el bicho debe crecer en vertical (Y). Si no, crece en X.")]
    [SerializeField] private bool expandAlongY = true;

    [Header("Daño")]
    [SerializeField, Range(0f, 1f)] private float seenExpansionDamageFraction = 0.2f;
    [SerializeField] private float crossingDamageMultiplier = 3f;

    private enum State { Idle, Expanding, Blocking }
    private State state = State.Idle;
    Animator animator;

    private BoxCollider2D col;
    private Vector2 baseSize;
    private Vector2 baseOffset;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        animator = GetComponent<Animator>();

        baseSize = col.size;
        baseOffset = col.offset;
        SetWidth(normalWidth);
    }

    private void Update()
    {
        if (state == State.Idle && DistanceToPlayer() <= triggerDistance)
            StartCoroutine(ExpandRoutine());
    }

    private IEnumerator ExpandRoutine()
    {
        state = State.Expanding;
        animator.Play("expand");

        if (player != null)
        {
            Collider2D playerCol = player.GetComponentInChildren<Collider2D>();
            if (playerCol != null)
                DamagePlayer(playerCol, baseDamage * seenExpansionDamageFraction);
        }

        float t = 0f;
        while (t < expandDuration)
        {
            t += Time.deltaTime;
            SetWidth(Mathf.Lerp(normalWidth, expandedWidth, t / expandDuration));
            yield return null;
        }
        SetWidth(expandedWidth);
        state = State.Blocking;
    }

    // Ahora solo cambia el collider, el sprite no se toca.
    private void SetWidth(float width)
    {
        // El tamaño del collider está en espacio local, así que dividimos
        // por la escala del transform para que "width" sea en unidades de mundo.
        Vector3 scale = transform.localScale;

        Vector2 size = baseSize;
        if (expandAlongY) size.y = width / Mathf.Abs(scale.y);
        else              size.x = width / Mathf.Abs(scale.x);
        col.size = size;

        // Opcional: mantener el collider centrado en el mismo punto.
        col.offset = baseOffset;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (state != State.Blocking) return;
        TryContactDamage(other, baseDamage * crossingDamageMultiplier);
    }
}