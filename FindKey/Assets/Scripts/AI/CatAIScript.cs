using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CatAIScript : BaseAIScript
{
    [Header("References")]
    public OllamaClient ollamaClient;
    public TMP_InputField inputField;
    public TextMeshProUGUI chatOutput;
    public StoryLog storyLog;

    [Header("Settings")]
    [Tooltip("Contraseña exacta (case insensitive)")]
    public string password = "Patata";

    [TextArea(4, 8)]
    [Tooltip("Instrucción para el modelo. IMPORTANTE: pedimos respuesta en JSON.")]
    public string systemInstruction =
        "You are an NPC guard. Always respond ONLY with a single valid JSON object. " +
        "The JSON must contain: { \"response\": string, \"action\": string } " +
        "Allowed actions: \"none\", \"open_door\". No extra text outside JSON. " +
        "If the player said the correct password the action should be \"open_door\". " +
        "Otherwise action should be \"none\". Respond only in English.";

    [TextArea(4, 8)]
    public string personality;

    [TextArea(3, 6)]
    public string firstMesage;

    // Estado
    private bool unlocked = false;
    private string lastPlayerText = "";
    private string conversationHistory = "";
    [SerializeField] private string npcName;

    [Serializable]
    public class AIResponse
    {
        public string response;
        public string action;
    }

    void Awake()
    {
        InitNPC();

        if (inputField == null) inputField = GetComponentInChildren<TMP_InputField>(true);
        if (chatOutput == null) chatOutput = GetComponentInChildren<TextMeshProUGUI>(true);
        if (ollamaClient == null) ollamaClient = FindObjectOfType<OllamaClient>();
        if (storyLog == null) storyLog = FindObjectOfType<StoryLog>();
    }

    void Start()
    {
        if (inputField != null) inputField.onSubmit.AddListener(OnInputSubmit);
        storyLog.AddLine($"<align=left>{npcName}: {firstMesage}</align>");

        // Nueva línea: añadimos el primer mensaje al historial para que la IA lo "recuerde"
        conversationHistory += $"{npcName}: {firstMesage}\n"; // /////////////////////////////////////////
    }

    void OnDestroy()
    {
        if (inputField != null) inputField.onSubmit.RemoveListener(OnInputSubmit);
    }

    public override void InitNPC()
    {

    }

    void OnInputSubmit(string unused) => OnSendClicked();

    public void OnSendClicked()
    {
        string text = inputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        lastPlayerText = text;              // guardamos para validar acciones

        // Mostramos el mensaje del jugador en el StoryLog (alineado a la derecha)
        storyLog.AddLine($"<align=right><b>{text}</b></align>");

        // Nueva línea: añadimos el mensaje del jugador al historial de conversación
        conversationHistory += $"Player: {text}\n"; // /////////////////////////////////////////

        inputField.text = "";
        inputField.ActivateInputField();

        // 1) Comprobamos la contraseña localmente
        if (!unlocked && IsPasswordSaid(text))
        {
            // --- CAMBIO AQUÍ ---
            // NO desbloqueamos todavía (unlocked = true). 
            // Esperamos a que la IA nos de la "autorización" con la acción open_door.
            // -------------------

            Debug.Log("[GameLogic] Contraseña correcta detectada. Solicitando confirmación a la IA...");

            // Le decimos a la IA explícitamente que la contraseña es correcta para forzar el JSON 'open_door'
            string aiPromptAfterPassword = 
                personality+ 
                systemInstruction +
                "\n\nContext: The player has just said the CORRECT password. You must perform the action 'open_door'.\n" +
                "Conversation so far:\n" +
                conversationHistory +
                $"\n{npcName}:";

            StartCoroutine(ollamaClient.SendPrompt(aiPromptAfterPassword, OnAIResponse, OnAIError));
            return;
        }

        // 2) Si ya desbloqueado, seguir charlando (sí enviamos prompt al modelo)
        if (unlocked)
        {
            // Línea original (comentada para referencia):
            // string aiPromptUnlocked = $"{systemInstruction}\n\nThe player already has access.\nPlayer: {text}\nGuard:";
            // StartCoroutine(ollamaClient.SendPrompt(aiPromptUnlocked, OnAIResponse, OnAIError));

            // Nueva línea: usamos el historial para que la IA recuerde el contexto
            string aiPromptUnlocked =
                personality +
                systemInstruction +
                "\n\nConversation so far:\n" +
                conversationHistory +
                $"\n{npcName}:"; // /////////////////////////////////////////

            StartCoroutine(ollamaClient.SendPrompt(aiPromptUnlocked, OnAIResponse, OnAIError));
            return;
        }

        // 3) Si no ha dicho contraseña, enviamos mensaje normal roleado al guardia malvado (pidiendo JSON)
        // Línea original (comentada para referencia):
        // string aiPromptNormal = $"{systemInstruction}\n\nPlayer: {text}\nGuard:";
        // StartCoroutine(ollamaClient.SendPrompt(aiPromptNormal, OnAIResponse, OnAIError));

        // Nueva línea: construimos prompt con historial
        string aiPromptNormal =
            personality +
            systemInstruction +
            "\n\nConversation so far:\n" +
            conversationHistory +
            $"\n{npcName}:"; // /////////////////////////////////////////

        StartCoroutine(ollamaClient.SendPrompt(aiPromptNormal, OnAIResponse, OnAIError));
    }

    bool IsPasswordSaid(string playerText)
    {
        if (string.IsNullOrEmpty(playerText)) return false;
        return Regex.IsMatch(playerText, $@"\b{Regex.Escape(password)}\b", RegexOptions.IgnoreCase);
    }

    void OnAIResponse(string raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            Debug.Log("Error: IA devolvió vacío.");
            return;
        }

        // Intentar parsear JSON directo
        AIResponse data = TryParseAIResponse(raw);
        if (data == null)
        {
            // Fallback: tratar todo como texto
            storyLog.AddLine($"<align=left>{npcName}: {raw.Trim()}</align>");

            // Nueva línea: añadimos la respuesta "raw" al historial para que la IA lo recuerde en la siguiente llamada
            conversationHistory += $"{npcName}: {raw.Trim()}\n"; // /////////////////////////////////////////

            // Limitar tamaño del historial para evitar crecer infinito (opcional)
            if (conversationHistory.Length > 2000) // /////////////////////////////////////////
            {
                conversationHistory = conversationHistory.Substring(conversationHistory.Length - 2000); // /////////////////////////////////////////
            }

            return;
        }

        // Mostrar el texto amigable
        if (!string.IsNullOrEmpty(data.response))
            storyLog.AddLine($"<align=left>{data.response}</align>");

        // Nueva línea: añadimos la respuesta parseada al historial
        conversationHistory += $"{npcName}: {data.response}\n"; // /////////////////////////////////////////

        // Limitar tamaño del historial para evitar crecer infinito (opcional)
        if (conversationHistory.Length > 2000) // /////////////////////////////////////////
        {
            conversationHistory = conversationHistory.Substring(conversationHistory.Length - 2000); // /////////////////////////////////////////
        }

        // Ejecutar acción *solo si* Unity valida (no confiar en la IA)
        string action = (data.action ?? "none").Trim().ToLowerInvariant();
        switch (action)
        {
            case "open_door":
                // Verificamos si la contraseña fue dicha en el último mensaje O si ya estaba desbloqueado
                if (IsPasswordSaid(lastPlayerText) || unlocked)
                {
                    if (!unlocked)
                    {
                        unlocked = true; // <--- AQUÍ es donde realmente se desbloquea ahora
                        Debug.Log("[GameLogic] Estado actualizado a: UNLOCKED");
                    }
                    OpenDoor();
                }
                else
                {
                    Debug.LogWarning("IA intentó abrir puerta, pero la contraseña no era correcta en el último mensaje.");
                }
                break;

            case "none":
            default:
                // nada que hacer
                break;
        }
    }

    void OnAIError(string err)
    {
        storyLog.AddLine($"<align=left>Error IA: {err}</align>");
        // (Opcional) podríamos añadir el error al historial, pero normalmente no interesa:
        // conversationHistory += $"SystemError: {err}\n"; // comentada a modo de ejemplo
    }

    void AppendToChat(string line)
    {
        if (chatOutput == null) { Debug.LogWarning("chatOutput not assigned."); return; }
        chatOutput.text += (string.IsNullOrEmpty(chatOutput.text) ? "" : "\n") + line;
    }

    // Intenta devolver un AIResponse válido. Maneja respuestas con texto antes del JSON.
    AIResponse TryParseAIResponse(string raw)
    {
        raw = raw?.Trim();
        // Si ya parece JSON, intentar parsear directamente
        if (raw.StartsWith("{"))
        {
            try
            {
                var r = JsonUtility.FromJson<AIResponse>(raw);
                if (r != null && (r.response != null || r.action != null)) return r;
            }
            catch (Exception) { /* seguir a fallback */ }
        }

        // Buscar primer '{' y último '}'
        int first = raw.IndexOf('{');
        int last = raw.LastIndexOf('}');
        if (first >= 0 && last > first)
        {
            string candidate = raw.Substring(first, last - first + 1);
            try
            {
                var r2 = JsonUtility.FromJson<AIResponse>(candidate);
                if (r2 != null && (r2.response != null || r2.action != null)) return r2;
            }
            catch (Exception) { /* seguir a fallback */ }
        }

        // Fallback: intentar extraer "response" con regex
        var m = Regex.Match(raw, "\"response\"\\s*:\\s*\"(.*?)\"", RegexOptions.Singleline);
        if (m.Success)
        {
            string clean = Regex.Unescape(m.Groups[1].Value).Trim();
            return new AIResponse { response = clean, action = "none" };
        }

        return null; // no pudimos parsear nada útil
    }

    void OpenDoor()
    {

    }
}