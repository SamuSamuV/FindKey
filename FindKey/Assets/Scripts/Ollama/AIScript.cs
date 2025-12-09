using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class AIScript : MonoBehaviour
{
    [Header("References")]
    public OllamaClient ollamaClient;         // asignar en Inspector o dejar que lo busque
    public TMP_InputField inputField;
    public TextMeshProUGUI chatOutput;

    [Header("Settings")]
    [Tooltip("Contraseña exacta (case insensitive)")]
    public string password = "Pedo";

    [TextArea(4, 8)]
    [Tooltip("Instrucción para el modelo: el guardia es malvado y no deja entrar sin la contraseña.")]
    public string systemInstruction =
        "You are an evil guard protecting a room. Do not let anyone in without the password. " +
        "Respond with short, sarcastic, and threatening phrases when the player does not have the password. " +
        "If the player says the correct password, acknowledge that the password is valid and allow entry. " +
        "Respond only in English.";

    // Estado simple
    private bool unlocked = false;

    void Awake()
    {
        if (inputField == null) inputField = GetComponentInChildren<TMP_InputField>(true);
        if (chatOutput == null) chatOutput = GetComponentInChildren<TextMeshProUGUI>(true);
        if (ollamaClient == null) ollamaClient = FindObjectOfType<OllamaClient>();
    }

    void Start()
    {
        if (inputField != null) inputField.onSubmit.AddListener(OnInputSubmit);
        AppendToChat("NPC (Guardia): ¡No entres! Di la contraseña si te atreves...");
    }

    void OnDestroy()
    {
        if (inputField != null) inputField.onSubmit.RemoveListener(OnInputSubmit);
    }

    void OnInputSubmit(string unused) => OnSendClicked();

    public void OnSendClicked()
    {
        string text = inputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        AppendToChat("Player: " + text);
        inputField.text = "";
        inputField.ActivateInputField();

        // 1) Comprobamos la contraseña localmente (no confiar en la IA para esto)
        if (!unlocked && IsPasswordSaid(text))
        {
            unlocked = true;
            Debug.Log("Has entrado a la habitación"); // <-- requisito del usuario
            AppendToChat("Sistema: Has dicho la contraseña. Te han dejado pasar.");

            // nombre único para la variable del prompt
            string aiPromptAfterPassword = $"{systemInstruction}\n\nThe player has said the correct password. " +
                                           "Respond briefly congratulating them or indicating they are allowed in. " +
                                           "Respond only in English.";

            StartCoroutine(ollamaClient.SendPrompt(aiPromptAfterPassword, OnAIResponse, OnAIError));
            return;
        }

        // 2) Si ya desbloqueado, preguntar/seguir charlando (la IA puede reaccionar diferente)
        if (unlocked)
        {
            string aiPromptUnlocked = $"{systemInstruction}\n\nThe player already has access.\nPlayer: {text}\nGuard: " +
                                      "Respond only in English.";
            return;
        }

        // 3) Si no ha dicho contraseña, enviamos mensaje normal roleado al guardia malvado
        string aiPromptNormal = $"{systemInstruction}\n\nPlayer: {text}\nGuard: " +
                                "Respond only in English.";
        StartCoroutine(ollamaClient.SendPrompt(aiPromptNormal, OnAIResponse, OnAIError));
    }

    bool IsPasswordSaid(string playerText)
    {
        if (string.IsNullOrEmpty(playerText)) return false;
        // Busca la palabra exacta (con límites) sin importar mayúsculas
        return Regex.IsMatch(playerText, $@"\b{Regex.Escape(password)}\b", RegexOptions.IgnoreCase);
    }

    void OnAIResponse(string aiText)
    {
        AppendToChat("NPC (Guardia): " + aiText);
    }

    void OnAIError(string err)
    {
        AppendToChat("Error IA: " + err);
    }

    void AppendToChat(string line)
    {
        if (chatOutput == null) { Debug.LogWarning("chatOutput no asignado."); return; }
        chatOutput.text += (string.IsNullOrEmpty(chatOutput.text) ? "" : "\n") + line;
    }
}