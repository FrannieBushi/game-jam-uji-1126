using UnityEngine;

public class Camarafollow : MonoBehaviour
{
    [SerializeField] private float smooth;
    [SerializeField] private Transform target;
    private Vector3 targetPosition;
    public bool fixedCamera = false;

    void Start()
    {
        if(target==null)
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
    void LateUpdate()
    {
        if (target == null) return;

        if(!fixedCamera)
        {
            //targetPosition.x = target.position.x;
            targetPosition.y = target.position.y;
            targetPosition.z = this.transform.position.z;
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, smooth * Time.deltaTime);
        }
    }

    public void FixCamera()
    {
        Vector3 pos = transform.position;
        pos.x = 0f;
        pos.y = -10.2f;
        transform.position = pos;
    }
}
