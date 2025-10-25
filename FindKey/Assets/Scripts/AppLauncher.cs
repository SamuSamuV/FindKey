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

        AppWindow existing = FindObjectOfType<AppWindow>();
        if (existing != null && existing.isOpen)
        {
            existing.Restore();
            existing.transform.SetAsLastSibling();
            return;
        }

        GameObject go = Instantiate(appWindowPrefab, windowsParent);
        AppWindow w = go.GetComponent<AppWindow>();
        w.Setup(appName);
        w.isOpen = true;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        TaskbarManager.GetOrFindInstance()?.RegisterWindow(w, appName);
    }
}
