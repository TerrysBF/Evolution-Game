using UnityEngine;
using TMPro;

public class SlimeBaseCostUI : MonoBehaviour
{
    [SerializeField] private SlimeBase slimeBase;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private bool faceCamera = true;

    private Camera mainCam;

    private void Awake()
    {
        if (slimeBase == null)
            slimeBase = GetComponentInParent<SlimeBase>();

        if (playerInventory == null)
            playerInventory = FindObjectOfType<PlayerInventory>();

        mainCam = Camera.main;
    }

    private void Start()
    {
        RefreshText();
    }

    private void Update()
    {
        RefreshText();

        if (faceCamera && mainCam != null)
        {
            // Billboard hacia la cámara
            transform.LookAt(
                transform.position + mainCam.transform.rotation * Vector3.forward,
                mainCam.transform.rotation * Vector3.up
            );
        }
    }

    private void RefreshText()
    {
        if (costText == null || slimeBase == null)
            return;

        var slime = slimeBase.CurrentSlime;

        // Base vacía
        if (slimeBase.State == SlimeSlotState.Empty || slime == null)
        {
            costText.text = "Sin proceso";
            return;
        }

        // Slime finalizado
        if (slime.IsAtFinalStage || slimeBase.State == SlimeSlotState.Finished)
        {
            costText.text = "Completado";
            return;
        }

        // Coste de la etapa actual
        var costs = slime.GetCurrentEvolutionCost();
        if (costs == null || costs.Length == 0)
        {
            costText.text = "Sin costo definido";
            return;
        }

        // Construimos el texto tipo:
        // Wood: 2/5
        // Mushrooms: 1/3
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Siguiente evolución:");

        foreach (var c in costs)
        {
            int have = playerInventory != null ? playerInventory.GetAmount(c.type) : 0;
            string nombre = GetResourceDisplayName(c.type);

            sb.AppendLine($"{nombre}: {have}/{c.amount}");
        }

        costText.text = sb.ToString();
    }

    private string GetResourceDisplayName(ResourceType type)
    {
        // Aquí puedes traducir los nombres a algo más bonito
        switch (type)
        {
            case ResourceType.SlimeGoo: return "Slime Goo";
            case ResourceType.Wood: return "Wood";
            case ResourceType.Mushroom: return "Mushrooms";
            // agrega los que tengas
            default: return type.ToString();
        }
    }
}
