using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class TaskbarManager : MonoBehaviour
{
    public static TaskbarManager Instance { get; private set; }

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
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

    public static TaskbarManager GetOrFindInstance()
    {
        if (Instance == null) Instance = FindObjectOfType<TaskbarManager>();
        return Instance;
    }

    public void ToggleMenu()
    {
        if (menuPanel == null) return;

        isMenuOpen = !isMenuOpen;

        if (isMenuOpen && isVolumeOpen) ToggleVolumeMenu();

        if (menuAnimationCoroutine != null) StopCoroutine(menuAnimationCoroutine);
        menuAnimationCoroutine = StartCoroutine(AnimatePanel(menuPanel, isMenuOpen));
    }

    public void ToggleVolumeMenu()
    {
        if (volumePanel == null) return;

        isVolumeOpen = !isVolumeOpen;

        if (isVolumeOpen && isMenuOpen) ToggleMenu();

        if (volumeAnimationCoroutine != null) StopCoroutine(volumeAnimationCoroutine);
        volumeAnimationCoroutine = StartCoroutine(AnimatePanel(volumePanel, isVolumeOpen));
    }

    public void SetVolume(float sliderValue)
    {
        AudioListener.volume = sliderValue;
    }

    public void CloseAllMenus()
    {
        if (isMenuOpen) ToggleMenu();
        if (isVolumeOpen) ToggleVolumeMenu();
    }

    private IEnumerator AnimatePanel(GameObject targetPanel, bool open)
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

    private void PopulateWallpapers()
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

    public void RegisterWindow(AppWindow window, string appName, Sprite iconSprite)
    {
        if (taskbarButtonPrefab == null || taskbarIconsParent == null) return;
        GameObject btnGo = Instantiate(taskbarButtonPrefab, taskbarIconsParent);
        TaskbarButton tbtn = btnGo.GetComponent<TaskbarButton>();
        tbtn.Setup(window, appName, iconSprite);
        windowToTaskButton[window] = btnGo;
    }

    public void UnregisterWindow(AppWindow window)
    {
        if (windowToTaskButton.TryGetValue(window, out GameObject go))
        {
            Destroy(go);
            windowToTaskButton.Remove(window);
        }
    }
}