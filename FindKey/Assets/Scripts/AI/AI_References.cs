/// <summary>
/// Class: AI_References
/// Description: This script serves as a centralized reference holder for various components related to the AI system in the FindKey game.
///              It includes references to the OllamaClient for AI interactions, input fields for user queries, text output for displaying AI responses, a StoryLog for tracking the narrative,
///              and an NPCVisualController for managing visual representations of NPCs. Additionally, it contains a reference to a thinkingPanel GameObject that can be used to indicate when the
///              AI is processing a response. This script allows for easy access to these components from other scripts that need to interact with the AI system, promoting better organization and
///              modularity in the codebase.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using TMPro;
using UnityEngine;

public class AI_References : MonoBehaviour
{
    public OllamaClient ollamaClient;
    public TMP_InputField inputField;
    public TextMeshProUGUI chatOutput;
    public StoryLog storyLog;
    public NPCVisualController visualController;

    [Header("UI Adicional")]
    public GameObject thinkingPanel;
}