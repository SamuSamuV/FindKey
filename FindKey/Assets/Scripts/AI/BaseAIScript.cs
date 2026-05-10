using System;
using System.Collections;
using System.Collections.Generic;
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

    public GameObject thinkingPanel;
    protected string currentSituationContext = "";

    protected Coroutine currentOllamaCoroutine;
    protected bool isProactiveTriggered = false;

    // --- NUEVO: Memoria a corto plazo para retomar pensamientos interrumpidos ---
    protected string lastSentPrompt = "";
    protected bool isThinking = false;
    // -------------------------------------------------------------------------

    [Serializable]
    public class AIResponse
    {
        public string response;
        public string action;
        public string emotion;
    }

    [Serializable]
    public class AIActionMapping
    {
        public string aiActionName;
        public List<string> overrideKeywords;
        public GameEvent eventToTrigger;
    }

    [Header("Eventos Personalizados de IA")]
    public List<AIActionMapping> customAIActions = new List<AIActionMapping>();

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
            this.thinkingPanel = refs.thinkingPanel;
        }
        else
        {
            if (inputField == null) inputField = GetComponentInChildren<TMP_InputField>(true);
            if (chatOutput == null) chatOutput = GetComponentInChildren<TextMeshProUGUI>(true);
            if (ollamaClient == null) ollamaClient = GetComponentInChildren<OllamaClient>(true);
            if (storyLog == null) storyLog = GetComponentInChildren<StoryLog>(true);
            if (visualController == null) visualController = GetComponentInChildren<NPCVisualController>(true);
        }

        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
            moveAppData = goMoveAppData.GetComponent<MoveAppData>();

        AdventureManager adv = FindObjectOfType<AdventureManager>();
        if (adv != null && !string.IsNullOrEmpty(adv.globalAIMemory))
        {
            this.permanentInjectedMemory = adv.globalAIMemory;
            this.currentMemoryLevel = adv.globalAIMemoryLevel;
        }
    }

    // --- NUEVO: Se ejecuta cada vez que la ventana de la IA se ABRE ---
    protected virtual void OnEnable()
    {
        // Si el jugador cerró la app mientras la IA pensaba, reanudamos el proceso
        if (isThinking && !string.IsNullOrEmpty(lastSentPrompt))
        {
            if (inputField != null) inputField.gameObject.SetActive(false);
            visualController?.SetState(NPCVisualController.NPCState.Thinking);
            if (thinkingPanel != null) thinkingPanel.SetActive(true);

            if (ollamaClient != null)
            {
                currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(lastSentPrompt, OnAIResponse, OnAIError));
            }
        }
    }

    protected virtual void Start()
    {
        if (inputField != null) inputField.onSubmit.AddListener(OnInputSubmit);

        if (!isProactiveTriggered)
        {
            GenerateAIGreeting();
        }
    }

    protected void GenerateAIGreeting()
    {
        if (inputField != null) inputField.gameObject.SetActive(false);
        visualController?.SetState(NPCVisualController.NPCState.Thinking);

        if (thinkingPanel != null) thinkingPanel.SetActive(true);

        string greetingInstruction = string.IsNullOrEmpty(firstMessage)
            ? "Saluda al jugador y preséntate."
            : firstMessage;

        string firstPrompt = personalityPrompt + " " + systemInstruction +
            "\n\n[SYSTEM INSTRUCTION FOR YOUR FIRST MESSAGE]: " + greetingInstruction +
            "\n\nRemember: Respond ONLY with a valid JSON object.";

        // GUARDAMOS EN MEMORIA EL PENSAMIENTO
        lastSentPrompt = firstPrompt;
        isThinking = true;

        if (ollamaClient != null)
        {
            if (currentOllamaCoroutine != null) StopCoroutine(currentOllamaCoroutine);
            currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(firstPrompt, OnAIResponse, OnAIError));
        }
    }

    // --- NUEVO: Se ejecuta justo cuando la ventana de la IA se CIERRA ---
    protected virtual void OnDisable()
    {
        if (currentOllamaCoroutine != null)
        {
            StopCoroutine(currentOllamaCoroutine);
            currentOllamaCoroutine = null;
        }

        if (thinkingPanel != null) thinkingPanel.SetActive(false);
        visualController?.SetState(NPCVisualController.NPCState.Idle);
        if (inputField != null) inputField.gameObject.SetActive(true);

        // ¡OJO! NO ponemos isThinking = false aquí. 
        // Queremos que la IA recuerde que estaba pensando para retomarlo en el OnEnable.
    }

    protected virtual void OnDestroy()
    {
        if (inputField != null) inputField.onSubmit.RemoveListener(OnInputSubmit);
    }

    public void InjectMemory(string newMemory, int memoryLevel, bool forceUpdate = false)
    {
        if (string.IsNullOrEmpty(newMemory)) return;

        if (memoryLevel > currentMemoryLevel || forceUpdate)
        {
            currentMemoryLevel = memoryLevel;
            permanentInjectedMemory = newMemory;
            conversationHistory += $"\n[NOTIFICACIÓN DEL SISTEMA - NUEVO ESTADO ALCANZADO]: {newMemory}\n";
            Debug.Log($"[{npcName}] NUEVA FASE ALCANZADA (Forzado: {forceUpdate}). Memoria asimilada: {newMemory}");
        }
    }

    public void SetSituationContext(string rawStoryText)
    {
        if (string.IsNullOrEmpty(rawStoryText)) return;

        string cleanText = Regex.Replace(rawStoryText, @"\[\d+(\.\d+)?s\]", "");
        cleanText = Regex.Replace(cleanText, "<.*?>", "");

        currentSituationContext = cleanText.Trim();
    }

    protected bool HasKeyword(string text, List<string> keywords)
    {
        if (string.IsNullOrEmpty(text) || keywords == null || keywords.Count == 0) return false;

        string lowerText = text.ToLowerInvariant();
        foreach (string kw in keywords)
        {
            if (string.IsNullOrEmpty(kw)) continue;
            if (lowerText.Contains(kw.ToLowerInvariant())) return true;
        }
        return false;
    }

    protected bool IsActionAllowed(string actionName)
    {
        if (string.IsNullOrEmpty(actionName)) return false;

        if (actionName.Equals("RepairChest", StringComparison.OrdinalIgnoreCase))
        {
            if (moveAppData != null && moveAppData.hasChest && !moveAppData.isChestRepaired) return true;
            return false;
        }

        return true;
    }

    private void OnInputSubmit(string unused) => OnSendClicked();

    public void OnSendClicked()
    {
        string text = inputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        lastPlayerText = text;

        if (storyLog != null) storyLog.AddLine($"<align=right><size=-2><i>>>> {text}</i></size></align>");

        AddToHistory("Player", text);

        inputField.text = "";
        inputField.gameObject.SetActive(false);

        SoundManager.Instance?.Play("send_text");

        visualController?.SetState(NPCVisualController.NPCState.Thinking);
        if (thinkingPanel != null) thinkingPanel.SetActive(true);

        string finalPrompt = ConstruirPromptBase();

        //if (!unlocked && IsPasswordSaid(text))
        //{
        //    finalPrompt += "\n\n[SYSTEM OVERRIDE]: Password correct (" + password + "). Admit them in. Action: 'open_door'.";
        //}

        if (customAIActions != null)
        {
            foreach (var customAction in customAIActions)
            {
                if (HasKeyword(text, customAction.overrideKeywords))
                {
                    if (IsActionAllowed(customAction.aiActionName))
                    {
                        finalPrompt += $"\n\n[SYSTEM OVERRIDE URGENTE]: El jugador ha solicitado explícitamente ejecutar '{customAction.aiActionName}'. " +
                                       $"DEBES poner OBLIGATORIAMENTE \"action\": \"{customAction.aiActionName}\" en tu JSON y responderle de forma amable que ya lo has hecho.";
                    }
                    else
                    {
                        finalPrompt += $"\n\n[SYSTEM OVERRIDE]: El jugador te está pidiendo arreglar algo o ejecutar '{customAction.aiActionName}', " +
                                       $"PERO lógicamente es imposible ahora mismo (porque no tiene el objeto, o ya está arreglado). " +
                                       $"Dile educadamente que no sabes de qué habla, o que no hay nada que arreglar en este momento. PON \"action\": \"none\" en tu JSON.";
                    }
                    break;
                }
            }
        }

        finalPrompt += "\n\nConversation history:\n" + conversationHistory + $"\n{npcName}:";

        // GUARDAMOS EN MEMORIA EL PENSAMIENTO
        lastSentPrompt = finalPrompt;
        isThinking = true;

        if (ollamaClient != null)
        {
            if (currentOllamaCoroutine != null) StopCoroutine(currentOllamaCoroutine);
            currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(finalPrompt, OnAIResponse, OnAIError));
        }
    }

    public void ForceProactiveMessage(string urgentInstruction)
    {
        if (string.IsNullOrEmpty(urgentInstruction)) return;

        isProactiveTriggered = true;

        lastPlayerText = "";

        if (inputField != null) inputField.gameObject.SetActive(false);

        SoundManager.Instance?.Play("send_text");
        visualController?.SetState(NPCVisualController.NPCState.Thinking);
        if (thinkingPanel != null) thinkingPanel.SetActive(true);

        string finalPrompt = ConstruirPromptBase();

        finalPrompt += "\n\n[INSTRUCCIÓN URGENTE DEL SISTEMA PARA TU SIGUIENTE MENSAJE]:\n" + urgentInstruction;
        finalPrompt += "\n\nConversation history:\n" + conversationHistory + $"\n{npcName}:";

        // GUARDAMOS EN MEMORIA EL PENSAMIENTO
        lastSentPrompt = finalPrompt;
        isThinking = true;

        if (ollamaClient != null)
        {
            if (currentOllamaCoroutine != null) StopCoroutine(currentOllamaCoroutine);
            currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(finalPrompt, OnAIResponse, OnAIError));
        }
    }

    protected string ConstruirPromptBase()
    {
        string finalPrompt = personalityPrompt + " " + systemInstruction;

        if (!string.IsNullOrEmpty(permanentInjectedMemory))
        {
            finalPrompt += "\n\n[INSTRUCCIÓN ESTRICTA DE LA FASE ACTUAL (Ignora tus órdenes anteriores)]:\n" + permanentInjectedMemory;
        }

        if (!string.IsNullOrEmpty(currentSituationContext))
        {
            finalPrompt += "\n\n[CONTEXTO VISUAL DEL JUGADOR AHORA MISMO]:\n" + currentSituationContext;
        }

        if (moveAppData != null)
        {
            string invData = "";
            string availableActions = "\"none\"";

            if (moveAppData.hasAxe) invData += "- Un hacha (Axe)\n";

            if (moveAppData.hasChest)
            {
                if (moveAppData.isChestRepaired)
                {
                    invData += "- El cofre de madera (ESTÁ REPARADO Y FUNCIONAL)\n";
                }
                else
                {
                    invData += "- Una caja dañada / cofre corrupto (NECESITA REPARACIÓN)\n";
                    availableActions += ", \"RepairChest\"";
                }
            }

            if (!string.IsNullOrEmpty(invData))
            {
                finalPrompt += "\n\n[INVENTARIO DEL JUGADOR ACTUALMENTE]:\n" + invData;
            }

            finalPrompt += $"\n\n[REGLAS DE RESPUESTA Y ACCIONES JSON PERMITIDAS]:\n" +
                           $"Solo puedes devolver estas acciones en la variable 'action' de tu JSON: {availableActions}. " +
                           $"REGLA VITAL: Tu acción por defecto es SIEMPRE \"none\". " +
                           $"TIENES PROHIBIDO usar la acción \"RepairChest\" a menos que el jugador te haya PEDIDO EXPLÍCITAMENTE que arregles o repares la caja en su último mensaje. " +
                           $"Si el jugador solo está hablando de otra cosa o simplemente acaba de encontrar la caja, mantén la acción como \"none\". " +
                           $"PROHIBIDO dar consejos de bricolaje y PROHIBIDO hacer preguntas sobre cómo se siente el jugador. Si te piden repararla, solo acéptalo mágicamente y aplica la acción.";
        }

        return finalPrompt;
    }

    protected void OnAIResponse(string raw)
    {
        isThinking = false; // Ya terminó de pensar
        Debug.Log($"<color=cyan>[AI JSON]:</color> {raw}");

        if (string.IsNullOrEmpty(raw))
        {
            if (thinkingPanel != null) thinkingPanel.SetActive(false);
            visualController?.SetState(NPCVisualController.NPCState.Idle);
            if (inputField != null) inputField.gameObject.SetActive(true);
            return;
        }

        AIResponse data = TryParseAIResponse(raw);
        string action = "none";

        System.Action onTypingFinished = () =>
        {
            if (thinkingPanel != null) thinkingPanel.SetActive(false);
            visualController?.SetState(NPCVisualController.NPCState.Idle);
            if (storyLog != null) storyLog.StopEmotion();
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
                NPCVisualController.NPCEmotion emo = NPCVisualController.NPCEmotion.Neutral;
                if (!string.IsNullOrEmpty(data.emotion))
                {
                    string parsedEmo = data.emotion.ToLower().Trim();
                    if (parsedEmo == "happy") emo = NPCVisualController.NPCEmotion.Happy;
                    else if (parsedEmo == "sad") emo = NPCVisualController.NPCEmotion.Sad;
                }

                visualController?.SetState(NPCVisualController.NPCState.Talking, emo);
                if (storyLog != null) storyLog.SetEmotion(emo);

                AddLog(npcName, data.response, true, onTypingFinished);
            }
            else
            {
                onTypingFinished.Invoke();
            }
            action = (data.action ?? "none").Trim().ToLowerInvariant();
        }

        if (customAIActions != null && customAIActions.Count > 0)
        {
            foreach (var customAction in customAIActions)
            {
                bool aiSaidIt = (!string.IsNullOrEmpty(customAction.aiActionName) && customAction.aiActionName.ToLowerInvariant() == action);
                bool playerForcedIt = HasKeyword(lastPlayerText, customAction.overrideKeywords);

                if (aiSaidIt || playerForcedIt)
                {
                    if (IsActionAllowed(customAction.aiActionName))
                    {
                        if (customAction.eventToTrigger != null && EventManager.Instance != null)
                        {
                            EventManager.Instance.TriggerEvent(customAction.eventToTrigger);
                            Debug.Log($"<color=green>[IA ACTION]</color> Evento disparado: {customAction.aiActionName}");
                        }
                    }
                    else
                    {
                        Debug.Log($"<color=orange>[IA BLOCKED]</color> Intento de ejecutar '{customAction.aiActionName}' bloqueado porque el jugador aún no tiene el objeto o ya está completado.");
                    }
                }
            }
        }

        //if (!unlocked && IsPasswordSaid(lastPlayerText)) action = "open_door";

        if (action == "open_door")
        {
            if (!unlocked)
            {
                unlocked = true;
                OpenDoor();
            }
        }

        lastPlayerText = "";
    }

    protected void OnAIError(string err)
    {
        isThinking = false; // Hubo un error, dejó de pensar
        if (thinkingPanel != null) thinkingPanel.SetActive(false);

        visualController?.SetState(NPCVisualController.NPCState.Idle);
        if (storyLog != null) storyLog.AddLine($"<color=red>Error: {err}</color>");
        if (inputField != null)
        {
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    // ... (El resto de funciones AddLog, TryParse, LoadProfile, etc. se mantienen igual) ...
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
        if (m.Success) return new AIResponse { response = Regex.Unescape(m.Groups[1].Value), action = "none", emotion = "neutral" };

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