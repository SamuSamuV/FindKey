using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class: AppWindow
/// Description: This script manages the behavior of application windows in the FindKey game. It handles opening, closing, minimizing, and restoring windows, as well as animating these
///              transitions. The script also interacts with the TaskbarManager to register and unregister windows, and with the DesktopManager to update the state of desktop icons. It includes
///              functionality for shaking the window for visual feedback and for forcing the window to open in the center of the screen if desired. The script is designed to be flexible and can be
///              extended for specific types of application windows by overriding its methods.
/// Author: Samuel Campos Borrego
/// Project: FindKey
/// </summary>
public class AppWindow : MonoBehaviour, IPointerDownHandler
{
    private Coroutine shakeCoroutine;

    [Tooltip("Si se marca, al cerrar la app no se destruirá, conservando su contenido. Solo se ocultará.")]
    public bool keepAliveOnClose = false;

    public TextMeshProUGUI titleText;
    public Button[] closeButton;
    public Button[] minimizeButton;
    public bool isOpen;
    [HideInInspector] public string appName;
    public MoveAppData moveAppData;

    public bool isMinimized = false;
    public float animationDuration = 0.2f;

    [Header("Opciones de Posición")]
    [Tooltip("Si se activa, esta ventana ignorará el efecto cascada y siempre se abrirá en el centro de la pantalla.")]
    public bool forceCenterOnOpen = false;

    protected virtual void Awake()
    {
        if (closeButton != null)
        {
            for (int i = 0; i < closeButton.Length; i++) // Use a for loop to avoid closure issues with lambda
            {
                closeButton[i].onClick.AddListener(Close);
            }
        }

        if (minimizeButton != null)
        {
            for (int i = 0; i < minimizeButton.Length; i++) // Use a for loop to avoid closure issues with lambda
            {
                minimizeButton[i].onClick.AddListener(Minimize); 
            }
        }

        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
            moveAppData = goMoveAppData.GetComponent<MoveAppData>();
    }

    protected virtual void Start() // Open with a scale of 0 and animate to full size
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(AnimateWindow(Vector3.zero, Vector3.one, 0f, 1f, null));
    }

    protected virtual void OnEnable() // Force center on open if the option is enabled
    {
        if (forceCenterOnOpen)
        {
            StartCoroutine(ForceCenterRoutine());
        }
    }

    private System.Collections.IEnumerator ForceCenterRoutine() // Wait until the end of the frame to ensure all layout calculations are done, then set anchoredPosition to zero
    {
        yield return new WaitForEndOfFrame();

        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
        }
    }

    public virtual void Setup(string title) // Set the title and register with TaskbarManager
    {
        if (titleText) titleText.text = title;
    }

    public virtual void Close() // Animate closing and then call CloseLogic to handle the actual closing behavior
    {
        SoundManager.Instance?.Play("close");
        StartCoroutine(AnimateWindow(transform.localScale, Vector3.zero, 1f, 0f, CloseLogic));
    }

    public virtual void Minimize() // Animate minimizing and then call MinimizeLogic to handle the actual minimizing behavior
    {
        SoundManager.Instance?.Play("minimize");
        StartCoroutine(AnimateWindow(transform.localScale, Vector3.zero, 1f, 0f, MinimizeLogic));
    }

    public virtual void Restore() // If the window is minimized, animate restoring it to full size and make it active again
    {
        if (!isMinimized) return;

        isMinimized = false;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        StartCoroutine(AnimateWindow(Vector3.zero, Vector3.one, 0f, 1f, null));
    }

    public void Reopen() // If the window is closed but not destroyed (keepAliveOnClose), animate reopening it to full size and make it active again
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        StartCoroutine(AnimateWindow(Vector3.zero, Vector3.one, 0f, 1f, null));
    }

    private IEnumerator AnimateWindow(Vector3 startScale, Vector3 endScale, float startAlpha, float endAlpha, Action onComplete) // Animate the window's scale and alpha over time, then call onComplete when done
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        float time = 0;
        transform.localScale = startScale;
        cg.alpha = startAlpha;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        transform.localScale = endScale;
        cg.alpha = endAlpha;
        onComplete?.Invoke();
    }

    private void MinimizeLogic() // Update the DesktopManager to mark this app as minimized and open, then set the window as inactive
    {
        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn) // Find the corresponding desktop icon data for this app and mark it as minimized and open
            {
                if (data.label == appName)
                {
                    data.isMinimized = true;
                    data.isOpen = true;
                    break;
                }
            }
        }

        isMinimized = true;
        gameObject.SetActive(false);
    }

    private void CloseLogic() // Unregister from TaskbarManager, update DesktopManager to mark this app as closed (and not minimized), and either destroy or deactivate the window based on keepAliveOnClose
    {
        TaskbarManager.GetOrFindInstance()?.UnregisterWindow(this);

        DesktopManager dm = FindObjectOfType<DesktopManager>();

        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.windowInstance == gameObject)
                {
                    data.isOpen = false;
                    data.isMinimized = false;

                    if (!keepAliveOnClose)
                    {
                        data.windowInstance = null;
                    }

                    if (data.label == "Buscador Enemigos")
                    {
                        foreach (var otherApp in dm.iconsToSpawn)
                        {
                            if (otherApp.label == "FindKey.exe" && otherApp.isOpen && otherApp.windowInstance != null)
                            {
                                Moves moves = otherApp.windowInstance.GetComponent<Moves>();
                                if (moves != null && moves.moveAppData.playerIsFrontCat)
                                {
                                    moves.GoToCatPosition();
                                }
                                break;
                            }
                        }
                    }
                    else if (data.label == "FindKey.exe")
                    {
                        moveAppData.playerIsFrontCat = false;

                        foreach (var otherApp in dm.iconsToSpawn)
                        {
                            if (otherApp.label == "Buscador Enemigos" && otherApp.isOpen && otherApp.windowInstance != null)
                            {
                                BaseEnemyEncounter baseEnemy = otherApp.windowInstance.GetComponent<BaseEnemyEncounter>();
                                EnemyEncounterData enemyData = otherApp.windowInstance.GetComponent<EnemyEncounterData>();

                                if (baseEnemy != null) baseEnemy.ResetEncounter();
                                if (enemyData != null) enemyData.CurrentType = EnemyEncounterData.NPCType.None;
                                break;
                            }
                        }
                    }
                    break;
                }
            }
        }

        if (keepAliveOnClose)
        {
            gameObject.SetActive(false);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMinimizeRestore() // If the window is currently minimized, restore it; otherwise, minimize it
    {
        if (isMinimized) Restore();
        else Minimize();
    }

    public void BringToFront() // Move this window to the end of its parent's children, making it render on top of other sibling windows
    {
        transform.SetAsLastSibling();
    }

    public void OnPointerDown(PointerEventData eventData) // When the window is clicked, bring it to the front
    {
        BringToFront();
    }

    public void ShakeWindow(float duration, float magnitude) // If the window is already shaking, stop the current shake coroutine before starting a new one to prevent overlapping shakes
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude) // Shake the window by randomly offsetting its local position for the specified duration, then return it to its original position
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void SetCloseAndMinimizeInteractable(bool state) // Enable or disable the interactability of the close and minimize buttons, if they exist
    {
        if (closeButton != null)
        {
            foreach (var btn in closeButton) btn.interactable = state;
        }

        if (minimizeButton != null)
        {
            foreach (var btn in minimizeButton) btn.interactable = state;
        }
    }
}