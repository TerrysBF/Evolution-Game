using System.Collections;
using UnityEngine;

public class WormholePortal : MonoBehaviour
{
    [Header("Referencia al otro portal")]
    [SerializeField] WormholePortal otherPortal;

    [Header("Ajustes de salida")]
    [SerializeField] float exitOffset = 2f;      // Qué tan lejos sale del otro portal
    [SerializeField] bool preserveVelocity = true;
    [SerializeField] float disableTime = 0.3f;   // Tiempo que ambos portales estarán apagados

    Collider myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider>();
        if (myCollider) myCollider.isTrigger = true; // nos aseguramos que sea trigger
    }

    void OnTriggerEnter(Collider other)
    {
        if (!otherPortal) return;
        if (myCollider == null || !myCollider.enabled) return;

        Rigidbody rb = other.attachedRigidbody;
        if (!rb) return;

        StartCoroutine(TeleportRoutine(rb));
    }

    IEnumerator TeleportRoutine(Rigidbody rb)
    {
        // Desactivar colliders de ambos portales
        Collider otherCol = otherPortal.GetComponent<Collider>();

        if (myCollider) myCollider.enabled = false;
        if (otherCol) otherCol.enabled = false;

        // Guardamos velocidad
        Vector3 originalVel = rb.linearVelocity;

        // Calculamos nueva posición y rotación
        Transform target = otherPortal.transform;
        Transform t = rb.transform;

        Vector3 newPos = target.position + target.forward * exitOffset;
        t.position = newPos;
        t.rotation = target.rotation;

        // Mantener velocidad alineada con el portal de salida
        if (preserveVelocity)
        {
            rb.linearVelocity = target.forward * originalVel.magnitude;
        }

        // Esperamos un ratito con ambos portales apagados
        yield return new WaitForSeconds(disableTime);

        // Reactivamos colliders
        if (myCollider) myCollider.enabled = true;
        if (otherCol) otherCol.enabled = true;
    }
}
