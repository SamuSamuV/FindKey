using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AppLauncher : MonoBehaviour
{
    public static AppLauncher Instance { get; private set; }
    public GameObject appWindowPrefab;
    public Transform windowsParent;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void LaunchApp(string appName, Vector2 launchPosition)
    {
        if (!appWindowPrefab) return;

        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == appName)
                {
                    if (data.windowInstance != null)
                    {
                        if (data.isMinimized)
                        {
                            data.windowInstance.SetActive(true);
                            data.isMinimized = false;
                                                    }

                        data.windowInstance.transform.SetAsLastSibling();

                        Debug.Log($"La app '{appName}' ya estaba abierta. Trayendo al frente.");
                        return;
                    }

                    data.isOpen = true;
                    break;
                }
            }
        }

        GameObject appGO = Instantiate(appWindowPrefab, windowsParent);
        AppWindow appWindow = appGO.GetComponent<AppWindow>();

        appGO.transform.SetAsLastSibling();

        RectTransform rt = appGO.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        TaskbarManager.GetOrFindInstance()?.RegisterWindow(appWindow, appName);

        Debug.Log($"App '{appName}' lanzada correctamente.");

        appWindow.appName = appName;

        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == appName)
                {
                    data.windowInstance = appGO;
                    break;
                }
            }
        }
    }
}
