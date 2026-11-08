using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("Enemigos posibles. Se elige uno al azar entre los que no sean null.")]
    public GameObject[] enemyPool;

    [Tooltip("Probabilidad base (0 a 1) de que este spawnpoint genere un enemigo.")]
    [SerializeField, Range(0f, 1f)] private float spawnChance = 0.5f;

    [Tooltip("Si es true, el enemigo se instancia como hijo del spawnpoint.")]
    [SerializeField] private bool parentToSpawnpoint = false;

    [Tooltip("Si es true, el spawnpoint se destruye tras intentar el spawn.")]
    [SerializeField] private bool destroyAfterSpawn = false;

    [Header("Aumento según distancia")]
    [SerializeField] private float originY = 0f;
    [SerializeField] private float unitsToMeters = 1f;
    [Tooltip("Metros a partir de los cuales sube la probabilidad (de menor a mayor).")]
    [SerializeField] private float[] distanceThresholds = { 100f, 250f, 500f };
    [Tooltip("Fracción de lo que falta para llegar a 1 que se suma a la probabilidad base en cada tramo. Un elemento más que los umbrales.")]
    [SerializeField, Range(0f, 1f)] private float[] fractionToOne = { 0f, 0f, 0.5f, 0.6667f };

    private GameObject spawnedEnemy;

    private void Start()
    {
        TrySpawn();
    }

    public GameObject TrySpawn()
    {
        if (enemyPool == null || enemyPool.Length == 0) return null;

        // Tirada de probabilidad (ajustada por distancia).
        if (Random.value > GetSpawnChance()) return null;

        // Elegir un prefab válido al azar.
        GameObject prefab = enemyPool[Random.Range(0, enemyPool.Length)];
        if (prefab == null) return null;

        spawnedEnemy = Instantiate(
            prefab,
            transform.position,
            Quaternion.identity,
            parentToSpawnpoint ? transform : null
        );

        if (destroyAfterSpawn) Destroy(gameObject);

        return spawnedEnemy;
    }

    private float GetSpawnChance()
    {
        float meters = Mathf.Max(0f, transform.position.y - originY) * unitsToMeters;

        int tier = 0;
        for (int i = 0; i < distanceThresholds.Length; i++)
        {
            if (meters >= distanceThresholds[i]) tier = i + 1;
        }
        tier = Mathf.Min(tier, fractionToOne.Length - 1);

        // p + (1 - p) * fracción
        return spawnChance + (1f - spawnChance) * fractionToOne[tier];
    }
}