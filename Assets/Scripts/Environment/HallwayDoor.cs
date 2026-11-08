using UnityEngine;

public class HallwayDoor : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        int r = Random.Range(0,4);
        if(r<2) Destroy(gameObject);
        if(r==3) gameObject.GetComponent<Door>().doorType = DoorType.Knock;
    }

    void Update()
    {
        if(DoorType.Knock == GetComponent<Door>().doorType)
        {
            Debug.Log("hola!");
            animator.Play("knock");
        }
    }
}
