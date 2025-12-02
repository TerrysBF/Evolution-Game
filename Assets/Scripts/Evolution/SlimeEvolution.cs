using UnityEngine;

public class SlimeEvolution : MonoBehaviour
{
    [Header("Configuración de etapas")]
    public SlimeStage[] stages;

    [Tooltip("Etapa actual (índice en el arreglo stages)")]
    [SerializeField] private int currentStageIndex = 0;

    private void Start()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Apagar todos los modelos
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].model != null)
                stages[i].model.SetActive(i == currentStageIndex);
        }

        if (currentStageIndex >= 0 && currentStageIndex < stages.Length)
        {
            Debug.Log($"[SlimeEvolution] Etapa actual: {stages[currentStageIndex].stageName}");
        }
    }

    public bool TryEvolve(PlayerInventory inventory)
    {
        if (stages == null || stages.Length == 0)
        {
            Debug.LogWarning("[SlimeEvolution] No hay etapas configuradas.");
            return false;
        }

        if (currentStageIndex >= stages.Length - 1)
        {
            Debug.Log("[SlimeEvolution] Ya está en la última etapa, no puede evolucionar más.");
            return false;
        }

        SlimeStage currentStage = stages[currentStageIndex];
        ResourceCost[] cost = currentStage.evolutionCost;

        // Verificar que el jugador tenga todos los recursos
        foreach (var c in cost)
        {
            int have = inventory.GetAmount(c.type);
            if (have < c.amount)
            {
                Debug.Log($"[SlimeEvolution] Falta recurso: {c.type}. Tiene {have}/{c.amount}");
                return false;
            }
        }

        // Descontar recursos
        foreach (var c in cost)
        {
            inventory.SpendResource(c.type, c.amount);
        }

        // 3) Avanzar de etapa
        currentStageIndex++;
        Debug.Log($"[SlimeEvolution] Evolucionó a etapa: {stages[currentStageIndex].stageName}");
        UpdateVisual();

        return true;
    }

    public bool IsAtFinalStage
    {
        get
        {
            return stages != null && stages.Length > 0 &&
                   currentStageIndex >= stages.Length - 1;
        }
    }

    public ResourceCost[] GetCurrentEvolutionCost()
    {
        if (stages == null || stages.Length == 0)
            return null;

        if (currentStageIndex < 0 || currentStageIndex >= stages.Length)
            return null;

        return stages[currentStageIndex].evolutionCost;
    }


}
