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