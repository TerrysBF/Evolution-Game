using UnityEngine;

public class BlackHoleStretch : MonoBehaviour
{
    [SerializeField] Transform blackHole;  // arrastra aquí tu BlackHole
    [SerializeField] float effectRadius = 15f;
    [SerializeField] float maxStretch = 3f; // qué tanto se alargan

    Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!blackHole) return;

        float dist = Vector3.Distance(transform.position, blackHole.position);

        // Si está fuera del área, regresa a su escala normal
        if (dist > effectRadius)
        {
            transform.localScale = originalScale;
            return;
        }

        // t = 0 (lejos), t = 1 (muy cerca)
        float t = 1f - (dist / effectRadius);

        // Mirar hacia el agujero negro, para que el estiramiento se alinee
        transform.LookAt(blackHole.position);

        float stretchFactor = Mathf.Lerp(1f, maxStretch, t);

        // Escala: ancho normal, largo estirado hacia adelante (eje Z local)
        transform.localScale = new Vector3(
            originalScale.x,
            originalScale.y,
            originalScale.z * stretchFactor
        );
    }
}
