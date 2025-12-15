using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

// Ejecutar antes que la mayoría para asegurarnos que Awake se llame temprano
[DefaultExecutionOrder(-100)]
public class TaskbarManager : MonoBehaviour
{
    public static TaskbarManager Instance { get; private set; }

    public Button menuButton;
    public GameObject menuPanel;
    public Button[] optionButtons;

    [Header("Taskbar Icons")]
    public Transform taskbarIconsParent;
    public GameObject taskbarButtonPrefab;

    private Dictionary<AppWindow, GameObject> windowToTaskButton = new Dictionary<AppWindow, GameObject>();

    private void Awake()
    {
        // Singleton básico (si hay otro, lo destruye para mantener 1 instancia)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("TaskbarManager: otra instancia ya existe, destruyendo esta.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("TaskbarManager: Instance asignada en Awake.");
    }

    private void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (menuButton != null) menuButton.onClick.AddListener(ToggleMenu);
        if (optionButtons != null)
        {
            foreach (var b in optionButtons) b.onClick.AddListener(OnOptionClicked);
        }
    }

    public static TaskbarManager GetOrFindInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<TaskbarManager>();
            if (Instance != null) Debug.Log("TaskbarManager: Encontrada instancia por FindObjectOfType.");
            else Debug.LogWarning("TaskbarManager: NO se encontró ninguna instancia en escena.");
        }
        return Instance;
    }

    public void ToggleMenu()
    {
        if (menuPanel == null) return;
        menuPanel.SetActive(!menuPanel.activeSelf);
    }

    private void OnOptionClicked()
    {
        SoundManager.Instance?.Play("failedto");
    }

    public void RegisterWindow(AppWindow window, string appName)
    {
        if (taskbarButtonPrefab == null || taskbarIconsParent == null)
        {
            Debug.LogWarning("TaskbarManager.RegisterWindow: prefab o parent no asignado.");
            return;
        }
        GameObject btnGo = Instantiate(taskbarButtonPrefab, taskbarIconsParent);
        TaskbarButton tbtn = btnGo.GetComponent<TaskbarButton>();
        tbtn.Setup(window, appName);
        windowToTaskButton[window] = btnGo;
        Debug.Log($"TaskbarManager: registrado window '{appName}'.");
    }

    public void UnregisterWindow(AppWindow window)
    {
        if (windowToTaskButton.TryGetValue(window, out GameObject go))
        {
            Destroy(go);
            windowToTaskButton.Remove(window);
            Debug.Log($"TaskbarManager: desregistrada window '{window.name}'.");
        }
    }
}
