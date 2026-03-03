using UnityEngine;

public enum TriggerCondition
{
    OnGameStart,
    OnAppOpened,
    OnAppClosed
}

public class SystemEventTrigger : MonoBehaviour
{
    [Header("¿Cuándo se dispara el evento?")]
    public TriggerCondition condition;

    [Header("Parámetros Condición")]
    [Tooltip("El nombre de la app. Ej: FindKey.exe (Solo necesario si eliges OnAppOpened u OnAppClosed)")]
    public string targetAppName;

    [Header("El Evento a disparar")]
    public GameEvent gameEventToTrigger;

    private void Start()
    {
        // Si el diseñador marcó que salte al empezar el juego (o al cargar la pantalla)
        if (condition == TriggerCondition.OnGameStart)
        {
            TriggerNow();
        }
    }

    private void OnEnable()
    {
        // Enchufamos la antena a la radio
        EventManager.OnAppOpened += HandleAppOpened;
        EventManager.OnAppClosed += HandleAppClosed;
    }

    private void OnDisable()
    {
        // Desenchufamos la antena si este objeto se destruye (vital para evitar errores)
        EventManager.OnAppOpened -= HandleAppOpened;
        EventManager.OnAppClosed -= HandleAppClosed;
    }

    private void HandleAppOpened(string appName)
    {
        if (condition == TriggerCondition.OnAppOpened && appName == targetAppName)
        {
            TriggerNow();
        }
    }

    private void HandleAppClosed(string appName)
    {
        if (condition == TriggerCondition.OnAppClosed && appName == targetAppName)
        {
            TriggerNow();
        }
    }

    private void TriggerNow()
    {
        if (gameEventToTrigger != null && EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(gameEventToTrigger);
        }
    }
}