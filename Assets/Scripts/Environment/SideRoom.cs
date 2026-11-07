using Unity.VisualScripting;
using UnityEngine;

public class SideRoom : MonoBehaviour
{
    public bool isLocked;
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;
    [HideInInspector] public DoorType doorType;
    [HideInInspector] public DoorOrientation doorOrientation;
    FloorGenerator fg;

    void Start()
    {
        GameObject door = null;
        fg = GameObject.FindWithTag("GameController").GetComponent<FloorGenerator>();
        if(doorOrientation==DoorOrientation.Left || doorOrientation==DoorOrientation.BackLeft )
            door = fg.SpawnDoor(doorType, doorOrientation, leftSpawnPoint);
        if(doorOrientation==DoorOrientation.Right || doorOrientation==DoorOrientation.BackRight )
            door = fg.SpawnDoor(doorType, doorOrientation, rightSpawnPoint);

        if(isLocked)
            door.GetComponent<Door>().doorType = DoorType.Locked;
        door.transform.SetParent(gameObject.transform);
    }

    public void StoreOldPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");
        
        if (player != null && fg.oldPlayerPosition != null)
        {
            fg.oldPlayerPosition.position = new Vector3(
                player.transform.position.x, 
                player.transform.position.y, 
                fg.oldPlayerPosition.position.z
            );
        }
    }

    public Transform GetOldPosition()
    {
        return fg.oldPlayerPosition;
    }
}
