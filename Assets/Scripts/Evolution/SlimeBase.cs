using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SlimeBase : MonoBehaviour
{
    [Header("Estado del slot (debug)")]
    [SerializeField] private SlimeSlotState state = SlimeSlotState.Empty;

    [Header("Slime actual en esta base")]
    [SerializeField] private Transform slimeSpawnPoint;      // dónde se coloca el slime/huevo
    [SerializeField] private float spawnYOffset = 0.5f;      // offset hacia arriba para que no se hunda


    [Header("Simulación de ejecución")]
    [SerializeField] private float evolutionDuration = 1.0f; // tiempo que dura el estado Running

    private SlimeEvolution currentSlime;
    private PlayerInventory playerInventory;
    private bool playerInRange = false;
    private bool isProcessing = false; // evitar spamear E durante Running

    public SlimeSlotState State => state;
    public SlimeEvolution CurrentSlime => currentSlime;

    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();

        // Siempre empezamos vacíos. La incubadora se encargará de llenar la base.
        SetState(SlimeSlotState.Empty);
    }

    // --- Helper para cambiar estado en un solo lugar ---
    private void SetState(SlimeSlotState newState)
    {
        if (state == newState) return;
        state = newState;
        // Aquí tu HUD flotante ya refleja el cambio con SlimeBaseStateUI
    }

    /// <summary>
    /// Llamado por la Incubadora para "crear" un nuevo slime en esta base.
    /// </summary>
    public void AssignNewSlime(SlimeEvolution slimePrefab)
    {
        // Solo aceptamos nuevo slime si la base está vacía o terminada
        if (state != SlimeSlotState.Empty && state != SlimeSlotState.Finished)
        {
            Debug.LogWarning($"[SlimeBase] Intentas asignar un slime a una base en estado {state}");
            return;
        }

        // Si ya había un slime (por ejemplo, uno terminado), lo destruimos
        if (currentSlime != null)
        {
            Destroy(currentSlime.gameObject);
            currentSlime = null;
        }

        // Instanciar el nuevo slime en el spawn point
        Vector3 basePos = slimeSpawnPoint != null ? slimeSpawnPoint.position : transform.position;
        Vector3 pos = basePos + Vector3.up * spawnYOffset;
        Quaternion rot = slimeSpawnPoint != null ? slimeSpawnPoint.rotation : transform.rotation;

        currentSlime = Instantiate(slimePrefab, pos, rot);

        // Nuevo proceso listo para entrar a la cola de listos
        SetState(SlimeSlotState.Ready);
    }



    private void Update()
    {
        if (!playerInRange || playerInventory == null)
            return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (isProcessing)
        {
            MessagePanelUI.Instance?.EnqueueMessage("Esta base ya está en ejecución (Running).");
            return;
        }

        switch (state)
        {
            case SlimeSlotState.Empty:
                MessagePanelUI.Instance?.EnqueueMessage(
                    "Esta base está vacía. Usa la incubadora para crear un slime."
                );
                break;

            case SlimeSlotState.New:
            case SlimeSlotState.Ready:
            case SlimeSlotState.Waiting:
                StartCoroutine(EvolutionRoutine());
                break;

            case SlimeSlotState.Running:
                MessagePanelUI.Instance?.EnqueueMessage(
                    "Esta base ya está evolucionando (Running)."
                );
                break;

            case SlimeSlotState.Finished:
                MessagePanelUI.Instance?.EnqueueMessage(
                    "Este slime ya alcanzó su evolución final (Terminado)."
                );
                break;
        }
    }

    private IEnumerator EvolutionRoutine()
    {
        if (currentSlime == null)
        {
            MessagePanelUI.Instance?.EnqueueMessage(
                "No hay slime asignado a esta base."
            );
            SetState(SlimeSlotState.Empty);
            yield break;
        }

        if (currentSlime.IsAtFinalStage)
        {
            SetState(SlimeSlotState.Finished);
            MessagePanelUI.Instance?.EnqueueMessage(
                "Este slime ya está en su etapa final."
            );
            yield break;
        }

        isProcessing = true;
        SetState(SlimeSlotState.Running);
        MessagePanelUI.Instance?.EnqueueMessage(
            "Slime en ejecución (Running)..."
        );

        // Simulas tiempo de CPU / ejecución
        yield return new WaitForSeconds(evolutionDuration);

        // Después del "tiempo de CPU" intentamos evolucionar
        bool evolved = currentSlime.TryEvolve(playerInventory);

        if (!evolved)
        {
            if (!currentSlime.IsAtFinalStage)
            {
                SetState(SlimeSlotState.Waiting);
                MessagePanelUI.Instance?.EnqueueMessage(
                    "No tienes suficientes recursos. El slime queda en pausa (Waiting)."
                );
            }
            else
            {
                SetState(SlimeSlotState.Finished);
                MessagePanelUI.Instance?.EnqueueMessage(
                    "Este slime ya está en su etapa final."
                );
            }
        }
        else
        {
            if (currentSlime.IsAtFinalStage)
            {
                SetState(SlimeSlotState.Finished);
                MessagePanelUI.Instance?.EnqueueMessage(
                    "¡El slime alcanzó su evolución final (Terminado)!"
                );
            }
            else
            {
                // Después de evolucionar y no ser final, pasa a espera para la siguiente etapa
                SetState(SlimeSlotState.Waiting);
                MessagePanelUI.Instance?.EnqueueMessage(
                    "El slime evolucionó. Está en pausa (Waiting) para la siguiente etapa."
                );
            }
        }

        isProcessing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            MessagePanelUI.Instance?.EnqueueMessage(
                "Base de slime. Presiona E para interactuar."
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }


}
