using UnityEngine;

public class HallwayDoor : MonoBehaviour
{
    void Start()
    {
        int r = Random.Range(0,3);
        if(r==0) gameObject.GetComponent<Door>().doorType = DoorType.Knock;
        if(r==1) Destroy(gameObject);
    }
}
