using UnityEngine;

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
                    if (data.isMinimized)
                    {
                        data.windowInstance.SetActive(true);
                        data.isMinimized = false;
                    }

                    if (data.isOpen)
                    {
                        Debug.Log($"La app '{appName}' ya está abierta.");
                        return;
                    }

                    data.isOpen = true;
                    break;
                }
            }
        }

        // Crear la ventana
        GameObject appGO = Instantiate(appWindowPrefab, windowsParent);
        AppWindow appWindow = appGO.GetComponent<AppWindow>();
    
        // Centrarla o colocarla
        RectTransform rt = appGO.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
    
        // Registrar en la barra de tareas (si tienes)
        TaskbarManager.Instance?.RegisterWindow(appWindow, appName);
    
        Debug.Log($"App '{appName}' lanzada correctamente.");

        appWindow.appName = appName;

        foreach (var data in dm.iconsToSpawn)
        {
            if (data.windowInstance == null)
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
