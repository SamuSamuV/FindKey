using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public static event Action<string> OnAppOpened;
    public static event Action<string> OnAppClosed;

    public static void NotifyAppOpened(string appName) => OnAppOpened?.Invoke(appName);
    public static void NotifyAppClosed(string appName) => OnAppClosed?.Invoke(appName);

    [Header("Configuración UI")]
    public Transform popupContainer;
    public GameObject defaultPopupPrefab;
    public GameObject blackScreenPanel;

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
                if (string.IsNullOrEmpty(action.appName) || action.appName.ToLowerInvariant() == "all")
                {
                    AppWindow[] allWindows = FindObjectsOfType<AppWindow>(false);
                    foreach (AppWindow win in allWindows)
                    {
                        win.Close();
                    }
                }
                else
                {
                    AppWindow windowToClose = GetAppWindowByName(action.appName);
                    if (windowToClose != null)
                        CloseAppByName(action.appName);
                }
                break;

            case EventActionType.ShakeWindow:
                if (string.IsNullOrEmpty(action.appName) || action.appName.ToLowerInvariant() == "all")
                {
                    AppWindow[] allWindows = FindObjectsOfType<AppWindow>(false);
                    foreach (AppWindow win in allWindows) win.ShakeWindow(action.shakeDuration, action.shakeMagnitude);
                }
                else
                {
                    AppWindow windowToShake = GetAppWindowByName(action.appName);
                    if (windowToShake != null) windowToShake.ShakeWindow(action.shakeDuration, action.shakeMagnitude);
                }
                break;

            case EventActionType.ShowPopup:
                SpawnEventPopup(action.popupData);
                break;

            case EventActionType.ChangeWallpaper:
                WallpapersScript ws = FindObjectOfType<WallpapersScript>();
                if (ws != null && action.newWallpaper != null) ws.ChangeBackground(action.newWallpaper);
                break;

            case EventActionType.MinimizeApp:
                AppWindow windowToMin = GetAppWindowByName(action.appName);
                if (windowToMin != null) windowToMin.Minimize();
                break;

            case EventActionType.ToggleBlackScreen:
                if (blackScreenPanel != null)
                {
                    blackScreenPanel.SetActive(action.toggleState);

                    if (action.toggleState)
                    {
                        Canvas panelCanvas = blackScreenPanel.GetComponent<Canvas>();
                        if (panelCanvas == null)
                        {
                            panelCanvas = blackScreenPanel.AddComponent<Canvas>();
                            blackScreenPanel.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                        }

                        panelCanvas.overrideSorting = true;
                        panelCanvas.sortingOrder = 50;
                    }
                }
                break;

            case EventActionType.BringTaskbarToFront:
                TaskbarManager taskbar = FindObjectOfType<TaskbarManager>(true);

                if (taskbar != null)
                {
                    Canvas taskbarCanvas = taskbar.GetComponent<Canvas>();
                    if (taskbarCanvas == null)
                    {
                        taskbarCanvas = taskbar.gameObject.AddComponent<Canvas>();
                        taskbar.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>(); 
                    }

                    taskbarCanvas.overrideSorting = true;
                    taskbarCanvas.sortingOrder = 100;
                }
                break;

            case EventActionType.ForceAIMessage:
                BaseAIScript[] ais = FindObjectsOfType<BaseAIScript>(true);
                foreach (BaseAIScript ai in ais) ai.ForceProactiveMessage(action.aiPromptMessage);
                break;

            case EventActionType.AddInventoryItem:
                MoveAppData moveData = FindObjectOfType<MoveAppData>();

                if (moveData != null)
                {
                    string nameLower = action.itemName.ToLowerInvariant();

                    if (nameLower.Contains("reparada") || nameLower.Contains("repaired") || nameLower == "caja")
                    {
                        moveData.isChestRepaired = true;

                        int removedCount = moveData.savedItems.RemoveAll(item =>
                            item.itemName.ToLowerInvariant().Contains("corrupta") ||
                            item.itemName.ToLowerInvariant().Contains("rota"));

                        if (removedCount > 0)
                        {
                            Debug.Log("<color=yellow>[INVENTARIO]</color> Caja corrupta destruida y eliminada de la mochila.");
                        }
                    }
                    else if (nameLower.Contains("caja") || nameLower.Contains("chest"))
                    {
                        moveData.hasChest = true;
                    }
                    else if (nameLower.Contains("hacha") || nameLower.Contains("axe"))
                    {
                        moveData.hasAxe = true;
                    }

                    if (!moveData.savedItems.Exists(item => item.itemName == action.itemName))
                    {
                        moveData.savedItems.Add(new SavedInventoryItem { itemName = action.itemName, itemSprite = action.itemSprite });
                        Debug.Log($"<color=green>[INVENTARIO]</color> Nuevo objeto añadido: {action.itemName}");
                    }
                }

                InventoryManager invManager = FindObjectOfType<InventoryManager>(true);
                if (invManager != null)
                {
                    invManager.RefreshInventory();
                }
                break;
        }
    }

    private AppWindow GetAppWindowByName(string appNameToFind)
    {
        if (string.IsNullOrEmpty(appNameToFind)) return null;

        AppWindow[] openWindows = FindObjectsOfType<AppWindow>(false);
        foreach (AppWindow window in openWindows)
        {
            if (window.appName == appNameToFind)
            {
                return window;
            }
        }
        return null;
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