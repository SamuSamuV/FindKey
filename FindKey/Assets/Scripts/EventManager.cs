using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class: EventManager
/// Description: Gestor centralizado de eventos del juego. Permite definir eventos con múltiples acciones (abrir apps, mostrar popups, cambiar fondo, etc) y ejecutarlos secuencialmente.
///              También mantiene un backlog de eventos ya ocurridos para evitar repeticiones no deseadas. Facilita la comunicación entre sistemas mediante eventos estáticos para apertura/cierre
///              de apps. Es el núcleo del sistema de eventos que impulsa la narrativa y las interacciones en FindKey.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; } // Singleton to global access

    public static event Action<string> OnAppOpened;
    public static event Action<string> OnAppClosed;

    public static void NotifyAppOpened(string appName) => OnAppOpened?.Invoke(appName);
    public static void NotifyAppClosed(string appName) => OnAppClosed?.Invoke(appName);

    [Header("Configuración UI")]
    public Transform popupContainer;
    public GameObject defaultPopupPrefab;
    public GameObject blackScreenPanel;

    [Header("Backlog (Eventos ya ocurridos)")]
    public List<GameEvent> executedEvents = new List<GameEvent>(); // List to keep track of events that have already been executed, to prevent repeating "play only once" events.

    private Queue<GameEvent> eventQueue = new Queue<GameEvent>(); // Queue to manage incoming events and ensure they are processed sequentially.
    private bool isPlayingEvent = false;
    private AudioSource managerAudioSource;

    private void Awake() // Singleton pattern implementation and setup of the audio source for event sounds.
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        managerAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start() // Try to find the popup container in the scene if it hasn't been assigned in the inspector.
    {
        if (popupContainer == null)
        {
            GameObject containerObj = GameObject.FindGameObjectWithTag("DeskopCanvas");
            if (containerObj != null) popupContainer = containerObj.transform;
        }
    }

    public void TriggerEvent(GameEvent gameEvent) // Main method to trigger an event. It checks if the event should only be played once and if it has already been executed, then adds it to the queue for processing.
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

    private IEnumerator ProcessEventQueue() // Coroutine that processes the event queue sequentially, executing each event's actions with their respective delays. It also marks events as executed to prevent repetition if needed.
    {
        isPlayingEvent = true;

        while (eventQueue.Count > 0) // Process each event in the queue one by one.
        {
            GameEvent currentEvent = eventQueue.Dequeue();

            executedEvents.Add(currentEvent);
            Debug.Log($"<color=cyan>[EventManager]</color> Iniciando evento: <b>{currentEvent.name}</b>");

            foreach (EventAction action in currentEvent.actions) // Execute each action of the event sequentially, respecting any specified delay before execution.
            {
                if (action.delayBeforeExecute > 0)
                {
                    yield return new WaitForSeconds(action.delayBeforeExecute);
                }

                yield return StartCoroutine(ExecuteAction(action)); // Execute the action and wait for it to complete before moving to the next one.
            }
        }

        isPlayingEvent = false;
    }

    private IEnumerator ExecuteAction(EventAction action) // Coroutine that executes a single action based on its type. It handles various action types such as waiting, playing sounds, opening/closing apps, showing popups, changing wallpaper, etc.
    {
        switch (action.actionType)
        {
            case EventActionType.Wait: // Simply wait for the specified time before proceeding to the next action.
                yield return new WaitForSeconds(action.waitTime);
                break;

            case EventActionType.PlaySound: // Play the specified sound using the manager's audio source. It checks if the sound is valid before attempting to play it.
                if (action.sound != null && action.sound.IsValid()) action.sound.PlayOn(managerAudioSource, true);
                break;

            case EventActionType.OpenApp: // Attempt to open the specified app by name. It checks if the AppLauncher instance exists and if the app name is not empty before trying to launch it.
                if (AppLauncher.Instance != null && !string.IsNullOrEmpty(action.appName))
                {
                    AppLauncher.Instance.LaunchApp(action.appName, Vector2.zero);
                }
                break;

            case EventActionType.CloseApp: // Attempt to close the specified app by name. If the app name is "all" or empty, it will close all open apps. Otherwise, it will try to find the specific app window and close it if found.
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

            case EventActionType.ShakeWindow: // Attempt to shake the specified app window by name. If the app name is "all" or empty, it will shake all open apps. Otherwise, it will try to find the specific app window and shake it if found.
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

            case EventActionType.ShowPopup: // Spawn a popup using the provided popup data. It checks if the popup data is valid before attempting to spawn it.
                SpawnEventPopup(action.popupData);
                break;

            case EventActionType.ChangeWallpaper: // Attempt to change the desktop wallpaper using the provided sprite. It looks for the WallpapersScript in the scene and calls its ChangeBackground method if found and if the new wallpaper is not null.
                WallpapersScript ws = FindObjectOfType<WallpapersScript>();
                if (ws != null && action.newWallpaper != null) ws.ChangeBackground(action.newWallpaper);
                break;

            case EventActionType.MinimizeApp: // Attempt to minimize the specified app window by name. If the app name is "all" or empty, it will minimize all open apps. Otherwise, it will try to find the specific app window and minimize it if found.
                AppWindow windowToMin = GetAppWindowByName(action.appName);
                if (windowToMin != null) windowToMin.Minimize();
                break;

            case EventActionType.ToggleBlackScreen: // Toggle the visibility of the black screen panel based on the action's toggle state. If the panel is being shown, it ensures it has a Canvas component with a high sorting order to appear above all other UI elements.
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

            case EventActionType.BringTaskbarToFront: // Attempt to bring the taskbar to the front by ensuring it has a Canvas component with a high sorting order. It looks for the TaskbarManager in the scene and modifies its Canvas settings if found.
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

            case EventActionType.ForceAIMessage: // Force all AI scripts in the scene to send a proactive message using the provided prompt. It finds all BaseAIScript instances in the scene and calls their ForceProactiveMessage method with the specified prompt.
                BaseAIScript[] ais = FindObjectsOfType<BaseAIScript>(true);
                foreach (BaseAIScript ai in ais) ai.ForceProactiveMessage(action.aiPromptMessage);
                break;

            case EventActionType.AddInventoryItem: // Add an item to the inventory using the provided item name and sprite. It looks for the MoveAppData in the scene to update the inventory state and then refreshes the inventory UI using the InventoryManager.
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

            case EventActionType.RemoveDesktopIcon: // Remove a desktop icon by app name. It looks for the DesktopManager in the scene and iterates through its list of icons to find one that matches the specified app name. If found, it removes the icon from the list and destroys the corresponding GameObject.
                DesktopManager desktopMgr = FindObjectOfType<DesktopManager>();
                if (desktopMgr != null)
                {
                    GameObject iconToDestroy = null;

                    for (int i = 0; i < desktopMgr.icons.Count; i++) // Iterate through the desktop icons to find the one that matches the specified app name.
                    {
                        GameObject iconGO = desktopMgr.icons[i];
                        if (iconGO != null)
                        {
                            DesktopIcon iconScript = iconGO.GetComponent<DesktopIcon>();
                            if (iconScript != null && iconScript.labelText != null && iconScript.labelText.text == action.appName)
                            {
                                iconToDestroy = iconGO;
                                break;
                            }
                        }
                    }

                    if (iconToDestroy != null)
                    {
                        desktopMgr.icons.Remove(iconToDestroy);
                        Destroy(iconToDestroy);
                        Debug.Log($"<color=red>[SISTEMA]</color> La aplicación '{action.appName}' ha sido desinstalada y borrada para siempre.");
                    }
                }
            break;

            case EventActionType.ChangeAdventureNode: // Force the adventure manager to load a specific story node. It looks for the AdventureManager in the scene and calls its ForceLoadNode method with the specified new story node. It also logs the action for debugging purposes.
                AdventureManager advManager = FindObjectOfType<AdventureManager>();

                if (advManager != null && action.newStoryNode != null)
                {
                    advManager.ForceLoadNode(action.newStoryNode);
                    Debug.Log($"<color=green>[SISTEMA]</color> El juego FindKey.exe ha sido forzado al nodo: {action.newStoryNode.name}");
                }

                else if (action.newStoryNode == null)
                {
                    Debug.LogError("<color=red>[EVENT ERROR]</color> No has asignado ningún 'New Story Node' en la acción ChangeAdventureNode.");
                }

            break;

            case EventActionType.PlayVideo: // Attempt to play a video in the specified app window. It looks for the app window by name and then tries to find a VideoPlayer component within it. If found, it assigns the specified video clip and sets up event handlers for when the video is prepared and when it finishes playing.
                AppWindow videoApp = GetAppWindowByName(action.appName);

                if (videoApp != null)
                {
                    videoApp.SetCloseAndMinimizeInteractable(false);

                    UnityEngine.Video.VideoPlayer videoPlayer = videoApp.GetComponentInChildren<UnityEngine.Video.VideoPlayer>(true);

                    if (videoPlayer != null)
                    {
                        if (action.videoClip != null)
                        {
                            videoPlayer.clip = action.videoClip;
                            videoPlayer.Prepare();

                            videoPlayer.prepareCompleted += (vp) => {
                                vp.Play();
                                Debug.Log($"<color=green>[EventManager]</color> Vídeo listo y reproduciéndose.");
                            };

                            videoPlayer.loopPointReached += (vp) => {
                                Debug.Log("<color=cyan>[EventManager]</color> El vídeo ha terminado. Saltando a la escena final...");
                                UnityEngine.SceneManagement.SceneManager.LoadScene("BlueScreen");
                            };
                        }
                    }
                }

                break;
        }
    }

    private AppWindow GetAppWindowByName(string appNameToFind) // Helper method to find an open app window by its name. It iterates through all open AppWindow instances and returns the one that matches the specified app name. If no matching window is found, it returns null.
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

    private void SpawnEventPopup(PopupData data) // Helper method to spawn a popup based on the provided PopupData. It checks if the data and popup container are valid, then instantiates the appropriate prefab (either a specific one defined in the data or a default one) and sets it up with the provided data.
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

    private void CloseAppByName(string appNameToClose) // Helper method to close an open app window by its name. If the app name is "all" or empty, it will close all open apps. Otherwise, it will try to find the specific app window and close it if found.
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