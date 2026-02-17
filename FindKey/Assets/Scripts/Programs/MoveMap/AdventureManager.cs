using UnityEngine;
using TMPro;

public class AdventureManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public StoryLog storyLog;
    public Moves movesScript;

    [Header("UI y Audio References")]
    public Transform popupContainer;
    public GameObject defaultPopupPrefab;
    public AudioSource audioSource;

    [Header("Estado Actual")]
    public StoryNode currentNode;
    public StoryNode nodeAfterCatWin;

    void Start()
    {
        if (popupContainer == null)
        {
            GameObject containerObj = GameObject.FindGameObjectWithTag("DeskopCanvas");

            popupContainer = containerObj.transform;
        }

        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
        }

        UpdateStoryVisuals();
        inputField.onSubmit.AddListener(ProcessInput);
        TryUpdateMap();
    }

    void UpdateStoryVisuals()
    {
        if (currentNode != null)
        {
            storyLog.SetTextAnimated(currentNode.storyText);
            CheckForEvents();
        }
    }

    void CheckForEvents()
    {
        PopupData data = currentNode.popupData;

        if (data.showPopup)
        {
            GameObject prefabToUse = data.specificPrefab != null ? data.specificPrefab : defaultPopupPrefab;

            if (prefabToUse != null)
            {
                GameObject newPopup = Instantiate(prefabToUse, popupContainer);

                PopupController controller = newPopup.GetComponent<PopupController>();
                if (controller != null)
                {
                    controller.Setup(data);
                }
            }

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
            if (option.validInputs != null)
            {
                foreach (string keyword in option.validInputs.synonyms)
                {
                    if (keyword.ToLower().Trim() == input)
                    {
                        ExecuteOption(option);
                        return;
                    }
                }
            }
        }

        ShowError();
    }

    void ExecuteOption(StoryOption option)
    {
        if (option.actionType != StoryAction.None)
        {
            ExecuteSpecialAction(option.actionType);
        }

        if (option.nextNode != null)
        {
            currentNode = option.nextNode;
            UpdateStoryVisuals();
            TryUpdateMap();
        }
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

    void ExecuteSpecialAction(StoryAction action)
    {
        if (movesScript == null) return;

        switch (action)
        {
            case StoryAction.TriggerCat:
                movesScript.GoToCatPosition();
                break;

            case StoryAction.PickAxe:
                movesScript.PickAxe();
                break;

            case StoryAction.LookPainting:
                movesScript.LookPainting();
                break;

            case StoryAction.Die:
                movesScript.GoFirstRightDie();
                break;

            case StoryAction.None:
            default:
                break;
        }
    }

    public void ForceLoadNode(StoryNode newNode)
    {
        if (newNode == null) return;
        currentNode = newNode;
        UpdateStoryVisuals();
        TryUpdateMap();
    }
}