using UnityEngine;
using UnityEngine.InputSystem;

public class EvolutionBase : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SlimeEvolution targetSlime;
    [SerializeField] private PlayerInventory playerInventory;

    private bool playerInRange = false;

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();
    }

    private void Update()
    {
        if (!playerInRange || targetSlime == null || playerInventory == null)
            return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame)
        {
            bool evolved = targetSlime.TryEvolve(playerInventory);
            if (evolved)
            {
                MessagePanelUI.Instance?.EnqueueMessage("¡El slime ha evolucionado!");
            }
            else
            {
                MessagePanelUI.Instance?.EnqueueMessage("No tienes suficientes recursos para evolucionar.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            MessagePanelUI.Instance?.EnqueueMessage(
                "Estás en la base. Presiona E para intentar evolucionar al slime."
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Opcional: mensaje al salir
            // MessagePanelUI.Instance?.EnqueueMessage("Te alejaste de la base de evolución.");
        }
    }
}
