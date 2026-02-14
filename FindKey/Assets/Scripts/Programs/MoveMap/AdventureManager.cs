using UnityEngine;
using TMPro;

public class AdventureManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public StoryLog storyLog;
    public Moves movesScript; // Este sí arrástralo (está en el mismo prefab)

    [Header("Estado Actual")]
    public StoryNode currentNode;
    public StoryNode nodeAfterCatWin;

    void Start()
    {
        UpdateStoryVisuals();
        inputField.onSubmit.AddListener(ProcessInput);

        // Al abrir la app, intentamos actualizar el mapa si está abierto
        TryUpdateMap();
    }

    void UpdateStoryVisuals()
    {
        if (currentNode != null) storyLog.SetTextAnimated(currentNode.storyText);
    }

    void ProcessInput(string input)
    {
        if (string.IsNullOrEmpty(input) || currentNode == null) return;

        input = input.ToLower().Trim();
        inputField.text = "";
        inputField.ActivateInputField();

        foreach (var option in currentNode.options)
        {
            if (option.commandKeyword.ToLower() == input)
            {
                if (!string.IsNullOrEmpty(option.specialActionID))
                    ExecuteSpecialAction(option.specialActionID);

                if (option.nextNode != null)
                {
                    currentNode = option.nextNode;
                    UpdateStoryVisuals();

                    TryUpdateMap();
                }

                return;
            }
        }
        ShowError();
    }

    void TryUpdateMap()
    {
        MapViewer map = FindObjectOfType<MapViewer>();

        if (map != null)
        {
            map.UpdateMap(currentNode);
        }
    }

    void ShowError()
    {
        string previousText = storyLog.lastLoadedText;
        storyLog.SetTextAnimated($"<color=red>You can't do that here.</color>\n\n{previousText}");
    }

    void ExecuteSpecialAction(string actionID)
    {
        if (movesScript == null) return;
        if (actionID == "TriggerCat") movesScript.GoToCatPosition();
        if (actionID == "PickAxe") movesScript.PickAxe();
        if (actionID == "LookPainting") movesScript.LookPainting();
        if (actionID == "Die") movesScript.GoFirstRightDie();
    }

    // Añade esto en AdventureManager.cs
    public void ForceLoadNode(StoryNode newNode)
    {
        if (newNode == null) return;

        currentNode = newNode;       // Cambiamos el nodo
        UpdateStoryVisuals();        // Actualizamos texto
        TryUpdateMap();              // Actualizamos mapa
    }
}