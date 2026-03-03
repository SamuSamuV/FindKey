using System.Collections.Generic;
using UnityEngine;

public enum TriggerCondition
{
    OnGameStart,
    OnAppOpened,
    OnAppClosed
}

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

public class SystemEventTrigger : MonoBehaviour
{
    [Header("Lista de Eventos del Sistema")]
    public List<EventTriggerEntry> triggers = new List<EventTriggerEntry>();

    private void Start()
    {
        foreach (var entry in triggers)
        {
            if (entry.condition == TriggerCondition.OnGameStart)
            {
                TriggerNow(entry.gameEventToTrigger);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.OnAppOpened += HandleAppOpened;
        EventManager.OnAppClosed += HandleAppClosed;
    }

    private void OnDisable()
    {
        EventManager.OnAppOpened -= HandleAppOpened;
        EventManager.OnAppClosed -= HandleAppClosed;
    }

    private void HandleAppOpened(string appName)
    {
        foreach (var entry in triggers)
        {
            if (entry.condition == TriggerCondition.OnAppOpened && entry.targetAppName == appName)
            {
                TriggerNow(entry.gameEventToTrigger);
            }
        }
    }

    private void HandleAppClosed(string appName)
    {
        foreach (var entry in triggers)
        {
            if (entry.condition == TriggerCondition.OnAppClosed && entry.targetAppName == appName)
            {
                TriggerNow(entry.gameEventToTrigger);
            }
        }
    }

    private void TriggerNow(GameEvent eventToTrigger)
    {
        if (eventToTrigger != null && EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(eventToTrigger);
        }
    }
}