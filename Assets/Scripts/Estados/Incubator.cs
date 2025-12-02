using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Incubator : MonoBehaviour
{
    [Header("Slimes a crear")]
    [SerializeField] private List<SlimeEvolution> slimePrefabs = new List<SlimeEvolution>();

    [Header("Modo de selección de slime")]
    [SerializeField] private IncubatorSpawnMode spawnMode = IncubatorSpawnMode.Sequential;
    [SerializeField] private int sequentialStartIndex = 0; // desde qué índice empezar en modo secuencial

    [Header("Bases disponibles (slots de proceso)")]
    [SerializeField] private List<SlimeBase> slimeBases = new List<SlimeBase>();

    [Header("Config")]
    [SerializeField] private bool allowReuseFinishedSlots = false;

    private bool playerInRange = false;
    private int nextSequentialIndex = 0;

    public enum IncubatorSpawnMode
    {
        Sequential,
        Random
    }


    private void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        if (slimePrefabs.Count > 0)
        {
            nextSequentialIndex = Mathf.Clamp(sequentialStartIndex, 0, slimePrefabs.Count - 1);
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.eKey.wasPressedThisFrame)
        {
            TryCreateNewSlime();
        }
    }

    private void TryCreateNewSlime()
    {
        if (slimePrefabs == null || slimePrefabs.Count == 0)
        {
            MessagePanelUI.Instance?.EnqueueMessage(
                "Incubadora: No hay ningún slime prefab asignado."
            );
            return;
        }

        SlimeBase freeBase = FindFreeBase();

        if (freeBase == null)
        {
            MessagePanelUI.Instance?.EnqueueMessage(
                "Incubadora: No hay bases libres para asignar un nuevo slime."
            );
            return;
        }

        SlimeEvolution chosenPrefab = GetNextSlimePrefab();
        if (chosenPrefab == null)
        {
            MessagePanelUI.Instance?.EnqueueMessage(
                "Incubadora: No se pudo seleccionar un slime válido."
            );
            return;
        }

        freeBase.AssignNewSlime(chosenPrefab);

        int index = slimeBases.IndexOf(freeBase);
        string baseName = (index >= 0) ? $"Base {index + 1}" : "una base";

        MessagePanelUI.Instance?.EnqueueMessage(
            $"Nuevo proceso creado en {baseName} con slime '{chosenPrefab.name}'. Estado: New → Ready."
        );
    }

    private SlimeBase FindFreeBase()
    {
        foreach (var b in slimeBases)
        {
            if (b == null) continue;

            if (b.State == SlimeSlotState.Empty)
                return b;

            if (allowReuseFinishedSlots && b.State == SlimeSlotState.Finished)
                return b;
        }

        return null;
    }

    private SlimeEvolution GetNextSlimePrefab()
    {
        if (slimePrefabs == null || slimePrefabs.Count == 0)
            return null;

        switch (spawnMode)
        {
            case IncubatorSpawnMode.Random:
                {
                    int idx = Random.Range(0, slimePrefabs.Count);
                    return slimePrefabs[idx];
                }

            case IncubatorSpawnMode.Sequential:
            default:
                {
                    var prefab = slimePrefabs[nextSequentialIndex];
                    nextSequentialIndex = (nextSequentialIndex + 1) % slimePrefabs.Count;
                    return prefab;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            MessagePanelUI.Instance?.EnqueueMessage(
                "Incubadora: Presiona E para crear un nuevo slime (estado New)."
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
