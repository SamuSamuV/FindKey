using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AdventureManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public StoryLog storyLog;
    public Moves movesScript;
    public MapViewer mapViewer;

    public Transform popupContainer;
    public GameObject defaultPopupPrefab;

    public AudioSource audioMaster;
    public AudioSource audioFront;
    public AudioSource audioBack;
    public AudioSource audioLeft;
    public AudioSource audioRight;

    public StoryNode currentNode;
    public StoryNode nodeAfterCatWin;

    private Coroutine popupSequenceCoroutine;

    void Start()
    {
        if (audioMaster == null) audioMaster = FindAudioSource("Main Camera");
        if (audioFront == null) audioFront = FindAudioSource("FrontSoundBox");
        if (audioBack == null) audioBack = FindAudioSource("BackSoundBox");
        if (audioLeft == null) audioLeft = FindAudioSource("LeftSoundBox");
        if (audioRight == null) audioRight = FindAudioSource("RightSoundBox");

        if (audioMaster == null) audioMaster = FindObjectOfType<AudioSource>();

        if (popupContainer == null)
        {
            GameObject containerObj = GameObject.FindGameObjectWithTag("DeskopCanvas");
            if (containerObj != null) popupContainer = containerObj.transform;
        }

        UpdateStoryVisuals();
        inputField.onSubmit.AddListener(ProcessInput);
        TryUpdateMap();
    }

    AudioSource FindAudioSource(string objectName)
    {
        GameObject go = GameObject.Find(objectName);
        if (go != null) return go.GetComponent<AudioSource>();
        return null;
    }

    void UpdateStoryVisuals()
    {
        if (currentNode != null)
        {
            storyLog.SetTextAnimated(currentNode.storyText);

            if (popupSequenceCoroutine != null) StopCoroutine(popupSequenceCoroutine);

            if (currentNode.popups != null && currentNode.popups.Count > 0)
            {
                popupSequenceCoroutine = StartCoroutine(RunPopupSequence(currentNode.popups));
            }

            UpdateChannelAudio(audioMaster, currentNode.masterSound);
            UpdateChannelAudio(audioFront, currentNode.frontSound);
            UpdateChannelAudio(audioBack, currentNode.backSound);
            UpdateChannelAudio(audioLeft, currentNode.leftSound);
            UpdateChannelAudio(audioRight, currentNode.rightSound);
        }
    }

    void UpdateChannelAudio(AudioSource source, AudioClip clip)
    {
        if (source == null) return;

        if (clip == null)
        {
            if (source.isPlaying) source.Stop();
            source.clip = null;
        }
        else
        {
            if (source.clip != clip)
            {
                source.clip = clip;
                source.Play();
            }
            else if (!source.isPlaying)
            {
                source.Play();
            }
        }
    }

    IEnumerator RunPopupSequence(List<PopupData> popups)
    {
        foreach (PopupData data in popups)
        {
            if (data.delayBeforeSpawn > 0)
                yield return new WaitForSeconds(data.delayBeforeSpawn);

            SpawnSinglePopup(data);
        }
    }

    void SpawnSinglePopup(PopupData data)
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

    void TryUpdateMap()
    {
        if (mapViewer == null) mapViewer = FindObjectOfType<MapViewer>();

        if (mapViewer != null)
        {
            mapViewer.UpdateMap(currentNode);
        }
    }

    void ShowError()
    {
        string previousText = storyLog.lastLoadedText;
        storyLog.SetTextAnimated($"<color=red>You can't do that here.</color>\n\n{previousText}");
    }

    public void ForceLoadNode(StoryNode newNode)
    {
        if (newNode == null) return;
        currentNode = newNode;
        UpdateStoryVisuals();
        TryUpdateMap();
    }
}