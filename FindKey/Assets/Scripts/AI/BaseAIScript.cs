using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/// <summary>
/// Class: BaseAIScript
/// Description: This is an abstract base class for AI-controlled NPCs in the FindKey game. It manages the core functionalities of NPC interactions, including handling player input,
///              generating AI responses using an OllamaClient, maintaining conversation history, and triggering in-game events based on AI actions. The class also supports proactive AI messages,
///              dynamic memory injection for evolving NPC behavior, and customizable AI action mappings that can be defined in derived classes. It serves as a foundation for creating various NPCs
///              with unique personalities and behaviors while ensuring consistent interaction mechanics across the game.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
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
    public bool isProactiveTriggered = false;

    protected string lastSentPrompt = "";
    protected bool isThinking = false;

    /// <summary>
    /// Class: AIResponse
    /// Description: This class represents the structured response expected from the AI. It contains three main fields: 'response' for the textual reply that the NPC will say to
    ///              the player, 'action' for any specific in-game action that the AI wants to trigger based on the player's input or the current situation, and 'emotion' to indicate the emotional
    ///              tone of the response (e.g., happy, neutral, sad). The AI is instructed to respond strictly in this JSON format, allowing for consistent parsing and handling of AI outputs within
    ///              the game logic.
    /// </summary>
    [Serializable]
    public class AIResponse
    {
        public string response;
        public string action;
        public string emotion;
    }

    /// <summary>
    /// Class: AIActionMapping
    /// Description: This class defines a mapping between specific AI actions, keywords that can trigger those actions, and the corresponding in-game events to be triggered.
    ///              Each instance of AIActionMapping contains an 'aiActionName' which is the identifier for the action that the AI can request in its JSON response, a list of 'overrideKeywords'
    ///              that represent player input keywords which can force the triggering of the action regardless of whether the AI requested it or not, and a 'GameEvent' reference that specifies
    ///              which event should be fired when the action is triggered. This structure allows for flexible and dynamic interactions where both AI decisions and player inputs can lead to
    ///              specific game events, enhancing the interactivity and responsiveness of NPCs in the game.
    /// </summary>
    [Serializable]
    public class AIActionMapping
    {
        public string aiActionName;
        public List<string> overrideKeywords;
        public GameEvent eventToTrigger;
    }

    [Header("Eventos Personalizados de IA")]
    public List<AIActionMapping> customAIActions = new List<AIActionMapping>();

    public abstract void InitNPC(); // Each NPC must implement this to set up its unique personality, first message, system instructions, and password if needed. This ensures that all NPCs have a consistent initialization point while allowing for maximum customization.

    protected virtual void Awake() // In the base class, we use Awake to ensure that references are set before any Start or OnEnable logic runs, which might depend on them. Derived classes can override this if they need to do additional setup, but they should call base.Awake() to maintain the reference initialization.
    {
        InitNPC();

        AI_References refs = GetComponent<AI_References>();
        if (refs != null) // If an AI_References component is attached, use it to set all references. This allows for flexible assignment of references in the Unity Editor and keeps the code clean.
        {
            this.inputField = refs.inputField;
            this.chatOutput = refs.chatOutput;
            this.ollamaClient = refs.ollamaClient;
            this.storyLog = refs.storyLog;
            this.visualController = refs.visualController;
            this.thinkingPanel = refs.thinkingPanel;
        }

        else // If no AI_References component is found, attempt to find references in children. This fallback ensures that the script can still function even if the AI_References component is not set up, but it encourages the use of AI_References for better organization.
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
        if (adv != null && !string.IsNullOrEmpty(adv.globalAIMemory)) // If the AdventureManager has global AI memory, inject it into the NPC's memory. This allows for a shared memory system where the NPC can evolve its behavior based on overarching game events and states, creating a more dynamic and responsive AI experience.
        {
            this.permanentInjectedMemory = adv.globalAIMemory;
            this.currentMemoryLevel = adv.globalAIMemoryLevel;
        }
    }

    protected virtual void OnEnable() // When the NPC is enabled, if it was in a thinking state and we have a last sent prompt, we should resume the thinking process. This ensures that if the NPC gets disabled (e.g., due to a scene change or being hidden) while waiting for an AI response, it can properly resume its state when re-enabled.
    {
        if (isThinking && !string.IsNullOrEmpty(lastSentPrompt))
        {
            if (inputField != null) inputField.gameObject.SetActive(false); // Hide input while thinking

            visualController?.SetState(NPCVisualController.NPCState.Thinking);

            if (thinkingPanel != null) thinkingPanel.SetActive(true); // Show thinking panel

            if (ollamaClient != null) // Resume the OllamaClient coroutine with the last sent prompt to continue waiting for the AI response. This allows for a seamless experience where the NPC can maintain its state across enable/disable cycles without losing the context of the ongoing interaction.
            {
                currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(lastSentPrompt, OnAIResponse, OnAIError));
            }

            else // If we don't have an OllamaClient reference, we can't really do much to resume, so we log an error. This is a critical issue because without the OllamaClient, the NPC cannot function properly in its AI interactions.
            {
                OnAIError("OllamaClient no encontrado al reanudar.");
            }
        }
    }

    protected virtual void Start() // In Start, we set up the input field listener and generate the initial AI greeting if we haven't already triggered a proactive message. This ensures that the NPC starts the interaction with its first message and is ready to receive player input.
    {
        if (inputField != null) inputField.onSubmit.AddListener(OnInputSubmit);

        if (!isProactiveTriggered)
        {
            GenerateAIGreeting(); // Generate the initial greeting from the AI when the NPC is first interacted with. This sets the tone for the interaction and allows the NPC to introduce itself to the player right away.
        }
    }

    /// <summary>
    /// Generates the AI's initial greeting when the player first interacts with the NPC.
    /// Constructs the prompt by injecting the personality and system instructions.
    /// </summary>
    protected void GenerateAIGreeting() // This method generates the initial greeting from the AI when the NPC is first interacted with. It constructs the prompt using the personality and system instructions, and sends it to the OllamaClient to get the AI's response. This allows the NPC to introduce itself and set the tone for the interaction right from the start.
    {
        if (inputField != null) inputField.gameObject.SetActive(false);
        visualController?.SetState(NPCVisualController.NPCState.Thinking);

        if (thinkingPanel != null) thinkingPanel.SetActive(true);

        string greetingInstruction = string.IsNullOrEmpty(firstMessage)
            ? "Saluda al jugador y preséntate."
            : firstMessage; // If a specific first message is provided, use it as the instruction for the AI's greeting. Otherwise, use a default instruction to simply greet the player and introduce itself. This allows for flexibility in how the NPC starts the conversation, while still ensuring that it has a clear directive for its initial response.

        string firstPrompt = personalityPrompt + " " + systemInstruction +
            "\n\n[SYSTEM INSTRUCTION FOR YOUR FIRST MESSAGE]: " + greetingInstruction +
            "\n\nRemember: Respond ONLY with a valid JSON object."; // We emphasize the JSON format right away to set clear expectations for the AI's responses from the very beginning of the interaction.

        lastSentPrompt = firstPrompt;
        isThinking = true;

        if (ollamaClient != null) // Send the initial prompt to the OllamaClient to get the AI's greeting response. This kickstarts the interaction with the NPC and allows it to establish its personality and relationship with the player right away.
        {
            if (currentOllamaCoroutine != null) StopCoroutine(currentOllamaCoroutine);
            currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(firstPrompt, OnAIResponse, OnAIError));
        }

        else
        {
            OnAIError("Falta asignar OllamaClient en AI_References.");
        }
    }

    protected virtual void OnDisable() // When the NPC is disabled, we should stop any ongoing OllamaClient coroutine to prevent it from trying to update the NPC while it's not active. We also reset the visual state and input field to ensure that when the NPC is re-enabled, it starts in a clean state. This prevents potential issues with lingering states or interactions that could occur if the NPC gets disabled while waiting for an AI response.
    {
        if (currentOllamaCoroutine != null)
        {
            StopCoroutine(currentOllamaCoroutine); // Stop any ongoing OllamaClient coroutine to prevent it from trying to update the NPC while it's disabled. This is crucial for avoiding errors or unintended behavior if the AI response arrives while the NPC is not active.
            currentOllamaCoroutine = null;
        }

        if (storyLog == null || !storyLog.IsTyping) // If the story log is currently animating text, we should let it finish before hiding the thinking panel and showing the input field again. This ensures that the NPC doesn't abruptly cut off its response if it gets disabled while still animating text, providing a smoother experience for the player.
        {
            if (thinkingPanel != null) thinkingPanel.SetActive(false);
            visualController?.SetState(NPCVisualController.NPCState.Idle);

            if (inputField != null) inputField.gameObject.SetActive(true);
        }
    }
    protected virtual void OnDestroy() // When the NPC is destroyed, we should clean up any listeners to prevent potential memory leaks or null reference errors. This is a good practice to ensure that when the NPC is removed from the scene, it doesn't leave behind any lingering references that could cause issues later on.
    {
        if (inputField != null) inputField.onSubmit.RemoveListener(OnInputSubmit);
    }

    /// <summary>
    /// Dynamically injects new information into the NPC's permanent memory.
    /// Uses a memory level system to avoid overwriting critical information with less important data.
    /// </summary>
    /// <param name="newMemory">The new information or context the AI needs to assimilate.</param>
    /// <param name="memoryLevel">Priority or importance level of the memory.</param>
    /// <param name="forceUpdate">Forces the update ignoring the current priority level.</param>
    public void InjectMemory(string newMemory, int memoryLevel, bool forceUpdate = false) // This method allows for dynamic injection of new memory into the NPC's permanent memory. The 'memoryLevel' parameter is used to determine the importance or priority of the new memory, ensuring that only more significant updates can overwrite existing memory unless 'forceUpdate' is set to true. This mechanism allows the NPC's behavior and responses to evolve over time based on new information or changes in the game state, creating a more dynamic and responsive AI experience.
    {
        if (string.IsNullOrEmpty(newMemory)) return;

        if (memoryLevel > currentMemoryLevel || forceUpdate) // Only update the permanent memory if the new memory has a higher level than the current one, or if we're forcing an update. This ensures that we don't accidentally overwrite important existing memory with less significant information, while still allowing for critical updates to take precedence when necessary.
        {
            currentMemoryLevel = memoryLevel;
            permanentInjectedMemory = newMemory;
            conversationHistory += $"\n[NOTIFICACIÓN DEL SISTEMA - NUEVO ESTADO ALCANZADO]: {newMemory}\n";
            Debug.Log($"[{npcName}] NUEVA FASE ALCANZADA (Forzado: {forceUpdate}). Memoria asimilada: {newMemory}");
        }
    }

    public void SetSituationContext(string rawStoryText) // This method processes the raw story text to extract a clean context for the current situation. It removes any time annotations (e.g., [3s], [1.5s]) and any text enclosed in angle brackets (e.g., <b>, <color=red>) to ensure that the context provided to the AI is clear and focused on the relevant information. This allows the NPC to have an accurate understanding of the player's current situation based on the story log, which can inform its responses and actions.
    {
        if (string.IsNullOrEmpty(rawStoryText)) return;

        string cleanText = Regex.Replace(rawStoryText, @"\[\d+(\.\d+)?s\]", "");
        cleanText = Regex.Replace(cleanText, "<.*?>", "");

        currentSituationContext = cleanText.Trim(); // Trim any leading or trailing whitespace from the cleaned text to ensure that the context is neat and doesn't contain unnecessary spaces that could affect the AI's understanding. This final clean context can then be used in the AI prompt to provide relevant information about the player's current situation.
    }

    protected bool HasKeyword(string text, List<string> keywords) // This method checks if any of the specified keywords are present in the given text, ignoring case. It returns true if at least one keyword is found, and false otherwise. This allows for flexible matching of player input to trigger specific actions or responses from the AI based on the presence of certain keywords, enhancing the interactivity of the NPC.
    {
        if (string.IsNullOrEmpty(text) || keywords == null || keywords.Count == 0) return false;

        string lowerText = text.ToLowerInvariant();

        foreach (string kw in keywords) // Check if any of the keywords are present in the text, ignoring case. This allows for flexible matching of player input to trigger specific actions or responses from the AI based on the presence of certain keywords, enhancing the interactivity of the NPC.
        {
            if (string.IsNullOrEmpty(kw)) continue;
            if (lowerText.Contains(kw.ToLowerInvariant())) return true;
        }

        return false;
    }

    protected bool IsActionAllowed(string actionName) // This method checks if a specific action is currently allowed based on the game state and the player's inventory. For example, if the action is "RepairChest", it checks if the player has the chest and if it is not already repaired. This ensures that the AI can only trigger actions that are logically possible given the current state of the game, preventing it from attempting to perform actions that don't make sense (e.g., repairing a chest that isn't damaged or doesn't exist).
    {
        if (string.IsNullOrEmpty(actionName)) return false;

        if (actionName.Equals("RepairChest", StringComparison.OrdinalIgnoreCase)) // If the action is "RepairChest", we need to check if the player has the chest and if it is not already repaired. This ensures that the AI can only trigger the "RepairChest" action when it is logically appropriate, preventing it from trying to repair a chest that doesn't exist or is already in good condition.
        {
            if (moveAppData != null && moveAppData.hasChest && !moveAppData.isChestRepaired) return true;
            return false;
        }

        return true;
    }

    private void OnInputSubmit(string unused) => OnSendClicked(); // When the player submits input in the input field, we call the OnSendClicked method to process the input and send it to the AI. This allows for a seamless interaction where the player can type their message and have it processed immediately when they hit enter or submit the input field.

    /// <summary>
    /// Executed when the send button is clicked. Processes the player's input, updates the conversation history, 
    /// and structures the final prompt verifying event overrides (keywords) before sending it to Ollama.
    /// </summary>
    public void OnSendClicked() // This method is called when the player clicks the send button or submits their input. It processes the player's input, updates the conversation history, and sends a new prompt to the AI based on the player's message and the current context. It also handles the UI changes for thinking and resets the input field for the next message. This is a critical part of the interaction loop, allowing the player to communicate with the NPC and receive responses based on their input.
    {
        string text = inputField.text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        lastPlayerText = text;

        if (storyLog != null) storyLog.AddLine($"<align=right><size=-2><i>>>> {text}</i></size></align>");

        AddToHistory("Player", text); // Add the player's message to the conversation history so that it can be included in the context for the AI's response.

        inputField.text = "";
        inputField.gameObject.SetActive(false);

        SoundManager.Instance?.Play("send_text");

        visualController?.SetState(NPCVisualController.NPCState.Thinking);
        if (thinkingPanel != null) thinkingPanel.SetActive(true);

        string finalPrompt = ConstruirPromptBase();

        if (customAIActions != null)
        {
            foreach (var customAction in customAIActions) // Check if any custom AI action should be triggered based on the player's input keywords or the AI's requested action.
            {
                if (HasKeyword(text, customAction.overrideKeywords)) // If the player's input contains any of the override keywords for this custom action, we should force the action to be triggered regardless of whether the AI requested it or not.
                {
                    if (IsActionAllowed(customAction.aiActionName)) // If the action is allowed based on the current game state, we should inform the AI that it must perform this action in its next response. This ensures that if the player explicitly asks for something to be done (e.g., "Please repair the chest"), the AI will understand that it needs to comply with that request, even if it wasn't planning to do so based on its own logic.
                    {
                        finalPrompt += $"\n\n[SYSTEM OVERRIDE]: The player has explicitly requested the action '{customAction.aiActionName}'. " +
                                       $"You MUST set \"action\": \"{customAction.aiActionName}\" in your JSON response. " +
                                       $"Reply politely IN SPANISH that you have completed the request magically.";
                    }

                    else // If the player is asking for an action that is not currently allowed (e.g., they ask to repair the chest but they don't have it or it's already repaired), we should inform the AI that it cannot perform this action and that it should respond accordingly. This prevents the AI from trying to perform actions that don't make sense given the current state of the game, while still acknowledging the player's request in a polite manner.
                    {
                        finalPrompt += $"\n\n[SYSTEM OVERRIDE]: The player is asking you to do something related to '{customAction.aiActionName}', " +
                                       $"BUT it is logically impossible right now (they don't have the item, or it is already done). " +
                                       $"Politely tell them IN SPANISH that you don't know what they mean or there is nothing to fix. Keep \"action\": \"none\" in your JSON.";
                    }
                    break;
                }
            }
        }

        finalPrompt += "\n\nConversation history:\n" + conversationHistory + $"\n{npcName}:";

        lastSentPrompt = finalPrompt;
        isThinking = true;

        if (ollamaClient != null) // Send the constructed prompt to the OllamaClient to get the AI's response based on the player's input and the current context.
        {
            if (currentOllamaCoroutine != null) StopCoroutine(currentOllamaCoroutine);
            currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(finalPrompt, OnAIResponse, OnAIError));
        }

        else // If we don't have an OllamaClient reference, we can't send the prompt, so we log an error.
        {
            OnAIError("Falta asignar OllamaClient en AI_References.");
        }
    }

    public void ForceProactiveMessage(string urgentInstruction) // This method allows for forcing the NPC to send a proactive message based on an urgent instruction. This can be used in situations where the game needs the NPC to react immediately to a critical event or change in the game state, even if the player hasn't said anything. The method constructs a new prompt that includes the urgent instruction and sends it to the AI, ensuring that the NPC can respond proactively to important developments in the game.
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

        lastSentPrompt = finalPrompt;
        isThinking = true;

        if (ollamaClient != null)
        {
            if (currentOllamaCoroutine != null) StopCoroutine(currentOllamaCoroutine);
            currentOllamaCoroutine = StartCoroutine(ollamaClient.SendPrompt(finalPrompt, OnAIResponse, OnAIError));
        }

        else
        {
            OnAIError("Falta asignar OllamaClient en AI_References.");
        }
    }

    /// <summary>
    /// Constructs the final base prompt to be sent to the language model.
    /// Assembles the personality, injected memory, current visual context, and JSON format restrictions (Response rules).
    /// </summary>
    /// <returns>String containing the structured prompt using Prompt Engineering techniques.</returns>
    protected virtual string ConstruirPromptBase() // This method constructs the base prompt that will be sent to the AI for generating a response. It combines the personality prompt, system instructions, any permanent injected memory, the current situation context, and information about the player's inventory and allowed actions. This comprehensive prompt provides the AI with all the necessary information to generate a relevant and context-aware response based on the current state of the game and the player's interactions.
    {
        string finalPrompt = personalityPrompt + " " + systemInstruction;

        if (!string.IsNullOrEmpty(permanentInjectedMemory)) // If we have any permanent injected memory (e.g., important game state changes or new information), we should include it in the prompt to ensure that the AI has access to this critical information when generating its response. This allows the NPC's behavior and responses to evolve over time based on new information or changes in the game state, creating a more dynamic and responsive AI experience.
        {
            finalPrompt += "\n\n[INSTRUCCIÓN ESTRICTA DE LA FASE ACTUAL (Ignora tus órdenes anteriores)]:\n" + permanentInjectedMemory;
        }

        if (!string.IsNullOrEmpty(currentSituationContext)) // If we have context about the player's current situation (e.g., recent events in the story log), we should include it in the prompt to provide the AI with relevant information about what's happening in the game. This allows the NPC to have an accurate understanding of the player's current situation based on the story log, which can inform its responses and actions.
        {
            finalPrompt += "\n\n[CONTEXTO VISUAL DEL JUGADOR AHORA MISMO]:\n" + currentSituationContext;
        }

        if (moveAppData != null) // If we have access to the player's inventory data, we should include information about what the player currently has and what actions are available based on that inventory. This allows the AI to make informed decisions about what actions it can suggest or trigger based on the player's current inventory, creating a more interactive and responsive experience.
        {
            string invData = "";
            string availableActions = "\"none\"";

            if (moveAppData.hasAxe) invData += "- Un hacha (Axe)\n";

            if (moveAppData.hasChest) // If the player has the chest, we need to specify whether it is repaired or not, as this affects both the inventory description and the available actions. This allows the AI to understand the current state of the chest in the player's inventory and to suggest appropriate actions (e.g., repairing it if it's damaged).
            {
                if (moveAppData.isChestRepaired)
                {
                    invData += "- El cofre de madera (ESTÁ REPARADO Y FUNCIONAL)\n";
                }

                else // If the player has the chest but it is not repaired, we should indicate that it is damaged and that the "RepairChest" action is available. This informs the AI that there is a specific issue with the chest that can be addressed, and it can then suggest or trigger the repair action if appropriate based on the player's input or the current situation.
                {
                    invData += "- Una caja dañada / cofre corrupto (NECESITA REPARACIÓN)\n";
                    availableActions += ", \"RepairChest\"";
                }
            }

            if (!string.IsNullOrEmpty(invData)) // If we have any inventory data to provide, we should include it in the prompt to inform the AI about the player's current inventory status. This allows the AI to make informed decisions about what actions it can suggest or trigger based on the player's current inventory, creating a more interactive and responsive experience.
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

    /// <summary>
    /// Processes the raw response received from the Ollama API and parses it from JSON format.
    /// Extracts the text message, the emotion to alter the visual interface, and the mechanical action to execute in the engine (Unity).
    /// </summary>
    /// <param name="raw">The raw text response (string) returned by the LLM.</param>
    protected virtual void OnAIResponse(string raw) // This method is called when we receive a response from the AI. It processes the raw response, updates the NPC's state and visuals based on the content of the response, and triggers any necessary game events based on the AI's requested actions. It also handles the UI changes for when the AI finishes responding, allowing the player to input their next message. This is a critical part of the interaction loop, as it determines how the NPC reacts to the AI's response and how it continues the conversation with the player.
    {
        isThinking = false;
        Debug.Log($"<color=cyan>[AI JSON]:</color> {raw}");

        if (string.IsNullOrEmpty(raw)) // If the AI response is empty, we should simply reset the NPC's state and allow the player to input their next message. This ensures that if the AI fails to generate a response for some reason, the interaction can continue without breaking, and the player can try again or say something else to prompt a new response from the AI.
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
        }; // This action will be called when the AI finishes "typing" its response in the story log. It resets the thinking panel, visual state, and input field to allow the player to continue the interaction. This ensures a smooth transition between the AI's response and the player's next input, maintaining the flow of the conversation.

        if (data == null) // If we couldn't parse the AI's response as JSON, we should treat the raw response as a simple text message from the NPC. This allows for some flexibility in the AI's responses, where it can still communicate with the player even if it fails to follow the strict JSON format, while also encouraging proper formatting for more complex interactions.
        {
            visualController?.SetState(NPCVisualController.NPCState.Talking);
            AddLog(npcName, raw, true, onTypingFinished);
        }

        else // If we successfully parsed the AI's response as JSON, we should handle it according to the structured data it contains, including the response text, any requested actions, and the emotional tone. This allows for a richer and more dynamic interaction where the AI can not only say things but also trigger specific behaviors and express emotions based on the content of its response.
        {
            if (!string.IsNullOrEmpty(data.response)) // If the AI provided a response message, we should display it in the story log with the appropriate formatting and emotion. This allows the NPC to communicate with the player in a more engaging way, using both text and visual cues to convey its feelings and intentions.
            {
                NPCVisualController.NPCEmotion emo = NPCVisualController.NPCEmotion.Neutral;

                if (!string.IsNullOrEmpty(data.emotion)) // If the AI provided an emotion in its response, we should parse it and set the NPC's visual state accordingly. This allows the NPC to express emotions visually based on the AI's response, creating a more immersive and engaging interaction for the player.
                {
                    string parsedEmo = data.emotion.ToLower().Trim();
                    if (parsedEmo == "happy") emo = NPCVisualController.NPCEmotion.Happy;
                    else if (parsedEmo == "sad") emo = NPCVisualController.NPCEmotion.Sad;
                }

                visualController?.SetState(NPCVisualController.NPCState.Talking, emo);
                if (storyLog != null) storyLog.SetEmotion(emo);

                AddLog(npcName, data.response, true, onTypingFinished);
            }

            else // If there is no response text but we still have an emotion or action, we should still update the visual state and allow the player to input their next message. This ensures that even if the AI doesn't say anything, it can still express emotions or trigger actions that affect the game state, while keeping the interaction flowing smoothly.
            {
                onTypingFinished.Invoke();
            }
            action = (data.action ?? "none").Trim().ToLowerInvariant();
        }

        if (customAIActions != null && customAIActions.Count > 0) // If we have any custom AI actions defined, we should check if any of them need to be triggered based on the AI's requested action or the player's input keywords. This allows for flexible and dynamic interactions where both AI decisions and player inputs can lead to specific game events, enhancing the interactivity and responsiveness of NPCs in the game.
        {
            foreach (var customAction in customAIActions) // Check if any custom AI action should be triggered based on the AI's requested action or the player's input keywords. This allows for flexible and dynamic interactions where both AI decisions and player inputs can lead to specific game events, enhancing the interactivity and responsiveness of NPCs in the game.
            {
                bool aiSaidIt = (!string.IsNullOrEmpty(customAction.aiActionName) && customAction.aiActionName.ToLowerInvariant() == action);
                bool playerForcedIt = HasKeyword(lastPlayerText, customAction.overrideKeywords);

                if (customAction.overrideKeywords != null && customAction.overrideKeywords.Count > 0) // If this custom action has override keywords defined, we need to check if the player forced it and if the AI also said it. If the player forced it but the AI didn't say it, we should block the action for security reasons, as it means the player is trying to trigger an action that the AI didn't intend to trigger based on its own logic. This is important to prevent potential exploits or unintended consequences from player input that tries to manipulate the AI's behavior in ways that don't make sense given the current game state.
                {
                    if (aiSaidIt && !playerForcedIt) // If the AI said the action but the player didn't force it, we allow it to proceed as normal, since this means the AI is trying to trigger the action based on its own logic, which is fine as long as it's allowed by the current game state.
                    {
                        Debug.Log($"<color=green>[IA ACTION]</color> La IA ha solicitado ejecutar '{customAction.aiActionName}' y no ha sido bloqueada porque el jugador no lo forzó.");
                    }

                    else if (!aiSaidIt && playerForcedIt) // If the player forced the action but the AI didn't say it, we block it for security reasons, as this means the player is trying to trigger an action that the AI didn't intend to trigger based on its own logic. This is important to prevent potential exploits or unintended consequences from player input that tries to manipulate the AI's behavior in ways that don't make sense given the current game state.
                    {
                        Debug.Log($"<color=red>[CANDADO ACTIVO]</color> La IA intentó lanzar '{customAction.aiActionName}' por su cuenta al coger la caja. Ha sido bloqueada por seguridad.");
                        aiSaidIt = false;
                    }
                }

                if (aiSaidIt || playerForcedIt) // If either the AI said the action or the player forced it, we should check if the action is allowed based on the current game state. If it is allowed, we trigger the associated event. If it's not allowed, we log that it was blocked for logical reasons. This ensures that actions are only executed when they make sense given the current state of the game, while still allowing for both AI-driven and player-driven triggers to lead to events when appropriate.
                {
                    if (IsActionAllowed(customAction.aiActionName)) // If the action is allowed based on the current game state, we should trigger the associated event to execute the action's effects in the game. This allows for dynamic interactions where both AI decisions and player inputs can lead to specific game events, enhancing the interactivity and responsiveness of NPCs in the game.
                    {
                        if (customAction.eventToTrigger != null && EventManager.Instance != null)
                        {
                            EventManager.Instance.TriggerEvent(customAction.eventToTrigger);
                            Debug.Log($"<color=green>[IA ACTION]</color> Evento disparado: {customAction.aiActionName}");
                        }
                    }

                    else // If the action is not allowed based on the current game state, we should log that it was blocked for logical reasons. This prevents the AI from trying to perform actions that don't make sense given the current state of the game, while still acknowledging that there was an attempt to trigger the action.
                    {
                        Debug.Log($"<color=orange>[IA BLOCKED]</color> Intento de ejecutar '{customAction.aiActionName}' bloqueado lógicamente.");
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

    protected void OnAIError(string err) // This method is called when there is an error in the AI response process. It resets the NPC's state, logs the error message in the story log, and allows the player to input their next message. This ensures that if something goes wrong with the AI interaction (e.g., a network error, parsing error, etc.), the NPC can recover gracefully and continue the interaction without breaking, while also providing feedback to the player about what went wrong.
    {
        isThinking = false; // Was an error so definitely not thinking anymore
        if (thinkingPanel != null) thinkingPanel.SetActive(false);

        visualController?.SetState(NPCVisualController.NPCState.Idle);
        if (storyLog != null) storyLog.AddLine($"<color=red>Error: {err}</color>");

        if (inputField != null)
        {
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    protected void AddLog(string speaker, string message, bool animate = false, System.Action onFinished = null) // This method adds a message to the story log with the specified speaker and formatting. If 'animate' is true, it will animate the text as if the NPC is "typing" it out, and call 'onFinished' when the animation is complete. If 'animate' is false, it will simply add the line to the log immediately and call 'onFinished' right away. This allows for dynamic presentation of the NPC's responses, making them more engaging when animated, while still supporting instant messages when needed.
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

        AddToHistory(speaker, message); // Add the message to the conversation history so that it can be included in the context for future AI responses. This ensures that the AI has access to the full conversation history when generating its responses, allowing for more coherent and context-aware interactions with the player.
    }

    protected void AddToHistory(string speaker, string message) // This method adds a message to the conversation history string, which is used to provide context for the AI's responses. It also ensures that the conversation history does not exceed a certain length (e.g., 2000 characters) by trimming older messages if necessary. This allows the AI to have access to recent conversation history without overwhelming it with too much information, while still maintaining enough context for coherent interactions.
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

    private AIResponse TryParseAIResponse(string raw) // This method attempts to parse the raw AI response as JSON to extract the structured data (response text, action, emotion). It first tries to parse the entire raw string as JSON, and if that fails, it looks for a JSON object within the raw string. If it still can't find valid JSON, it falls back to using a regex to extract a simple "response" field from the raw text. This allows for some flexibility in how the AI formats its responses while still trying to extract meaningful information for the NPC's behavior.
    {
        raw = raw?.Trim();
        try // First, we try to parse the entire raw string as JSON. This is the ideal case where the AI follows the expected format and provides a clean JSON response.
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

    public void LoadProfile(NPCProfile profile) // This method loads the NPC's profile data, which includes the NPC's name, personality prompt, first message, password, and system instructions. It updates the NPC's properties based on the provided profile, allowing for easy configuration of different NPCs with unique personalities and behaviors by simply creating different profiles. This is useful for setting up various characters in the game with distinct traits and responses without having to hardcode their attributes in the script.
    {
        if (profile == null) return;
        this.npcName = profile.npcName;
        this.personalityPrompt = profile.personalityPrompt;
        this.firstMessage = profile.firstMessage;
        if (!string.IsNullOrEmpty(profile.password)) this.password = profile.password;
        if (!string.IsNullOrEmpty(profile.systemInstruction)) this.systemInstruction = profile.systemInstruction;
    }
}