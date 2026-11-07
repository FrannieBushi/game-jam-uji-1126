using UnityEngine;

public class SideRoomManager : MonoBehaviour
{
    public GameObject[] normalRooms;
    public GameObject[] knockRooms;
    public GameObject activeSideRoom = null;

    Vector3 returnPosition;
    Transform player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    public void StoreReturnPosition()
    {
        returnPosition = player.position;
    }

    public void GenerateSideRoom(DoorType doorType, DoorOrientation doorOrientation)
    {
        if (doorType == DoorType.Normal)
        {
            int r = Random.Range(0, normalRooms.Length);
            activeSideRoom = Instantiate(normalRooms[r]);
            var sr = activeSideRoom.GetComponent<SideRoom>();
            sr.doorType = doorType;
            sr.doorOrientation = doorOrientation;
        }
        else
        {
            Debug.Log("no se ha creado un array de gameobjects de tipo " + doorType);
        }
    }

    public void DeleteSideRoom()
    {
        if (activeSideRoom != null) Destroy(activeSideRoom);
    }

    public void TeleportPlayer(DoorOrientation doorOrientation)
    {
        //cortesia de claude
        var room = activeSideRoom.GetComponent<SideRoom>();
        Transform spawn = (doorOrientation == DoorOrientation.Left)
            ? room.rightSpawnPoint
            : room.leftSpawnPoint;
        MovePlayer(spawn.position);
    }

    public void TeleportPlayerBack()
    {
        MovePlayer(returnPosition);
    }

    void MovePlayer(Vector3 pos)
    {
        Vector3 target = player.position;
        target.x = pos.x;
        target.y = pos.y;
        player.position = target;
    }
}