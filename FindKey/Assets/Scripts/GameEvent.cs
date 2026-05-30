/// <summary>
/// Class: GameEvent
/// Description: ScriptableObject that defines an event in the game, which can consist of a sequence of actions. Each action can be of various types
///              (waiting, playing sound, opening apps, etc.) and can have specific parameters. This allows for flexible and modular event creation that can be easily edited in the Unity Inspector.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;
using System.Collections.Generic;

public enum EventActionType
{
    Wait, // Just wait
    PlaySound, // Play a sound effect or music
    OpenApp, // Open an application window
    CloseApp, // Close an application window
    ShowPopup, // Show a popup message on the screen
    ChangeWallpaper, // Change the desktop wallpaper
    ShakeWindow, // Shake a specific application window or the entire screen
    MinimizeApp, // Minimize an application window
    ToggleBlackScreen, // Toggle a full-screen black overlay on/off
    BringTaskbarToFront, // Bring the taskbar to the front of all windows
    ForceAIMessage, // Force a specific message to appear in the AI chat interface
    AddInventoryItem, // Add an item to the player's inventory
    RemoveDesktopIcon, // Remove a specific icon from the desktop
    ChangeAdventureNode, // Change the current node in an adventure/story system
    PlayVideo, // Play a video clip
}

/// <summary>
/// Class: EventAction
/// Description: Defines a single action that can be part of a GameEvent. Each action has a type (defined by EventActionType) and parameters relevant to that type.
///              For example, a PlaySound action would have sound settings, while an OpenApp action would specify the app name. The delayBeforeExecute parameter allows for timing control
///              between actions in a sequence.
/// </summary>
[System.Serializable]
public class EventAction
{
    public EventActionType actionType;

    [Tooltip("Tiempo a esperar ANTES de ejecutar esta acción (útil para encadenar).")]
    public float delayBeforeExecute = 0f;

    [Header("Parámetros de Tiempo (Wait)")]
    [Tooltip("Tiempo que detendrá toda la cola de eventos.")]
    public float waitTime = 1f;

    [Header("Parámetros de App (Open/Close/Shake/Minimize)")]
    [Tooltip("Nombre exacto de la app. Déjalo vacío o pon 'all' en ShakeWindow/CloseApp para afectar a todas.")]
    public string appName;

    [Header("Parámetros de Temblor (ShakeWindow)")]
    [Tooltip("Intensidad o fuerza del temblor.")]
    public float shakeMagnitude = 10f;
    [Tooltip("Duración del temblor en segundos.")]
    public float shakeDuration = 0.5f;

    [Header("Parámetros de Sonido")]
    public SoundSettings sound;

    [Header("Parámetros de Popup")]
    public PopupData popupData;

    [Header("Parámetros de Sistema")]
    public Sprite newWallpaper;

    [Header("Pantalla Negra (ToggleBlackScreen)")]
    public bool toggleState = false;

    [Header("Mensaje IA (ForceAIMessage)")]
    [TextArea(2, 4)]
    public string aiPromptMessage;

    [Header("Inventario (AddInventoryItem)")]
    public string itemName;
    public Sprite itemSprite;

    [Header("Aventura (ChangeAdventureNode)")]
    public StoryNode newStoryNode;

    [Header("Parámetros de Video (PlayVideo)")]
    [Tooltip("El clip de video que quieres reproducir.")]
    public UnityEngine.Video.VideoClip videoClip;
}

/// <summary>
/// Class: GameEvent
/// Description: ScriptableObject that represents a game event, which can consist of a sequence of EventActions. It includes a description for the designer and a flag to determine
///              if the event should only be played once per game.
/// </summary>
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "OS System/Game Event")]

public class GameEvent : ScriptableObject
{
    [TextArea(2, 4)]
    public string eventDescription = "Describe qué hace este evento...";

    [Tooltip("Si es True, el EventManager lo guardará en el backlog y nunca más dejará que se repita en esta partida.")]
    public bool playOnlyOnce = true;

    [Header("Secuencia de Acciones")]
    public List<EventAction> actions;
}