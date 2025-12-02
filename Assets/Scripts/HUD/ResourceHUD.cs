using UnityEngine;
using TMPro;

public class ResourceHUD : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerInventory inventory;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI slimeGooText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI mushroomText;
    [SerializeField] private TextMeshProUGUI flowerText;
    [SerializeField] private TextMeshProUGUI rockText;
    [SerializeField] private TextMeshProUGUI leavesText;
    [SerializeField] private TextMeshProUGUI ivyText;
    [SerializeField] private TextMeshProUGUI moldywoodText;
    [SerializeField] private TextMeshProUGUI rootsText;

    void Awake()
    {
        // Si no lo asignas a mano, intenta encontrarlo
        if (inventory == null)
            inventory = FindObjectOfType<PlayerInventory>();
    }

    void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += RefreshUI;
            RefreshUI(); // actualizar al inicio
        }
    }

    void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        if (slimeGooText != null)
            slimeGooText.text = $"Slime Goo: {inventory.GetAmount(ResourceType.SlimeGoo)}";

        if (woodText != null)
            woodText.text = $"Wood: {inventory.GetAmount(ResourceType.Wood)}";

        if (mushroomText != null)
            mushroomText.text = $"Mushrooms: {inventory.GetAmount(ResourceType.Mushroom)}";

        if (flowerText != null)
            flowerText.text = $"Flowers: {inventory.GetAmount(ResourceType.Flower)}";

        if (rockText != null)
            rockText.text = $"Rock: {inventory.GetAmount(ResourceType.Rock)}";

        if (leavesText != null)
            leavesText.text = $"Leaves: {inventory.GetAmount(ResourceType.Leaves)}";

        if (ivyText != null)
            ivyText.text = $"Ivy: {inventory.GetAmount(ResourceType.Ivy)}";

        if (moldywoodText != null)
            moldywoodText.text = $"Moldy Wood: {inventory.GetAmount(ResourceType.MoldyWood)}";

        if (rootsText != null)
            rootsText.text = $"Roots: {inventory.GetAmount(ResourceType.Roots)}";
    }
}
