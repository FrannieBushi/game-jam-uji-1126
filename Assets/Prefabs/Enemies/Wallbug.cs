using System.Collections;
using UnityEngine;

/// <summary>
/// Bicho pared: ancho 1 hasta que te acercas; entonces se expande (5 tiles) y bloquea el paso.
/// Ver cómo se expande quita un poco de cordura; atravesarlo cuesta mucha.
/// Requiere un Collider2D con IsTrigger activado (así el jugador puede cruzarlo pagando vida).
/// </summary>
/// 
[RequireComponent(typeof(Collider2D))]
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
    [Tooltip("Fracción del daño base que quita ver la expansión (una vez).")]
    [SerializeField, Range(0f, 1f)] private float seenExpansionDamageFraction = 0.2f;
    [Tooltip("Multiplicador del daño base al atravesar el bicho ya expandido.")]
    [SerializeField] private float crossingDamageMultiplier = 3f;

    private enum State { Idle, Expanding, Blocking }
    private State state = State.Idle;

    private Collider2D col;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
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

        // El jugador está lo bastante cerca para verlo: pierde un poco de cordura.
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

    private void SetWidth(float width)
    {
        Vector3 s = transform.localScale;
        if (expandAlongY) s.y = width; else s.x = width;
        transform.localScale = s;
    }

    // Mientras el jugador esté dentro del bicho ya expandido, pierde mucha cordura (con cooldown).
    private void OnTriggerStay2D(Collider2D other)
    {
        if (state != State.Blocking) return;
        TryContactDamage(other, baseDamage * crossingDamageMultiplier);
    }
}
