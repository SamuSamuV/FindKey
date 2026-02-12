using UnityEngine;
using TMPro;

public class AdventureManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public StoryLog storyLog;

    [Header("Estado Actual")]
    public StoryNode currentNode; // Arrastra aquí la "Habitación Inicial"

    // Referencias a tus otros scripts para cosas especiales
    public Moves movesScript;

    void Start()
    {
        UpdateStoryVisuals();
        inputField.onSubmit.AddListener(ProcessInput);
    }

    void UpdateStoryVisuals()
    {
        if (currentNode != null)
        {
            storyLog.SetTextAnimated(currentNode.storyText);
        }
    }

    void ProcessInput(string input)
    {
        if (string.IsNullOrEmpty(input) || currentNode == null) return;

        input = input.ToLower().Trim();
        inputField.text = "";
        inputField.ActivateInputField();

        // 1. Buscamos si el input coincide con alguna opción del nodo actual
        foreach (var option in currentNode.options)
        {
            if (option.commandKeyword.ToLower() == input)
            {
                // ¿Tiene alguna acción especial? (Para conectar con tu código antiguo)
                if (!string.IsNullOrEmpty(option.specialActionID))
                {
                    ExecuteSpecialAction(option.specialActionID);
                }

                // Cambiamos de nodo si hay uno asignado
                if (option.nextNode != null)
                {
                    currentNode = option.nextNode;
                    UpdateStoryVisuals();
                }
                return;
            }
        }

        // 2. Si no coincide con nada
        ShowError();
    }

    void ShowError()
    {
        string previousText = storyLog.lastLoadedText;
        string message = $"<color=red>You can't do that here.</color>\n\n{previousText}";
        storyLog.SetTextAnimated(message);
    }

    // Aquí conectas tu lógica antigua con el sistema nuevo
    void ExecuteSpecialAction(string actionID)
    {
        if (actionID == "TriggerCat") movesScript.GoToCatPosition();
        if (actionID == "PickAxe") movesScript.PickAxe();
        if (actionID == "Die") movesScript.GoFirstRightDie();
    }
}