using UnityEngine;

public class PlayerDoorInteractor : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactRadius = 1.2f;
    [SerializeField] private LayerMask doorLayer;

    [Header("Indicador de interacción")]
    [Tooltip("Prefab de la 'E' (sprite o Canvas world-space con CanvasGroup).")]
    [SerializeField] private GameObject keyIndicatorPrefab;
    [Tooltip("Posición respecto al centro de la puerta.")]
    [SerializeField] private Vector3 indicatorOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float indicatorFadeDuration = 0.25f;

    private Door currentDoor;

    private GameObject indicator;
    private SpriteRenderer[] indicatorSprites;
    private CanvasGroup indicatorGroup;
    private float indicatorAlpha;
    private Vector3 indicatorPos;

    private void Start()
    {
        if (keyIndicatorPrefab == null) return;

        indicator = Instantiate(keyIndicatorPrefab);
        indicatorSprites = indicator.GetComponentsInChildren<SpriteRenderer>(true);
        indicatorGroup = indicator.GetComponentInChildren<CanvasGroup>(true);
        ApplyAlpha(0f);
        indicator.SetActive(false);
    }

    private void Update()
    {
        currentDoor = FindClosestDoor();

        if (Input.GetKeyDown(interactKey) && !Door.OnCooldown && currentDoor != null)
            currentDoor.OpenDoor();

        UpdateIndicator();
    }

    private Door FindClosestDoor()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, doorLayer);

        Door closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Door door = hit.GetComponent<Door>();
            if (door == null || door.IsOpening) continue;

            float dist = ((Vector2)door.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = door;
            }
        }
        return closest;
    }

    private void UpdateIndicator()
    {
        if (indicator == null) return;

        bool show = currentDoor != null && !Door.OnCooldown;

        // Si hay puerta, seguirla. Si no, se queda donde estaba mientras hace fade out.
        if (currentDoor != null)
            indicatorPos = currentDoor.transform.position + indicatorOffset;

        float target = show ? 1f : 0f;
        if (!Mathf.Approximately(indicatorAlpha, target))
        {
            float step = Time.unscaledDeltaTime / Mathf.Max(0.01f, indicatorFadeDuration);
            indicatorAlpha = Mathf.MoveTowards(indicatorAlpha, target, step);
            ApplyAlpha(indicatorAlpha);
        }

        bool visible = indicatorAlpha > 0f;
        if (indicator.activeSelf != visible) indicator.SetActive(visible);
        if (visible) indicator.transform.position = indicatorPos;
    }

    private void ApplyAlpha(float a)
    {
        if (indicatorSprites != null)
        {
            foreach (SpriteRenderer sr in indicatorSprites)
            {
                Color c = sr.color;
                c.a = a;
                sr.color = c;
            }
        }
        if (indicatorGroup != null) indicatorGroup.alpha = a;
    }

    private void OnDestroy()
    {
        if (indicator != null) Destroy(indicator);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}