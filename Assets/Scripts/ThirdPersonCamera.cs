using UnityEngine;

/// <summary>
/// Cámara de tercera persona estilo Roblox.
/// 
/// - Orbita con el mouse alrededor del jugador.
/// - Usa la rueda del mouse para acercar/alejar (zoom).
/// - La cámara siempre mira al objetivo (target).
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Objetivo")]
    /// <summary>
    /// Transform del objetivo que la cámara va a seguir (normalmente el jugador).
    /// </summary>
    public Transform target;

    /// <summary>
    /// Desplazamiento vertical/horizontal respecto al objetivo.
    /// Sirve para colocar la cámara un poco encima del jugador.
    /// </summary>
    public Vector3 targetOffset = new Vector3(0f, 2f, 0f); // altura sobre el player

    [Header("Órbita")]
    /// <summary>
    /// Distancia actual de la cámara al objetivo.
    /// </summary>
    public float distance = 6f;

    /// <summary>
    /// Distancia mínima permitida para el zoom (no acercarse demasiado).
    /// </summary>
    public float minDistance = 3f;

    /// <summary>
    /// Distancia máxima permitida para el zoom (no alejarse demasiado).
    /// </summary>
    public float maxDistance = 10f;

    /// <summary>
    /// Sensibilidad del mouse para rotar la cámara.
    /// </summary>
    public float mouseSensitivity = 120f;

    /// <summary>
    /// Límite inferior del ángulo vertical (pitch) de la cámara, en grados.
    /// </summary>
    public float minPitch = -20f;

    /// <summary>
    /// Límite superior del ángulo vertical (pitch) de la cámara, en grados.
    /// </summary>
    public float maxPitch = 60f;

    [Header("Colisión opcional")]
    /// <summary>
    /// Capas con las que la cámara puede chocar (paredes, obstáculos, etc.).
    /// </summary>
    public LayerMask collisionMask;

    /// <summary>
    /// Radio de la esfera para el SphereCast, usado para detectar colisiones de cámara.
    /// </summary>
    public float collisionRadius = 0.2f;

    /// <summary>
    /// Ángulo horizontal acumulado de la cámara (giro alrededor del jugador).
    /// </summary>
    private float yaw;

    /// <summary>
    /// Ángulo vertical acumulado de la cámara (mirar arriba/abajo).
    /// </summary>
    private float pitch;

    /// <summary>
    /// Inicializa yaw y pitch según la posición inicial de la cámara
    /// y configura el estado del cursor (bloqueado/visible).
    /// </summary>
    private void Start()
    {
        if (target != null)
        {
            // Calcular dirección inicial desde el objetivo hacia la cámara
            Vector3 dir = (transform.position - (target.position + targetOffset)).normalized;

            // Convertir la dirección en ángulos yaw (Y) y pitch (X)
            yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            pitch = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        }

        // Si no quieres bloquear el cursor, comenta estas dos líneas.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Actualiza la posición y rotación de la cámara después de que
    /// todos los objetos se hayan movido (LateUpdate).
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        // --- Entrada de mouse para rotar la cámara ---
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Yaw = giro horizontal, Pitch = giro vertical
        yaw += mouseX * mouseSensitivity * Time.deltaTime;
        pitch -= mouseY * mouseSensitivity * Time.deltaTime;

        // Limitar el ángulo vertical
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Crear la rotación final de la cámara
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // --- Zoom con la rueda del mouse ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            distance -= scroll * 5f;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // Posición objetivo (jugador + offset)
        Vector3 targetPos = target.position + targetOffset;

        // Posición deseada de la cámara a cierta distancia detrás del target
        Vector3 desiredPos = targetPos - rotation * Vector3.forward * distance;

        // --- Evitar que la cámara atraviese paredes (opcional) ---
        if (collisionMask.value != 0)
        {
            Vector3 dir = (desiredPos - targetPos).normalized;

            // SphereCast desde el target hacia la cámara
            if (Physics.SphereCast(targetPos, collisionRadius, dir,
                out RaycastHit hit, distance, collisionMask))
            {
                // Colocar la cámara justo antes del obstáculo
                desiredPos = targetPos + dir * (hit.distance - 0.1f);
            }
        }

        // Aplicar posición y rotación a la cámara
        transform.position = desiredPos;
        transform.rotation = rotation;
    }
}
