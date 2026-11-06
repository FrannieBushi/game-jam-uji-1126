using UnityEngine;

public class Camarafollow : MonoBehaviour
{
    [SerializeField] private float smooth;
    [SerializeField] private Transform target;
    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (target == null) return;

        targetPosition.x = target.position.x;
        // targetPosition.y = target.position.y;
        targetPosition.z = this.transform.position.z;

        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, smooth * Time.deltaTime);
    }
}