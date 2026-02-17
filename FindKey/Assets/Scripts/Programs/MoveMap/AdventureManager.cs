using UnityEngine;
using TMPro;

public class AdventureManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public StoryLog storyLog;
    public Moves movesScript;

    [Header("UI y Audio References")]
    public Transform popupContainer;
    public GameObject defaultPopupPrefab; // Prefab por defecto (diseño estándar)
    public AudioSource audioSource;

    [Header("Estado Actual")]
    public StoryNode currentNode;
    public StoryNode nodeAfterCatWin;

    void Start()
    {
        UpdateStoryVisuals();
        inputField.onSubmit.AddListener(ProcessInput);
        TryUpdateMap();
    }

    void UpdateStoryVisuals()
    {
        if (currentNode != null)
        {
            storyLog.SetTextAnimated(currentNode.storyText);
            CheckForEvents(); // Verificamos eventos al cambiar de nodo
        }
    }

    void CheckForEvents()
    {
        // Accedemos a la estructura de datos que creamos
        PopupData data = currentNode.popupData;

        // 1. Verificamos si el popup está activado
        if (data.showPopup)
        {
            // Determinamos qué prefab usar (el específico del nodo o el general)
            GameObject prefabToUse = data.specificPrefab != null ? data.specificPrefab : defaultPopupPrefab;

            if (prefabToUse != null)
            {
                // Instanciamos
                GameObject newPopup = Instantiate(prefabToUse, popupContainer);

                // Buscamos el script controlador y le pasamos los datos
                PopupController controller = newPopup.GetComponent<PopupController>();
                if (controller != null)
                {
                    controller.Setup(data);
                }
            }

            // 2. Reproducimos el sonido si existe
            if (data.sound != null && audioSource != null)
            {
                audioSource.PlayOneShot(data.sound);
            }
        }
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
                if (!string.IsNullOrEmpty(option.specialActionID)) ExecuteSpecialAction(option.specialActionID);

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

    public void ForceLoadNode(StoryNode newNode)
    {
        if (newNode == null) return;
        currentNode = newNode;
        UpdateStoryVisuals();
        TryUpdateMap();
    }
}