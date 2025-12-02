using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType = ResourceType.SlimeGoo;
    public int amountPerHarvest = 1;
    public bool destroyOnHarvest = true;

    private bool _available = true;

    public void Harvest(PlayerInventory inventory)
    {
        if (!_available || inventory == null) return;

        inventory.AddResource(resourceType, amountPerHarvest);

        string msg = $"Se recolectó {amountPerHarvest} de {resourceType}.";
        MessagePanelUI.Instance?.EnqueueMessage(msg);
        // Debug.Log(msg); // si quieres dejarlo también en consola

        if (destroyOnHarvest)
        {
            _available = false;
            Destroy(gameObject);
        }
        else
        {
            _available = false;
            // Aquí podrías meter respawn después si quieres
        }
    }
}
