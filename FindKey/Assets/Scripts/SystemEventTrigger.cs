using System.Collections.Generic;
using UnityEngine;

public enum TriggerCondition // Enum to specify when the event should be triggered
{
    OnGameStart,
    OnAppOpened,
    OnAppClosed
}

/// <summary>
/// Class: EventTriggerEntry
/// Description: This class represents a single entry in the SystemEventTrigger's list of triggers. It contains the condition for triggering and the associated GameEvent.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
[System.Serializable]
public class EventTriggerEntry
{
    [Header("¿Cuándo se dispara?")]
    public TriggerCondition condition;

    [Tooltip("Nombre de la app (Ej: FindKey.exe). Déjalo vacío si usas OnGameStart.")]
    public string targetAppName;

    [Header("Evento a ejecutar")]
    public GameEvent gameEventToTrigger;
}

/// <summary>
/// Class: SystemEventTrigger
/// Description: This class listens for specific system events (like app opening/closing) and triggers corresponding GameEvents in Unity.
///              It allows you to define which GameEvent should be triggered based on the specified conditions.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class SystemEventTrigger : MonoBehaviour
{
    [Header("Lista de Eventos del Sistema")]
    public List<EventTriggerEntry> triggers = new List<EventTriggerEntry>(); // List of triggers to check against system events

    private void Start() // Check for OnGameStart triggers when the game starts
    {
        foreach (var entry in triggers)
        {
            if (entry.condition == TriggerCondition.OnGameStart)
            {
                TriggerNow(entry.gameEventToTrigger);
            }
        }
    }

    private void OnEnable() // Subscribe to system events when the object is enabled
    {
        EventManager.OnAppOpened += HandleAppOpened;
        EventManager.OnAppClosed += HandleAppClosed;
    }

    private void OnDisable() // Unsubscribe from system events when the object is disabled
    {
        EventManager.OnAppOpened -= HandleAppOpened;
        EventManager.OnAppClosed -= HandleAppClosed;
    }

    private void HandleAppOpened(string appName) // Check for OnAppOpened triggers when an app is opened
    {
        foreach (var entry in triggers)
        {
            if (entry.condition == TriggerCondition.OnAppOpened && entry.targetAppName == appName)
            {
                TriggerNow(entry.gameEventToTrigger);
            }
        }
    }

    private void HandleAppClosed(string appName) // Check for OnAppClosed triggers when an app is closed
    {
        foreach (var entry in triggers)
        {
            if (entry.condition == TriggerCondition.OnAppClosed && entry.targetAppName == appName)
            {
                TriggerNow(entry.gameEventToTrigger);
            }
        }
    }

    private void TriggerNow(GameEvent eventToTrigger) // Method to trigger the specified GameEvent
    {
        if (eventToTrigger != null && EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(eventToTrigger);
        }
    }
}