using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    protected virtual void Awake()
    {
        if (closeButton != null)
        {
            for (int i = 0; i < closeButton.Length; i++)
            {
                closeButton[i].onClick.AddListener(Close);
            }
        }

        if (minimizeButton != null)
        {
            for (int i = 0; i < minimizeButton.Length; i++)
            {
                minimizeButton[i].onClick.AddListener(Minimize);
            }
        }

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
        SoundManager.Instance?.Play("close");
        StartCoroutine(AnimateWindow(transform.localScale, Vector3.zero, 1f, 0f, CloseLogic));
    }

    public virtual void Minimize()
    {
        SoundManager.Instance?.Play("minimize");
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

    public void Reopen()
    {
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

        isMinimized = true;
        gameObject.SetActive(false);
    }

    private void CloseLogic()
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

                    if (data.label == "Enemy Encounter")
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
                            if (otherApp.label == "Enemy Encounter" && otherApp.isOpen && otherApp.windowInstance != null)
                            {
                                BaseEnemyEncounter baseEnemy = otherApp.windowInstance.GetComponent<BaseEnemyEncounter>();
                                EnemyEncounterData enemyData = otherApp.windowInstance.GetComponent<EnemyEncounterData>();

                                if (baseEnemy != null) baseEnemy.nonEnemyFindedPanel.SetActive(true);
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

    public void ShakeWindow(float duration, float magnitude)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
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
}