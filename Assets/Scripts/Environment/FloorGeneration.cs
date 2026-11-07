using UnityEngine;

public class FloorGenerator : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] GameObject[] chunkPrefabs;
    [SerializeField] GameObject closedPrefab;
    [SerializeField] float chunkHeight = 10f;
    [SerializeField] int chunksAhead = 3;
    [SerializeField] float closeMargin = 2f;
    [SerializeField] int maxChunksBehind = 2;
    [SerializeField] bool avoidRepeat = true;
    [SerializeField] int seed = 0;

    
    GameObject[] activeChunks;
    GameObject[] closedOverlays;
    int head;
    private int count;
    int closedCount;
 
    float nextSpawnY;
    int lastIndex = -1;
    System.Random rng;
 
    private void Start()
    {
        //seed aleatoria
        rng = seed == 0 ? new System.Random() : new System.Random(seed);
 
        int capacity = chunksAhead + maxChunksBehind + 3;
        activeChunks = new GameObject[capacity];
        closedOverlays = new GameObject[capacity];
 
        nextSpawnY = transform.position.y;
        UpdateGeneration();
    }
 
    private void Update()
    {
        UpdateGeneration();
    }
 
    private void UpdateGeneration()
    {
        //generar
        float targetY = player.position.y + chunksAhead * chunkHeight;
        while (nextSpawnY < targetY)
            SpawnChunk();
 
        //cerrar con tope
        for (int k = 0; k < count; k++)
        {
            int i = (head + k) % activeChunks.Length;
            if (closedOverlays[i] != null) continue;
 
            float bottom = activeChunks[i].transform.position.y;
            if (player.position.y > bottom + chunkHeight + closeMargin)
            {
                closedOverlays[i] = Instantiate(closedPrefab,
                    new Vector3(transform.position.x, bottom, 0f),
                    Quaternion.identity,
                    activeChunks[i].transform);
                closedCount++;
            }
        }
 
        //limpiar chunks
        while (count > 0 && closedCount > maxChunksBehind)
            RemoveOldest();
    }
 
    private void SpawnChunk()
    {
        if (count == activeChunks.Length)
            RemoveOldest();
 
        int index = PickIndex();
        GameObject currentChunk = Instantiate(chunkPrefabs[index],
            new Vector3(transform.position.x, nextSpawnY, 0f),
            Quaternion.identity,
            transform);
 
        int slot = (head + count) % activeChunks.Length; //literalmente progra 1 (nicho)
        activeChunks[slot] = currentChunk;
        closedOverlays[slot] = null;
        count++;
 
        nextSpawnY += chunkHeight;
        lastIndex = index;
    }
 
    private void RemoveOldest()
    {
        if (closedOverlays[head] != null) closedCount--;
 
        Destroy(activeChunks[head]);
        activeChunks[head] = null;
        closedOverlays[head] = null;
 
        head = (head + 1) % activeChunks.Length;
        count--;
    }
 
    private int PickIndex()
    {
        if (chunkPrefabs.Length == 1) return 0;
        int i;
        do { i = rng.Next(chunkPrefabs.Length); }
        while (avoidRepeat && i == lastIndex);
        return i;
    }

    public void SpawnDoor(DoorType doorType, DoorOrientation doorOrientation, Transform transform)
    {
        
    }
}
