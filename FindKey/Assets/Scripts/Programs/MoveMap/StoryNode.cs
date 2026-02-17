using UnityEngine;
using System.Collections.Generic;

public enum StoryAction
{
    None, TriggerCat, PickAxe, LookPainting, Die
}

[CreateAssetMenu(fileName = "NewStoryNode", menuName = "Adventure/Story Node")]
public class StoryNode : ScriptableObject
{
    [TextArea(5, 10)]
    public string storyText;
    public List<StoryOption> options;

    [Space(20)]
    [Header("Configuración del PopUp")]
    public PopupData popupData;
}

[System.Serializable]
public class PopupData
{
    public bool showPopup = false;
    public float duration = 0f;
    public string title;
    [TextArea(3, 5)]
    public string message;
    public Sprite image;
    public AudioClip sound;
    public GameObject specificPrefab;
}

[System.Serializable]
public class StoryOption
{
    public CommandVocabulary validInputs;

    public StoryNode nextNode;

    [Header("Acción Especial")]
    public StoryAction actionType;
}