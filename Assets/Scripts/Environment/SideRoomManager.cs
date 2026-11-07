using UnityEngine;

public class SideRoomManager : MonoBehaviour
{
    public GameObject[] normalRooms;
    public GameObject[] knockRooms;
    GameObject activeSideRoom = null;

    public void GenerateSideRoom(DoorType doorType)
    {
        if(doorType==DoorType.Normal)
        {
            int r = Random.Range(0, normalRooms.Length);
            activeSideRoom = Instantiate(normalRooms[r]);
        }
        else
        {
            Debug.Log("no se ha creado una array de gameobjebtcs de tipo " + doorType);
        }
    }

    public void DeleteSideRoom()
    {
        if(activeSideRoom!=null) Destroy(activeSideRoom);
    }

    public void TeleportPlayer(DoorOrientation doorOrientation)
    {
        Transform newPos;

        if(doorOrientation == DoorOrientation.Left)
        {
            newPos = activeSideRoom.GetComponent<SideRoom>().leftSpawnPoint;
        }
        else
        {
            newPos = activeSideRoom.GetComponent<SideRoom>().rightSpawnPoint;
        }
        
        Transform playerTransform = GameObject.FindWithTag("Player").transform;

        Vector3 targetPosition = playerTransform.position;
        targetPosition.x = newPos.position.x;
        targetPosition.y = newPos.position.y;
        playerTransform.position = targetPosition;
        
    }
}
