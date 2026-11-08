using UnityEngine;

public class LuzDireccion : MonoBehaviour
{
    [SerializeField] private Transform luz;         
    [SerializeField] private float velocidadGiro = 10f;

    private Vector2 ultimaDireccion = Vector2.down;

    void Update()
    {
        Vector2 mov = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (mov != Vector2.zero)
            ultimaDireccion = mov;

        float angulo = Mathf.Atan2(ultimaDireccion.y, ultimaDireccion.x) * Mathf.Rad2Deg - 90f;
        Quaternion objetivo = Quaternion.Euler(0f, 0f, angulo);
        luz.rotation = Quaternion.Lerp(luz.rotation, objetivo, velocidadGiro * Time.deltaTime);
    }
}