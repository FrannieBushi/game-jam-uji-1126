using System.Collections;
using UnityEngine;

/// <summary>
/// Gato fantasma: empieza como una sombra en el suelo. Si te acercas mucho, sale
/// (aggro, te quita cordura) y a partir de ahí te persigue; si te toca, más daño.
/// Asigna en el inspector el objeto "shadowVisual" (sombra) y "catVisual" (gato).
/// </summary>
public class GhostCat : ChaserEnemy
{
    [Header("Sombra / aparición")]
    [SerializeField] private GameObject shadowVisual;
    [SerializeField] private GameObject catVisual;
    [Tooltip("Tiempo entre que sale del suelo y empieza a perseguir.")]
    [SerializeField] private float emergeTime = 0.5f;

    [Header("Aggro")]
    [Tooltip("Fracción del daño base que quita pillar el aggro (si estás dentro del trigger al salir).")]
    [SerializeField, Range(0f, 2f)] private float aggroDamageMultiplier = 1f;

    private bool emerging;

    protected override void Awake()
    {
        base.Awake();
        // detectionRange aquí es la distancia a la que sale de la sombra.
        if (shadowVisual != null) shadowVisual.SetActive(true);
        if (catVisual != null) catVisual.SetActive(false);
    }

    protected override void Update()
    {
        if (player == null) return;

        if (!isChasing && !emerging && DistanceToPlayer() <= detectionRange)
        {
            StartCoroutine(EmergeRoutine());
            return;
        }

        if (isChasing) Chase();
    }

    private IEnumerator EmergeRoutine()
    {
        emerging = true;
        if (shadowVisual != null) shadowVisual.SetActive(false);
        if (catVisual != null) catVisual.SetActive(true);

        yield return new WaitForSeconds(emergeTime);

        OnAggro();
        emerging = false;
    }

    protected override void OnAggro()
    {
        base.OnAggro();

        // Aggro: daño inicial si el jugador está dentro de su collider al salir.
        Collider2D myCol = GetComponent<Collider2D>();
        Collider2D playerCol = player.GetComponentInChildren<Collider2D>();
        if (playerCol != null && myCol.IsTouching(playerCol))
            TryContactDamage(playerCol, baseDamage * aggroDamageMultiplier);
    }
}
