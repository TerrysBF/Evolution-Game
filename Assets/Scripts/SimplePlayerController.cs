using UnityEngine;

/// <summary>
/// Controlador de jugador en tercera persona usando CharacterController.
/// 
/// - Se mueve con WASD (o flechas) relativo a la cámara.
/// - Gira hacia la dirección de movimiento.
/// - Incluye gravedad y salto opcional.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonPlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    /// <summary>
    /// Velocidad de movimiento del jugador en unidades por segundo.
    /// </summary>
    public float moveSpeed = 6f;

    /// <summary>
    /// Velocidad a la que el jugador rota hacia la dirección de movimiento.
    /// </summary>
    public float rotationSpeed = 10f;

    [Header("Gravedad / salto (opcional)")]
    /// <summary>
    /// Fuerza de gravedad aplicada al jugador (negativa hacia abajo).
    /// </summary>
    public float gravity = -9.81f;

    /// <summary>
    /// Altura del salto en unidades.
    /// </summary>
    public float jumpHeight = 1.5f;

    [Header("Referencias")]
    [Tooltip("Transform de la cámara que orbita al jugador.")]
    /// <summary>
    /// Transform de la cámara, usada para mover al jugador relativo
    /// a la dirección en que está mirando la cámara.
    /// </summary>
    public Transform cameraTransform;

    /// <summary>
    /// Referencia al CharacterController usado para mover al jugador.
    /// </summary>
    private CharacterController controller;

    /// <summary>
    /// Velocidad actual del jugador (se usa principalmente para la Y: gravedad/salto).
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// Indica si el jugador está tocando el suelo.
    /// </summary>
    private bool isGrounded;

    /// <summary>
    /// Obtiene y guarda la referencia al CharacterController.
    /// </summary>
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Lógica principal de movimiento y salto del jugador.
    /// Se llama una vez por frame.
    /// </summary>
    private void Update()
    {
        // Comprobar si está tocando el suelo
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            // Un valor pequeño negativo para mantenerlo pegado al piso
            velocity.y = -2f;
        }

        // Input WASD / flechas
        float h = Input.GetAxisRaw("Horizontal"); // A/D o ←/→
        float v = Input.GetAxisRaw("Vertical");   // W/S o ↑/↓
        Vector3 input = new Vector3(h, 0f, v);

        // Limitar el vector de entrada a longitud 1
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 moveDir = Vector3.zero;

        if (input.sqrMagnitude > 0.001f)
        {
            // --- Movimiento relativo a la cámara ---
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // Ignorar la componente vertical de la cámara
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Combinar entrada con ejes de la cámara
            moveDir = camForward * input.z + camRight * input.x;
            moveDir.Normalize();

            // Mover al player en esa dirección
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // Rotar al player hacia la dirección de movimiento
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // --- Salto opcional ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Fórmula para alcanzar una altura deseada con una gravedad dada
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- Gravedad ---
        velocity.y += gravity * Time.deltaTime;

        // Aplicar movimiento vertical (gravedad/salto)
        controller.Move(velocity * Time.deltaTime);
    }
}
