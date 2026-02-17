using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStoryNode", menuName = "Adventure/Story Node")]
public class StoryNode : ScriptableObject
{
    [TextArea(5, 10)]
    public string storyText;

    public List<StoryOption> options;

    [Space(20)]
    [Header("Configuración del PopUp")]
    public PopupData popupData; // Aquí guardamos toda la info del popup
}

[System.Serializable]
public class PopupData
{
    public bool showPopup = false;

    [Tooltip("Deja en 0 para que no se cierre solo.")]
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
    public string commandKeyword;
    public StoryNode nextNode;
    public string specialActionID;
}