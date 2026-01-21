using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public abstract class BaseAIScript : MonoBehaviour
{
    [Header("Referencias (Auto-asignadas si están vacías)")]
    public OllamaClient ollamaClient;
    public TMP_InputField inputField;
    public TextMeshProUGUI chatOutput;
    public StoryLog storyLog;

    [Header("Configuración General")]
    public string npcName;
    public string password;
    [TextArea(3, 5)] public string personalityPrompt;
    [TextArea(3, 5)] public string firstMessage;
    [TextArea(5, 10)] public string systemInstruction;

    // ESTADO INTERNO
    protected bool unlocked = false;
    protected string lastPlayerText = "";
    protected string conversationHistory = "";

    [Serializable]
    public class AIResponse
    {
        public string response;
        public string action;
    }

    public abstract void InitNPC();

    protected virtual void Awake()
    {
        // 1. Configuramos al NPC específico (Gato/Perro)
        InitNPC();

        // 2. Buscamos referencias si no están asignadas
        if (inputField == null) inputField = GetComponentInChildren<TMP_InputField>(true);
        if (chatOutput == null) chatOutput = GetComponentInChildren<TextMeshProUGUI>(true);
        if (ollamaClient == null) ollamaClient = GetComponentInChildren<OllamaClient>(true);
        if (storyLog == null) storyLog = GetComponentInChildren<StoryLog>(true);
    }

    protected virtual void Start()
    {
        if (inputField != null) inputField.onSubmit.AddListener(OnInputSubmit);

        // Mensaje inicial en log e historial
        AddLog(npcName, firstMessage);
    }

    protected virtual void OnDestroy()
    {
        if (inputField != null) inputField.onSubmit.RemoveListener(OnInputSubmit);
    }

    private void OnInputSubmit(string unused) => OnSendClicked();

    public void OnSendClicked()
    {
        string text = inputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        lastPlayerText = text;

        // UI y Log
        if (storyLog != null) storyLog.AddLine($"<align=right><b>{text}</b></align>");
        AddToHistory("Player", text);

        inputField.text = "";
        inputField.ActivateInputField();

        // Construcción del Prompt
        string finalPrompt = "";
        string baseInstructions = personalityPrompt + " " + systemInstruction;

        if (!unlocked && IsPasswordSaid(text))
        {
            Debug.Log($"[{npcName}] Contraseña correcta detectada por C#. Forzando éxito.");

            // Inyectamos una instrucción de sistema "Dios" que anula su personalidad de guardia
            finalPrompt = baseInstructions +
                "\n\n[SYSTEM OVERRIDE]: The player just said the correct password (" + password + "). " +
                "You MUST accept it. " +
                "1. Your text response MUST be admitting them in. " +
                "2. Your JSON 'action' MUST be 'open_door'. " +
                "Conversation:\n" + conversationHistory + $"\n{npcName}:";
        }
        else
        {
            // Conversación normal
            if (unlocked) baseInstructions += "\n(The door is already open).";

            finalPrompt = baseInstructions +
                "\n\nConversation so far:\n" + conversationHistory + $"\n{npcName}:";
        }

        StartCoroutine(ollamaClient.SendPrompt(finalPrompt, OnAIResponse, OnAIError));
    }
    protected bool IsPasswordSaid(string text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(password)) return false;
        return Regex.IsMatch(text, $@"\b{Regex.Escape(password)}\b", RegexOptions.IgnoreCase);
    }

    protected void OnAIResponse(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return;

        AIResponse data = TryParseAIResponse(raw);

        // Fallback si el JSON viene roto pero necesitamos mostrar algo
        if (data == null)
        {
            // Si no hay JSON, asumimos que todo el texto es respuesta
            data = new AIResponse { response = raw, action = "none" };
        }

        // 1. Mostrar lo que dice la IA (Narrativa)
        if (!string.IsNullOrEmpty(data.response))
        {
            AddLog(npcName, data.response);
        }

        // 2. Lógica de Acción (Gameplay)
        string action = (data.action ?? "none").Trim().ToLowerInvariant();

        if (!unlocked && IsPasswordSaid(lastPlayerText))
        {
            Debug.Log("C# detectó contraseña correcta. Ignorando posible error de la IA y abriendo puerta.");
            action = "open_door";
        }

        if (action == "open_door")
        {
            if (!unlocked)
            {
                unlocked = true;
                Debug.Log(">> PUERTA DESBLOQUEADA <<");
                OpenDoor();
            }
        }
    }
    protected void OnAIError(string err)
    {
        if (storyLog != null) storyLog.AddLine($"<color=red>Error: {err}</color>");
    }

    // Helpers
    protected void AddLog(string speaker, string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        if (storyLog != null) storyLog.AddLine($"<align=left>{speaker}: {message}</align>");
        AddToHistory(speaker, message);
    }

    protected void AddToHistory(string speaker, string message)
    {
        conversationHistory += $"{speaker}: {message}\n";
        // Limitar historial
        if (conversationHistory.Length > 2500)
            conversationHistory = conversationHistory.Substring(conversationHistory.Length - 2500);
    }

    protected virtual void OpenDoor()
    {
        // Sobreescribe esto en el hijo si quieres efectos distintos para cada animal
        Debug.Log("La puerta se abre genéricamente.");
    }

    private AIResponse TryParseAIResponse(string raw)
    {
        // Lógica de limpieza de JSON idéntica a la que tenías
        raw = raw?.Trim();
        try
        {
            if (raw.StartsWith("{")) return JsonUtility.FromJson<AIResponse>(raw);
            int first = raw.IndexOf('{'); int last = raw.LastIndexOf('}');
            if (first >= 0 && last > first) return JsonUtility.FromJson<AIResponse>(raw.Substring(first, last - first + 1));
        }
        catch { }

        // Regex fallback
        var m = Regex.Match(raw, "\"response\"\\s*:\\s*\"(.*?)\"", RegexOptions.Singleline);
        if (m.Success) return new AIResponse { response = Regex.Unescape(m.Groups[1].Value), action = "none" };

        return null;
    }

    public void LoadProfile(NPCProfile profile)
    {
        if (profile == null) return;

        // Sobreescribimos las variables del script con las de la ficha
        this.npcName = profile.npcName;
        this.password = profile.password;
        this.personalityPrompt = profile.personalityPrompt;
        this.firstMessage = profile.firstMessage;

        // Solo sobreescribimos la instrucción si la ficha tiene algo, si no, dejamos la del código
        if (!string.IsNullOrEmpty(profile.systemInstruction))
        {
            this.systemInstruction = profile.systemInstruction;
        }

        if (storyLog != null && !string.IsNullOrEmpty(this.firstMessage))
        {

        }
    }
}