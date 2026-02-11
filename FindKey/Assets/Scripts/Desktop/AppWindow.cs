using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AppWindow : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI titleText;
    public Button closeButton;
    public Button minimizeButton;
    public bool isOpen;
    [HideInInspector] public string appName;
    public MoveAppData moveAppData;

    public bool isMinimized = false;
    public float animationDuration = 0.2f;

    protected virtual void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (minimizeButton != null) minimizeButton.onClick.AddListener(Minimize);

        GameObject goMoveAppData = GameObject.FindGameObjectWithTag("MoveAppData");
        if (goMoveAppData != null)
            moveAppData = goMoveAppData.GetComponent<MoveAppData>();
    }

    protected virtual void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(AnimateWindow(Vector3.zero, Vector3.one, 0f, 1f, null));
    }

    public virtual void Setup(string title)
    {
        if (titleText) titleText.text = title;
    }

    public virtual void Close()
    {
        StartCoroutine(AnimateWindow(transform.localScale, Vector3.zero, 1f, 0f, CloseLogic));
    }

    public virtual void Minimize()
    {
        StartCoroutine(AnimateWindow(transform.localScale, Vector3.zero, 1f, 0f, MinimizeLogic));
    }

    public virtual void Restore()
    {
        if (!isMinimized) return;

        isMinimized = false;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        StartCoroutine(AnimateWindow(Vector3.zero, Vector3.one, 0f, 1f, null));
    }

    private IEnumerator AnimateWindow(Vector3 startScale, Vector3 endScale, float startAlpha, float endAlpha, Action onComplete)
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

    private void MinimizeLogic()
    {
        DesktopManager dm = FindObjectOfType<DesktopManager>();
        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                if (data.label == appName)
                {
                    data.isMinimized = true;
                    data.isOpen = true;
                    break;
                }
            }
        }

        TaskbarManager.GetOrFindInstance()?.UnregisterWindow(this);
        isMinimized = true;
        gameObject.SetActive(false);
    }

    private void CloseLogic()
    {
        DesktopManager dm = FindObjectOfType<DesktopManager>();

        if (dm != null)
        {
            foreach (var data in dm.iconsToSpawn)
            {
                // Buscamos cuál es la ventana que se está cerrando ahora mismo
                if (data.windowInstance == gameObject)
                {
                    // 1. Marcar como cerrada
                    data.isOpen = false;
                    data.isMinimized = false;
                    data.windowInstance = null; // Importante: Ya no hay ventana

                    // ----------------------------------------------------
                    // CASO A: SE ESTÁ CERRANDO "ENEMY ENCOUNTER"
                    // ----------------------------------------------------
                    if (data.label == "Enemy Encounter")
                    {
                        // Buscamos si "Move" sigue abierta para avisarle
                        foreach (var otherApp in dm.iconsToSpawn)
                        {
                            if (otherApp.label == "Move" && otherApp.isOpen && otherApp.windowInstance != null)
                            {
                                Moves moves = otherApp.windowInstance.GetComponent<Moves>();
                                if (moves != null)
                                {
                                    // Al llamar a esto, Moves verá que EnemyEncounter está cerrada
                                    // y mostrará el texto "Deberías abrir la app..."
                                    moves.GoToCatPosition();
                                }
                                break;
                            }
                        }
                    }
                    // ----------------------------------------------------
                    // CASO B: SE ESTÁ CERRANDO "MOVE"
                    // ----------------------------------------------------
                    else if (data.label == "Move")
                    {
                        // Buscamos si "Enemy Encounter" sigue abierta para resetearla
                        foreach (var otherApp in dm.iconsToSpawn)
                        {
                            if (otherApp.label == "Enemy Encounter" && otherApp.isOpen && otherApp.windowInstance != null)
                            {
                                BaseEnemyEncounter baseEnemy = otherApp.windowInstance.GetComponent<BaseEnemyEncounter>();
                                EnemyEncounterData enemyData = otherApp.windowInstance.GetComponent<EnemyEncounterData>();

                                if (baseEnemy != null)
                                {
                                    // Ponemos la pantalla de "No Signal"
                                    baseEnemy.nonEnemyFindedPanel.SetActive(true);
                                }

                                if (enemyData != null)
                                {
                                    // Quitamos el gato de la pantalla
                                    enemyData.CurrentType = EnemyEncounterData.NPCType.None;
                                }
                                break;
                            }
                        }
                    }
                    // ----------------------------------------------------

                    break; // Ya encontramos la ventana que se cierra, salimos del bucle principal
                }
            }
        }

        Destroy(gameObject);
    }

    public void ToggleMinimizeRestore()
    {
        if (isMinimized) Restore();
        else Minimize();
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BringToFront();
    }
}