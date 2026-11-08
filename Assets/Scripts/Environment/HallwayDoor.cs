using UnityEngine;
using System.Collections;

public class HallwayDoor : MonoBehaviour
{
    [Header("Knock")]
    [SerializeField] private float minKnockInterval = 2f;
    [SerializeField] private float maxKnockInterval = 6f;

    Animator animator;
    Door door;

    void Start()
    {
        animator = GetComponent<Animator>();
        door = GetComponent<Door>();

        int r = Random.Range(0, 4);
        if (r < 2)
        {
            Destroy(gameObject);
            return;
        }
        if (r == 3) door.doorType = DoorType.Knock;

        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minKnockInterval, maxKnockInterval));

            // Solo suena si la puerta es de tipo Knock (por si cambia en tiempo de ejecución).
            if (door.doorType == DoorType.Knock)
                animator.Play("knock", 0, 0f);
        }
    }
}