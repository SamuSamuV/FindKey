/// <summary>
/// Class: TaskbarManager
/// Description: This script manages the taskbar functionality in the FindKey game, including the start menu, volume control, wallpaper selection, and taskbar icons for open applications.
///              It handles user interactions with the taskbar buttons, animates the opening and closing of menus, and keeps track of open application windows to display corresponding
///              icons on the taskbar. The script also allows for changing the desktop wallpaper through the start menu and adjusting the system volume through a dedicated volume control panel.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class TaskbarManager : MonoBehaviour
{
    public static TaskbarManager Instance { get; private set; } // Singleton pattern for easy access from other scripts

    [Header("Menu de Inicio (Start Menu)")]
    public Button menuButton;
    public GameObject menuPanel;
    public Button shutDownButton;
    public float animationDuration = 0.15f;

    [Header("Control de Volumen")]
    public Button volumeButton;
    public GameObject volumePanel;
    public Slider volumeSlider;

    [Header("Fondos de Pantalla (En el menu)")]
    public Transform wallpaperGrid;
    public GameObject wallpaperButtonPrefab;
    public WallpapersScript wallpapersScript;

    [Header("Taskbar Icons")]
    public Transform taskbarIconsParent;
    public GameObject taskbarButtonPrefab;

    private Dictionary<AppWindow, GameObject> windowToTaskButton = new Dictionary<AppWindow, GameObject>();

    public int OpenAppCount => windowToTaskButton.Count;

    private bool isMenuOpen = false;
    private bool isVolumeOpen = false;

    private Coroutine menuAnimationCoroutine;
    private Coroutine volumeAnimationCoroutine;

    private void Awake() // Singleton pattern
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() // Initialize the taskbar, set up button listeners, and populate the wallpaper menu
    {
        if (menuPanel != null)
        {
            menuPanel.transform.localScale = Vector3.zero;
            menuPanel.SetActive(false);
        }

        if (menuButton != null) menuButton.onClick.AddListener(ToggleMenu);

        if (shutDownButton != null) shutDownButton.onClick.AddListener(ShutDown);

        if (volumePanel != null)
        {
            volumePanel.transform.localScale = Vector3.zero;
            volumePanel.SetActive(false);
        }

        if (volumeButton != null) volumeButton.onClick.AddListener(ToggleVolumeMenu);

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (wallpapersScript == null) wallpapersScript = FindObjectOfType<WallpapersScript>();

        if (wallpapersScript != null && wallpapersScript.desktopBackground == null)
        {
            GameObject backgroundObj = GameObject.Find("DesktopArea");
            if (backgroundObj != null)
            {
                wallpapersScript.desktopBackground = backgroundObj.GetComponent<Image>();
            }
        }

        PopulateWallpapers();
    }

    public static TaskbarManager GetOrFindInstance() // Utility method to get the instance of TaskbarManager, or find it in the scene if not already set
    {
        if (Instance == null) Instance = FindObjectOfType<TaskbarManager>();
        return Instance;
    }

    public void ToggleMenu() // Method to toggle the start menu open or closed, with animation and ensuring that the volume menu is closed if the start menu is opened
    {
        if (menuPanel == null) return;

        isMenuOpen = !isMenuOpen;

        if (isMenuOpen && isVolumeOpen) ToggleVolumeMenu();

        if (menuAnimationCoroutine != null) StopCoroutine(menuAnimationCoroutine);
        menuAnimationCoroutine = StartCoroutine(AnimatePanel(menuPanel, isMenuOpen));
    }

    public void ToggleVolumeMenu() // Method to toggle the volume control panel open or closed, with animation and ensuring that the start menu is closed if the volume panel is opened
    {
        if (volumePanel == null) return;

        isVolumeOpen = !isVolumeOpen;

        if (isVolumeOpen && isMenuOpen) ToggleMenu();

        if (volumeAnimationCoroutine != null) StopCoroutine(volumeAnimationCoroutine);
        volumeAnimationCoroutine = StartCoroutine(AnimatePanel(volumePanel, isVolumeOpen));
    }

    public void SetVolume(float sliderValue) // Method to set the system volume based on the value of the volume slider in the volume control panel
    {
        AudioListener.volume = sliderValue;
    }

    public void CloseAllMenus() // Utility method to close both the start menu and the volume control panel, used when opening an application or changing the wallpaper to ensure a clean user interface
    {
        if (isMenuOpen) ToggleMenu();
        if (isVolumeOpen) ToggleVolumeMenu();
    }

    private IEnumerator AnimatePanel(GameObject targetPanel, bool open) // Coroutine to animate the opening or closing of a panel (either the start menu or the volume control panel) by scaling it up or down smoothly over a short duration
    {
        if (open) targetPanel.SetActive(true);

        float time = 0;
        Vector3 startScale = targetPanel.transform.localScale;
        Vector3 targetScale = open ? Vector3.one : Vector3.zero;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            t = t * (2f - t);
            targetPanel.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        targetPanel.transform.localScale = targetScale;
        if (!open) targetPanel.SetActive(false);
    }

    private void PopulateWallpapers() // Method to populate the wallpaper selection menu in the start menu with buttons for each available wallpaper, setting up the button images and click listeners to change the desktop background when a wallpaper is selected
    {
        if (wallpaperGrid == null || wallpaperButtonPrefab == null || wallpapersScript == null) return;

        foreach (Sprite wallpaper in wallpapersScript.availableWallpapers)
        {
            Sprite wallpaperToApply = wallpaper;
            GameObject newBtn = Instantiate(wallpaperButtonPrefab, wallpaperGrid);
            Image btnImage = newBtn.transform.GetChild(0).GetComponent<Image>();

            if (btnImage != null)
            {
                btnImage.sprite = wallpaperToApply;
                btnImage.preserveAspect = true;
            }

            Button btn = newBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    wallpapersScript.ChangeBackground(wallpaperToApply);
                    ToggleMenu();
                });
            }
        }
    }

    private void ShutDown()
    {
        Application.Quit();
    }

    public void RegisterWindow(AppWindow window, string appName, Sprite iconSprite) // Method to register an open application window with the taskbar manager, creating a corresponding taskbar button with the application's name and icon, and keeping track of the association between the window and its taskbar button for later removal when the window is closed
    {
        if (taskbarButtonPrefab == null || taskbarIconsParent == null) return;
        GameObject btnGo = Instantiate(taskbarButtonPrefab, taskbarIconsParent);
        TaskbarButton tbtn = btnGo.GetComponent<TaskbarButton>();
        tbtn.Setup(window, appName, iconSprite);
        windowToTaskButton[window] = btnGo;
    }

    public void UnregisterWindow(AppWindow window) // Method to unregister a closed application window from the taskbar manager, destroying the corresponding taskbar button and removing the association from the tracking dictionary
    {
        if (windowToTaskButton.TryGetValue(window, out GameObject go))
        {
            Destroy(go);
            windowToTaskButton.Remove(window);
        }
    }
}