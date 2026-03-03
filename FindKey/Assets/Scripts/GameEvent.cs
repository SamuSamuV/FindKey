using UnityEngine;
using System.Collections.Generic;

public enum EventActionType
{
    Wait,
    PlaySound,
    OpenApp,
    CloseApp,
    ShowPopup,
    ChangeWallpaper
}
[System.Serializable]
public class EventAction
{
    public EventActionType actionType;
    
    [Tooltip("Tiempo a esperar ANTES de ejecutar esta acción (útil para encadenar).")]
    public float delayBeforeExecute = 0f;

    [Header("Parámetros de Tiempo (Wait)")]
    [Tooltip("Tiempo que detendrá toda la cola de eventos.")]
    public float waitTime = 1f;

    [Header("Parámetros de App (Open/Close)")]
    [Tooltip("Nombre exacto de la app (Ej: FindKey.exe, Enemy Encounter)")]
    public string appName; 

    [Header("Parámetros de Sonido")]
    public SoundSettings sound;

    [Header("Parámetros de Popup")]
    public PopupData popupData;

    [Header("Parámetros de Sistema")]
    public Sprite newWallpaper;
}

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
