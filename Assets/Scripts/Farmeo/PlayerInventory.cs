using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    // Evento para que el HUD se actualice (lo usaremos después)
    public event Action OnInventoryChanged;

    public void AddResource(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;

        resources[type] += amount;
        OnInventoryChanged?.Invoke();
    }

    public int GetAmount(ResourceType type)
    {
        if (resources.TryGetValue(type, out int value))
            return value;
        return 0;
    }

    // Esto lo usaremos cuando evolucione el slime
    public bool HasResources(ResourceType type, int requiredAmount)
    {
        return GetAmount(type) >= requiredAmount;
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (!HasResources(type, amount))
            return false;

        resources[type] -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }
}
