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
    [Header("Popups Sequence")]
    public List<PopupData> popups;

    [Space(20)]
    [Header("Audio Channels (Empty = Stop)")]
    public AudioClip masterSound;
    public AudioClip frontSound;
    public AudioClip backSound;
    public AudioClip leftSound;
    public AudioClip rightSound;
}

[System.Serializable]
public class PopupData
{
    public float delayBeforeSpawn = 0f;
    public float duration = 0f;
    public Vector2 position;
    public GameObject specificPrefab;
    public string title;
    [TextArea(3, 5)]
    public string message;
    public Sprite image;
    public AudioClip sound;
}

[System.Serializable]
public class StoryOption
{
    public CommandVocabulary validInputs;
    public StoryNode nextNode;
    public StoryAction actionType;
}