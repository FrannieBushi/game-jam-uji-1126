using UnityEngine;

public class Crystal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().RestoreHealth(10);
            Destroy(gameObject);
        }
    }
}
