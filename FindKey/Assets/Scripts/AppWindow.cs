using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppWindow : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Button closeButton;
    public Button minimizeButton;

    [HideInInspector] public bool isMinimized = false;

    protected virtual void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (minimizeButton != null) minimizeButton.onClick.AddListener(Minimize);
    }

    public virtual void Setup(string title)
    {
        if (titleText) titleText.text = title;
    }

    public virtual void Close()
    {
        TaskbarManager.GetOrFindInstance()?.UnregisterWindow(this);
        Destroy(gameObject);
    }

    public virtual void Minimize()
    {
        if (isMinimized) return;
        isMinimized = true;
        gameObject.SetActive(false);
    }

    public virtual void Restore()
    {
        if (!isMinimized) return;
        isMinimized = false;
        gameObject.SetActive(true);
    }

    public void ToggleMinimizeRestore()
    {
        if (isMinimized) Restore(); else Minimize();
    }
}
