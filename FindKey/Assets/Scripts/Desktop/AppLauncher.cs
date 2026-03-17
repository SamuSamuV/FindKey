using UnityEngine;

public class AppLauncher : MonoBehaviour
{
    public static AppLauncher Instance { get; private set; }
    public GameObject appWindowPrefab;
    public Transform windowsParent;

    public Vector2 cascadeOffset = new Vector2(30f, -30f);
    public int maxCascades = 10;
    private int windowCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void LaunchApp(string appName, Vector2 launchPosition)
    {
        if (!appWindowPrefab) return;

        Sprite appIconSprite = null;

        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == appName)
                {
                    appIconSprite = data.sprite;

                    if (data.windowInstance != null)
                    {
                        AppWindow existingWindow = data.windowInstance.GetComponent<AppWindow>();

                        if (!data.isOpen)
                        {
                            data.isOpen = true;
                            if (existingWindow != null)
                            {
                                existingWindow.Reopen();
                                TaskbarManager.GetOrFindInstance()?.RegisterWindow(existingWindow, appName, appIconSprite);
                            }
                            else
                            {
                                data.windowInstance.SetActive(true);
                            }
                        }
                        else if (data.isMinimized)
                        {
                            if (existingWindow != null) existingWindow.Restore();
                            else data.windowInstance.SetActive(true);

                            data.isMinimized = false;
                        }

                        data.windowInstance.transform.SetAsLastSibling();
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
        rt.anchoredPosition = new Vector2(cascadeOffset.x * windowCount, cascadeOffset.y * windowCount);

        windowCount++;
        if (windowCount >= maxCascades) windowCount = 0;

        TaskbarManager.GetOrFindInstance()?.RegisterWindow(appWindow, appName, appIconSprite);

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

        EventManager.NotifyAppOpened(appName);
    }
}