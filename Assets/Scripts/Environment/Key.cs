using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out Player player))
        {
            player.GiveKey();

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
            }

            Destroy(gameObject);
        }
    }
}
