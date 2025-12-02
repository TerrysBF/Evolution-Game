using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerResourceCollector : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;

    private ResourceNode nodeInRange;

    void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame && nodeInRange != null)
        {
            nodeInRange.Harvest(inventory);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var node = other.GetComponent<ResourceNode>();
        if (node != null)
        {
            nodeInRange = node;

            string msg = $"Estás cerca de {node.resourceType}. Presiona E para recolectar.";
            MessagePanelUI.Instance?.EnqueueMessage(msg);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var node = other.GetComponent<ResourceNode>();
        if (node != null && node == nodeInRange)
        {
            nodeInRange = null;
            // Opcional: puedes mandar un mensaje al salir
            // MessagePanelUI.Instance?.EnqueueMessage("Te alejaste del recurso.");
        }
    }
}
