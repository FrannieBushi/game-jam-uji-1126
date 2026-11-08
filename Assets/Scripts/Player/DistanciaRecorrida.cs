using UnityEngine;
using TMPro;

public class DistanciaRecorrida : MonoBehaviour
{
    public enum HallwayDetection { XRange, Collider }

    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private string textFormat = "Distancia: {0:0} m";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float originY = 0f;
    [SerializeField] private float unitsToMeters = 1f;
    [SerializeField] private HallwayDetection detection = HallwayDetection.XRange;
    [SerializeField] private float hallwayCenterX = 0f;
    [SerializeField] private float hallwayHalfWidth = 2f;
    [SerializeField] private Collider2D hallwayArea;
    private Player playerComp;
    private float maxDistance;


    public float Distance { get; private set; }
    public bool InHallway { get; private set; }

    private Transform player;

    private void Awake()
    {
        if (visualRoot == null && label != null) visualRoot = label.gameObject;
        SetVisible(false);
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag(playerTag);
            if (p == null) return;
            player = p.transform;

            playerComp = p.GetComponent<Player>();
            if (playerComp != null) playerComp.OnDied += SaveRun;
        }

        bool inside = IsInHallway(player.position);

        if (inside)
        {
            Distance = Mathf.Abs(player.position.y - originY) * unitsToMeters;
            if (Distance > maxDistance) maxDistance = Distance;
            if (label != null) label.text = string.Format(textFormat, Distance);
        }

        if (inside != InHallway)
        {
            InHallway = inside;
            SetVisible(inside);
        }
    }

    private void SaveRun()
    {
        RunStats.SaveRun(maxDistance);
    }

    private void OnDestroy()
    {
        if (playerComp != null) playerComp.OnDied -= SaveRun;
    }

    private bool IsInHallway(Vector2 pos)
    {
        // Por debajo del origen cuenta como fuera del pasillo.
        if (pos.y < originY) return false;

        if (detection == HallwayDetection.Collider && hallwayArea != null)
            return hallwayArea.OverlapPoint(pos);

        return Mathf.Abs(pos.x - hallwayCenterX) <= hallwayHalfWidth;
    }

    private void SetVisible(bool visible)
    {
        if (visualRoot != null) visualRoot.SetActive(visible);
    }

    private void OnDrawGizmosSelected()
    {
        if (detection != HallwayDetection.XRange) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(hallwayCenterX, originY, 0f),
                            new Vector3(hallwayHalfWidth * 2f, 200f, 0f));
    }
}