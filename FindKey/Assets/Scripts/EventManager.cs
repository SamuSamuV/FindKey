using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // <--- NECESARIO PARA LOS EVENTOS

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // --- LA EMISORA DE RADIO (Eventos Globales) ---
    public static event Action<string> OnAppOpened;
    public static event Action<string> OnAppClosed;

    public static void NotifyAppOpened(string appName) => OnAppOpened?.Invoke(appName);
    public static void NotifyAppClosed(string appName) => OnAppClosed?.Invoke(appName);
    // ----------------------------------------------

    [Header("Configuración UI")]
    public Transform popupContainer;
    public GameObject defaultPopupPrefab;

    [Header("Backlog (Eventos ya ocurridos)")]
    public List<GameEvent> executedEvents = new List<GameEvent>();

    private Queue<GameEvent> eventQueue = new Queue<GameEvent>();
    private bool isPlayingEvent = false;
    private AudioSource managerAudioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        managerAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        if (popupContainer == null)
        {
            GameObject containerObj = GameObject.FindGameObjectWithTag("DeskopCanvas");
            if (containerObj != null) popupContainer = containerObj.transform;
        }
    }

    public void TriggerEvent(GameEvent gameEvent)
    {
        if (gameEvent == null) return;

        if (gameEvent.playOnlyOnce && executedEvents.Contains(gameEvent))
        {
            Debug.Log($"[EventManager] El evento '{gameEvent.name}' ya ocurrió. Se ignora.");
            return;
        }

        eventQueue.Enqueue(gameEvent);

        if (!isPlayingEvent)
        {
            StartCoroutine(ProcessEventQueue());
        }
    }

    private IEnumerator ProcessEventQueue()
    {
        isPlayingEvent = true;

        while (eventQueue.Count > 0)
        {
            GameEvent currentEvent = eventQueue.Dequeue();

            executedEvents.Add(currentEvent);
            Debug.Log($"<color=cyan>[EventManager]</color> Iniciando evento: <b>{currentEvent.name}</b>");

            foreach (EventAction action in currentEvent.actions)
            {
                if (action.delayBeforeExecute > 0)
                {
                    yield return new WaitForSeconds(action.delayBeforeExecute);
                }

                yield return StartCoroutine(ExecuteAction(action));
            }
        }

        isPlayingEvent = false;
    }

    private IEnumerator ExecuteAction(EventAction action)
    {
        switch (action.actionType)
        {
            case EventActionType.Wait:
                yield return new WaitForSeconds(action.waitTime);
                break;

            case EventActionType.PlaySound:
                if (action.sound != null && action.sound.IsValid()) action.sound.PlayOn(managerAudioSource, true);
                break;

            case EventActionType.OpenApp:
                if (AppLauncher.Instance != null && !string.IsNullOrEmpty(action.appName))
                {
                    AppLauncher.Instance.LaunchApp(action.appName, Vector2.zero);
                }
                break;

            case EventActionType.CloseApp:
                CloseAppByName(action.appName);
                break;

            case EventActionType.ShowPopup:
                SpawnEventPopup(action.popupData);
                break;

            case EventActionType.ChangeWallpaper:
                WallpapersScript ws = FindObjectOfType<WallpapersScript>();
                if (ws != null && action.newWallpaper != null) ws.ChangeBackground(action.newWallpaper);
                break;
        }
    }

    private void SpawnEventPopup(PopupData data)
    {
        if (data == null || popupContainer == null) return;

        GameObject prefabToUse = data.specificPrefab != null ? data.specificPrefab : defaultPopupPrefab;
        if (prefabToUse != null)
        {
            GameObject newPopup = Instantiate(prefabToUse, popupContainer);
            PopupController controller = newPopup.GetComponent<PopupController>();
            if (controller != null) controller.Setup(data);
        }
    }

    private void CloseAppByName(string appNameToClose)
    {
        if (string.IsNullOrEmpty(appNameToClose)) return;

        AppWindow[] openWindows = FindObjectsOfType<AppWindow>(false);
        foreach (AppWindow window in openWindows)
        {
            if (window.appName == appNameToClose)
            {
                window.Close();
                break;
            }
        }
    }
}