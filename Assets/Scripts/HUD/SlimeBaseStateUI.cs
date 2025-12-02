using UnityEngine;
using TMPro;

public class SlimeBaseStateUI : MonoBehaviour
{
    [SerializeField] private SlimeBase slimeBase;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private bool faceCamera = true;

    private Camera mainCam;
    private SlimeSlotState lastState;

    private void Awake()
    {
        if (slimeBase == null)
            slimeBase = GetComponentInParent<SlimeBase>();

        mainCam = Camera.main;
    }

    private void Start()
    {
        if (slimeBase != null)
        {
            lastState = slimeBase.State;
            UpdateLabel(lastState);
        }
    }

    private void Update()
    {
        if (slimeBase == null || stateText == null)
            return;

        // Actualizar texto si el estado cambió
        if (slimeBase.State != lastState)
        {
            lastState = slimeBase.State;
            UpdateLabel(lastState);
        }

        // Que el canvas mire a la cámara si quieres
        if (faceCamera && mainCam != null)
        {
            transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                             mainCam.transform.rotation * Vector3.up);
        }
    }

    private void UpdateLabel(SlimeSlotState state)
    {
        string label = state.ToString();

        // Si quieres textos en español más bonitos:
        switch (state)
        {
            case SlimeSlotState.Empty:
                label = "Vacío";
                break;
            case SlimeSlotState.New:
                label = "Nuevo";
                break;
            case SlimeSlotState.Ready:
                label = "Listo";
                break;
            case SlimeSlotState.Running:
                label = "En proceso";
                break;
            case SlimeSlotState.Waiting:
                label = "En pausa";
                break;
            case SlimeSlotState.Finished:
                label = "Terminado";
                break;
        }

        stateText.text = label;
    }
}
