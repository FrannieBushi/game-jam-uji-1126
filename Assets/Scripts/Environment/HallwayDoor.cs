using UnityEngine;
using System.Collections;

public class HallwayDoor : MonoBehaviour
{
    [Header("Knock")]
    [SerializeField] private float minKnockInterval = 2f;
    [SerializeField] private float maxKnockInterval = 6f;

    [Header("Probabilidad de aparecer según distancia")]
    [SerializeField] private float originY = 0f;
    [SerializeField] private float unitsToMeters = 1f;
    [Tooltip("Metros a partir de los cuales baja la probabilidad (de menor a mayor).")]
    [SerializeField] private float[] distanceThresholds = { 100f, 250f, 500f };
    [Tooltip("Probabilidad de que la puerta exista: [0] antes del primer umbral, [1] tras el primero, etc. Debe tener un elemento más que los umbrales.")]
    [SerializeField, Range(0f, 1f)] private float[] spawnChances = { 0.5f, 0.35f, 0.2f, 0.1f };
    [SerializeField, Range(0f, 1f)] private float knockChance = 0.5f;

    Animator animator;
    Door door;

    void Start()
    {
        animator = GetComponent<Animator>();
        door = GetComponent<Door>();

        float meters = Mathf.Max(0f, transform.position.y - originY) * unitsToMeters;

        if (Random.value > GetSpawnChance(meters))
        {
            Destroy(gameObject);
            return;
        }

        if (Random.value < knockChance) door.doorType = DoorType.Knock;

        StartCoroutine(KnockRoutine());
    }

    private float GetSpawnChance(float meters)
    {
        int tier = 0;
        for (int i = 0; i < distanceThresholds.Length; i++)
        {
            if (meters >= distanceThresholds[i]) tier = i + 1;
        }

        tier = Mathf.Min(tier, spawnChances.Length - 1);
        return spawnChances[tier];
    }

    private IEnumerator KnockRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minKnockInterval, maxKnockInterval));

            if (door.doorType == DoorType.Knock)
                animator.Play("knock", 0, 0f);
        }
    }
}