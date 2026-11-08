using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("Enemigos posibles. Se elige uno al azar entre los que no sean null.")]
    public GameObject[] enemyPool;

    [Tooltip("Probabilidad (0 a 1) de que este spawnpoint genere un enemigo.")]
    [SerializeField, Range(0f, 1f)] private float spawnChance = 0.5f;

    [Tooltip("Si es true, el enemigo se instancia como hijo del spawnpoint.")]
    [SerializeField] private bool parentToSpawnpoint = false;

    [Tooltip("Si es true, el spawnpoint se destruye tras intentar el spawn.")]
    [SerializeField] private bool destroyAfterSpawn = false;

    private GameObject spawnedEnemy;

    private void Start()
    {
        TrySpawn();
    }

    public GameObject TrySpawn()
    {
        if (enemyPool == null || enemyPool.Length == 0) return null;

        // Tirada de probabilidad.
        if (Random.value > spawnChance) return null;

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
}