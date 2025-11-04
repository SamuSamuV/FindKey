using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OllamaChatUI : MonoBehaviour
{
    [Header("References")]
    public OllamaClient ollamaClient;
    public TMP_InputField inputField;
    public TextMeshProUGUI chatOutput;
    public Button sendButton;

    [Header("Prompt settings")]
    [TextArea(3, 10)]
    public string systemInstruction = "Eres un NPC conciso en un videojuego. Responde con texto claro y breve al jugador.";

    void Awake()
    {
        //Si el clon no tiene referencias, las busca en sus hijos
        if (inputField == null)
            inputField = GetComponentInChildren<TMP_InputField>(true);
        if (chatOutput == null)
            chatOutput = GetComponentInChildren<TextMeshProUGUI>(true);
        if (sendButton == null)
            sendButton = GetComponentInChildren<Button>(true);
        if (ollamaClient == null)
            ollamaClient = FindAnyObjectByType<OllamaClient>(); // busca el cliente en la escena
    }

    void Start()
    {
        if (sendButton != null) sendButton.onClick.AddListener(OnSendClicked);
        if (inputField != null) inputField.onSubmit.AddListener(OnInputSubmit);
    }

    void OnDestroy()
    {
        if (sendButton != null) sendButton.onClick.RemoveListener(OnSendClicked);
        if (inputField != null) inputField.onSubmit.RemoveListener(OnInputSubmit);
    }

    void OnInputSubmit(string unused) => OnSendClicked();

    public void OnSendClicked()
    {
        if (ollamaClient == null)
        {
            Debug.LogError("No se encontró OllamaClient en la escena.");
            return;
        }
        if (inputField == null || chatOutput == null)
        {
            Debug.LogError("Referencias UI nulas (inputField o chatOutput).");
            return;
        }

        string text = inputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        AppendToChat("Player: " + text);
        inputField.text = "";
        inputField.ActivateInputField();

        sendButton.interactable = false;

        string prompt = $"{systemInstruction}\n\nJugador: {text}\nNPC:";
        StartCoroutine(ollamaClient.SendPrompt(prompt, OnSuccess, OnError));
    }

    void OnSuccess(string response)
    {
        AppendToChat("NPC: " + response);
        sendButton.interactable = true;
    }

    void OnError(string error)
    {
        AppendToChat("Error: " + error);
        sendButton.interactable = true;
    }

    void AppendToChat(string line)
    {
        if (chatOutput == null)
        {
            Debug.LogError("chatOutput no asignado en el clon.");
            return;
        }

        chatOutput.text += (string.IsNullOrEmpty(chatOutput.text) ? "" : "\n") + line;
    }
}