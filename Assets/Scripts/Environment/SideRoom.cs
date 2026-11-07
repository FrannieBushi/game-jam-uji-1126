using UnityEngine;

public class SideRoom : MonoBehaviour
{
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    void SpawnDoor(DoorOrientation doorOrientation)
    {
        //aqui realmente el doorOrientation está invertido
        if(doorOrientation==DoorOrientation.Left)
        {
            
        }
    }
}
