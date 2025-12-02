using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePanelUI : MonoBehaviour
{
    public static MessagePanelUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Config")]
    [SerializeField] private float messageDuration = 2f; // segundos por mensaje

    private readonly Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    /// <summary>
    /// Agrega un mensaje a la cola para mostrarse en pantalla.
    /// </summary>
    public void EnqueueMessage(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
            return;

        messageQueue.Enqueue(msg);

        if (!isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (messageQueue.Count > 0)
        {
            string msg = messageQueue.Dequeue();

            if (panel != null) panel.SetActive(true);
            if (messageText != null) messageText.text = msg;

            yield return new WaitForSeconds(messageDuration);
        }

        if (panel != null) panel.SetActive(false);
        isShowing = false;
    }
}
