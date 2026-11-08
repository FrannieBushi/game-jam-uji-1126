using UnityEngine;

public class PlayerDoorInteractor : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactRadius = 1.2f;
    [SerializeField] private LayerMask doorLayer;
    public bool hasKey = false;

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
            TryInteract();
    }

    private void TryInteract()
    {
        if (Door.OnCooldown) return;

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

        if (closest != null)
            closest.OpenDoor();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}