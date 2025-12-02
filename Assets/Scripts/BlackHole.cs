using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("Radio de influencia")]
    [SerializeField] float gravityRadius = 15f;   
    [SerializeField] float killRadius = 2f;       

    [Header("Fuerza de gravedad")]
    [SerializeField] float gravityStrength = 50f; 

    [Header("Debug")]
    [SerializeField] bool debugGizmos = true;

    void FixedUpdate()
    {
        // Busca todo lo que tenga collider dentro del radio
        Collider[] hits = Physics.OverlapSphere(transform.position, gravityRadius);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            Rigidbody rb = hit.attachedRigidbody;
            if (rb == null) continue; // solo se afectan objetos físicos

            Vector3 dir = (transform.position - rb.position);
            float dist = dir.magnitude;
            if (dist < 0.01f) continue;

            // Normalizar dirección
            dir /= dist;

            // más cerca = fuerza MUCHO mayor
            float falloff = 1f / (dist * dist);

            rb.AddForce(dir * gravityStrength * falloff, ForceMode.Acceleration);

            // Si cruza el "horizonte de sucesos", lo destruimos
            if (dist <= killRadius)
            {
                Destroy(rb.gameObject);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!debugGizmos) return;

        // Radio de gravedad
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);

        // Horizonte de sucesos
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRadius);
    }
}

