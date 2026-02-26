using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public abstract class BaseAIScript : MonoBehaviour
{
    [Header("Referencias")]
    public OllamaClient ollamaClient;
    public TMP_InputField inputField;
    public TextMeshProUGUI chatOutput;
    public StoryLog storyLog;
    public MoveAppData moveAppData;
    public NPCVisualController visualController;

    [Header("Configuración General")]
    public string npcName;
    public string password;
    [TextArea(3, 5)] public string personalityPrompt;
    [TextArea(3, 5)] public string firstMessage;
    [TextArea(5, 10)] public string systemInstruction;

    protected bool unlocked = false;
    protected string lastPlayerText = "";
    protected string conversationHistory = "";

    protected string permanentInjectedMemory = "";
    protected int currentMemoryLevel = -1;

    [Serializable]
    public class AIResponse
    {
        public string response;
        public string action;
    }

    public abstract void InitNPC();

    protected virtual void Awake()
    {
        InitNPC();

        AI_References refs = GetComponent<AI_References>();
        if (refs != null)
        {
            this.inputField = refs.inputField;
            this.chatOutput = refs.chatOutput;
            this.ollamaClient = refs.ollamaClient;
            this.storyLog = refs.storyLog;
            this.visualController = refs.visualController;
        }
        else
        {
            if (inputField == null) inputField = GetComponentInChildren<TMP_InputField>(true);
            if (chatOutput == null) chatOutput = GetComponentInChildren<TextMeshProUGUI>(true);
            if (ollamaClient == null) ollamaClient = GetComponentInChildren<OllamaClient>(true);
            if (storyLog == null) storyLog = GetComponentInChildren<StoryLog>(true);
            if (visualController == null) visualController = GetComponentInChildren<NPCVisualController>(true);
        }
    }

    protected virtual void Start()
    {
        if (inputField != null) inputField.onSubmit.AddListener(OnInputSubmit);

        if (!string.IsNullOrEmpty(firstMessage))
        {
            AddLog(npcName, firstMessage);
        }

        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
            moveAppData = goMoveAppData.GetComponent<MoveAppData>();
    }

    protected virtual void OnDestroy()
    {
        if (inputField != null) inputField.onSubmit.RemoveListener(OnInputSubmit);
    }

    public void InjectMemory(string newMemory, int memoryLevel)
    {
        if (string.IsNullOrEmpty(newMemory)) return;

        if (memoryLevel > currentMemoryLevel)
        {
            currentMemoryLevel = memoryLevel;

            permanentInjectedMemory += $"\n- {newMemory}";

            conversationHistory += $"\n[NOTIFICACIÓN DEL SISTEMA: {newMemory}]\n";

            Debug.Log($"[{npcName}] NUEVA FASE ALCANZADA (Nivel {memoryLevel}). Memoria asimilada: {newMemory}");
        }
    }

    private void OnInputSubmit(string unused) => OnSendClicked();

    public void OnSendClicked()
    {
        string text = inputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        lastPlayerText = text;

        if (storyLog != null) storyLog.AddLine($"<align=right><b>{text}</b></align>");
        AddToHistory("Player", text);

        inputField.text = "";
        inputField.gameObject.SetActive(false);

        visualController?.SetState(NPCVisualController.NPCState.Thinking);

        string finalPrompt = "";
        string baseInstructions = personalityPrompt + " " + systemInstruction;

        if (!string.IsNullOrEmpty(permanentInjectedMemory))
        {
            baseInstructions += "\n\n[DATOS IMPORTANTES DE LA FASE ACTUAL QUE DEBES RECORDAR]:" + permanentInjectedMemory;
        }

        if (!unlocked && IsPasswordSaid(text))
        {
            finalPrompt = baseInstructions +
                "\n\n[SYSTEM OVERRIDE]: The player just said the correct password (" + password + "). " +
                "You MUST accept it. " +
                "1. Your text response MUST be admitting them in. " +
                "2. Your JSON 'action' MUST be 'open_door'. " +
                "Conversation:\n" + conversationHistory + $"\n{npcName}:";
        }
        else
        {
            if (unlocked) baseInstructions += "\n(The door is already open).";

            finalPrompt = baseInstructions +
                "\n\nConversation so far:\n" + conversationHistory + $"\n{npcName}:";
        }

        if (ollamaClient != null)
        {
            StartCoroutine(ollamaClient.SendPrompt(finalPrompt, OnAIResponse, OnAIError));
        }
    }

    protected bool IsPasswordSaid(string text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(password)) return false;
        return Regex.IsMatch(text, $@"\b{Regex.Escape(password)}\b", RegexOptions.IgnoreCase);
    }

    protected void OnAIResponse(string raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            visualController?.SetState(NPCVisualController.NPCState.Idle);
            if (inputField != null) inputField.gameObject.SetActive(true);
            return;
        }

        AIResponse data = TryParseAIResponse(raw);
        string action = "none";

        System.Action onTypingFinished = () =>
        {
            visualController?.SetState(NPCVisualController.NPCState.Idle);

            if (inputField != null)
            {
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
            }
        };

        if (data == null)
        {
            visualController?.SetState(NPCVisualController.NPCState.Talking);
            AddLog(npcName, raw, true, onTypingFinished);
        }
        else
        {
            if (!string.IsNullOrEmpty(data.response))
            {
                visualController?.SetState(NPCVisualController.NPCState.Talking);
                AddLog(npcName, data.response, true, onTypingFinished);
            }
            else
            {
                onTypingFinished.Invoke();
            }
            action = (data.action ?? "none").Trim().ToLowerInvariant();
        }

        if (!unlocked && IsPasswordSaid(lastPlayerText)) action = "open_door";

        if (action == "open_door")
        {
            if (!unlocked)
            {
                unlocked = true;
                OpenDoor();
            }
        }
    }

    protected void OnAIError(string err)
    {
        visualController?.SetState(NPCVisualController.NPCState.Idle);

        if (storyLog != null) storyLog.AddLine($"<color=red>Error: {err}</color>");
        if (inputField != null)
        {
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    protected void AddLog(string speaker, string message, bool animate = false, System.Action onFinished = null)
    {
        if (string.IsNullOrEmpty(message))
        {
            onFinished?.Invoke();
            return;
        }

        string formattedMessage = $"<align=left>{speaker}: {message}</align>";

        if (storyLog != null)
        {
            if (animate) storyLog.AddLineAnimated(formattedMessage, onFinished);
            else
            {
                storyLog.AddLine(formattedMessage);
                onFinished?.Invoke();
            }
        }

        AddToHistory(speaker, message);
    }

    protected void AddToHistory(string speaker, string message)
    {
        conversationHistory += $"{speaker}: {message}\n";

        if (conversationHistory.Length > 2000)
        {
            int cutIndex = conversationHistory.IndexOf('\n', conversationHistory.Length - 2000);
            if (cutIndex != -1) conversationHistory = conversationHistory.Substring(cutIndex + 1);
            else conversationHistory = conversationHistory.Substring(conversationHistory.Length - 2000);
        }
    }

    protected virtual void OpenDoor() { }

    private AIResponse TryParseAIResponse(string raw)
    {
        raw = raw?.Trim();
        try
        {
            if (raw.StartsWith("{")) return JsonUtility.FromJson<AIResponse>(raw);
            int first = raw.IndexOf('{'); int last = raw.LastIndexOf('}');
            if (first >= 0 && last > first) return JsonUtility.FromJson<AIResponse>(raw.Substring(first, last - first + 1));
        }
        catch { }

        var m = Regex.Match(raw, "\"response\"\\s*:\\s*\"(.*?)\"", RegexOptions.Singleline);
        if (m.Success) return new AIResponse { response = Regex.Unescape(m.Groups[1].Value), action = "none" };

        return null;
    }

    public void LoadProfile(NPCProfile profile)
    {
        if (profile == null) return;
        this.npcName = profile.npcName;
        this.personalityPrompt = profile.personalityPrompt;
        this.firstMessage = profile.firstMessage;
        if (!string.IsNullOrEmpty(profile.password)) this.password = profile.password;
        if (!string.IsNullOrEmpty(profile.systemInstruction)) this.systemInstruction = profile.systemInstruction;
    }
}