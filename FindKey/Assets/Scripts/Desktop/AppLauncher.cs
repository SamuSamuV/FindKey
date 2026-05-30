/// <summary>
/// Class: AppLauncher
/// Description: This script is responsible for launching applications in the FindKey game. It manages the instantiation of application windows based on a given app name and launch
///              position. The script checks for existing instances of the application and handles their state (open, minimized) accordingly. It also manages the cascading of windows to prevent
///              overlap and registers opened applications with the TaskbarManager for easy access. The AppLauncher ensures that each application window is properly initialized and displayed on the
///              desktop when launched.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using UnityEngine;

public class AppLauncher : MonoBehaviour
{
    public static AppLauncher Instance { get; private set; } // Singleton to global access
    [Tooltip("Prefab por defecto por si falla la búsqueda. Ya no es estrictamente necesario.")]
    public GameObject appWindowPrefab;
    public Transform windowsParent;

    public Vector2 cascadeOffset = new Vector2(30f, -30f); // Displacement for cascading windows
    public int maxCascades = 10;
    private int windowCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void LaunchApp(string appName, Vector2 launchPosition) // Main function to launch an app by name and position
    {
        Sprite appIconSprite = null;
        GameObject prefabToSpawn = appWindowPrefab;

        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn) // Search for the app in the desktop manager's icon data
            {
                if (data.label == appName)
                {
                    appIconSprite = data.sprite;

                    if (data.windowApp != null)
                    {
                        prefabToSpawn = data.windowApp;
                    }

                    if (data.windowInstance != null)
                    {
                        AppWindow existingWindow = data.windowInstance.GetComponent<AppWindow>();

                        if (!data.isOpen)
                        {
                            data.isOpen = true;
                            if (existingWindow != null)
                            {
                                existingWindow.Reopen(); // Reopen the existing window if it was closed but kept alive
                                TaskbarManager.GetOrFindInstance()?.RegisterWindow(existingWindow, appName, appIconSprite); // Re-register the window with the taskbar in case it was closed
                            }

                            else
                            {
                                data.windowInstance.SetActive(true); // If for some reason the instance reference exists but the GameObject is inactive, activate it
                            }
                        }

                        else if (data.isMinimized)
                        {
                            if (existingWindow != null) existingWindow.Restore(); // Restore the window if it was minimized
                            else data.windowInstance.SetActive(true); // If for some reason the instance reference exists but the GameObject is inactive, activate it

                            data.isMinimized = false;
                        }

                        data.windowInstance.transform.SetAsLastSibling(); // Bring the existing window to the front
                        return;
                    }
                    data.isOpen = true;
                    break;
                }
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError($"[AppLauncher] No se pudo encontrar un prefab para abrir la app '{appName}'.");
            return;
        }

        GameObject appGO = Instantiate(prefabToSpawn, windowsParent); // Instantiate the app window as a child of the windows parent
        AppWindow appWindow = appGO.GetComponent<AppWindow>();

        appGO.transform.SetAsLastSibling(); // Ensure the new window is on top of others
        RectTransform rt = appGO.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(cascadeOffset.x * windowCount, cascadeOffset.y * windowCount); // Position the window with an offset based on the current window count to create a cascading effect

        windowCount++;
        if (windowCount >= maxCascades) windowCount = 0;

        TaskbarManager.GetOrFindInstance()?.RegisterWindow(appWindow, appName, appIconSprite); // Register the new window with the taskbar manager for easy access

        appWindow.appName = appName;

        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn) // Update the desktop manager's icon data to reference the new window instance
            {
                if (data.label == appName)
                {
                    data.windowInstance = appGO;
                    break;
                }
            }
        }

        EventManager.NotifyAppOpened(appName); // Notify the event manager that an app has been opened, allowing other systems to react accordingly (e.g., triggering events, updating UI)
    }
}