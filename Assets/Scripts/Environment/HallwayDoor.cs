using UnityEngine;

public class HallwayDoor : MonoBehaviour
{
    void Start()
    {
        int r = Random.Range(0,5);
        if(r<2) Destroy(gameObject);
        if(r==3) gameObject.GetComponent<Door>().doorType = DoorType.Knock;
    }
}
