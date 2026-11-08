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
        GenerateSideRoom(DoorType.Normal, DoorOrientation.Left);
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
            int r = Random.Range(0, knockRooms.Length);
            activeSideRoom = Instantiate(knockRooms[r]);
            var sr = activeSideRoom.GetComponent<SideRoom>();
            sr.doorType = doorType;
            sr.doorOrientation = doorOrientation;
        }
    }

    public void DeleteSideRoom()
    {
        if (activeSideRoom != null) Destroy(activeSideRoom);
    }

    public void TeleportPlayer(DoorOrientation doorOrientation)
    {
        var room = activeSideRoom.GetComponent<SideRoom>();

        Transform spawn;
        float offsetX;
        if (doorOrientation == DoorOrientation.Left)
        {
            spawn = room.rightSpawnPoint;
            offsetX = -0.8f;
        }
        else
        {
            spawn = room.leftSpawnPoint;
            offsetX = 0.8f;
        }
        MovePlayer(spawn.position + new Vector3(offsetX, -0.2f, 0f));
        GameObject.FindWithTag("MainCamera").GetComponent<Camarafollow>().FixCamera();
        GameObject.FindWithTag("MainCamera").GetComponent<Camarafollow>().fixedCamera=true;
    }

    public void TeleportPlayerBack()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<Camarafollow>().fixedCamera=false;
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