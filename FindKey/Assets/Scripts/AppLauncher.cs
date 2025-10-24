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
        GameObject go = Instantiate(appWindowPrefab, windowsParent);
        AppWindow w = go.GetComponent<AppWindow>();
        w.Setup(appName);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = launchPosition;

        // Use el helper seguro
        TaskbarManager.GetOrFindInstance()?.RegisterWindow(w, appName);
    }
}
